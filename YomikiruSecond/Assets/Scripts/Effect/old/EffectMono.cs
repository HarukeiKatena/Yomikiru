using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Yomikiru.Utility;

namespace Yomikiru.Effect
{
    [RequireComponent(typeof(Transformation))]
    public class EffectMono : MonoBehaviour
    {
        [SerializeField] private EffectTable _table;

        public bool isPlaying { get; private set; }

        private EffectData _data;
        private Transformation _transform;

        private void Awake() => TryGetComponent(out _transform);

        private void Start() => isPlaying = false;

        public void SetEffect(EffectData data)
        {
            gameObject.SetActive(false);

            _data = data;
            _transform.data = data.transform;
        }

        private async UniTask Play(Ease ease, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            gameObject.SetActive(true);

            bool canceled;
            if (_data.isDynamic is true)
            {
                canceled = await _transform.DoTransform(ease, true, ct).SuppressCancellationThrow();
            }
            else
            {
                canceled = await UniTask.Delay((int)(_data.lifeTime * 1000.0f), cancellationToken: ct).SuppressCancellationThrow();
            }
            if (canceled is true) return;

            gameObject.SetActive(false);
        }

        public async void Play(Vector3 pos, Ease ease = Ease.Linear)
        {
            isPlaying = true;
            transform.position = pos;

            var canceled = await Play(ease, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            if (canceled is true) return;

            transform.position = Vector3.zero;
            isPlaying = false;
        }

        public async void Play(Transform parent, Ease ease = Ease.Linear)
        {
            var before = transform.parent;

            isPlaying = true;
            transform.SetParent(parent, false);

            var canceled = await Play(ease, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            if (canceled is true) return;

            transform.SetParent(before, false);
            isPlaying = false;
        }
    }
}
