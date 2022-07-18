using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Yomikiru.Effect
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class EffectOperation : MonoBehaviour
    {
        public bool isPlaying { get; private set; } = false;
        public IObservable<Unit> OnStopEffect => onStopEffect;


        private MeshFilter mesh;
        private MeshRenderer renderer;
        private Subject<Unit> onStopEffect = new Subject<Unit>();

        private void Awake()
        {
            //startだとPlayで呼ばれる頃に参照が消えてるためこっちに書いた
            TryGetComponent(out mesh);
            TryGetComponent(out renderer);
        }

        //再生する
        public void Play(EffectClip clip, Vector3 pos)
        {
            EffectPlay(this.GetCancellationTokenOnDestroy(), clip, pos).Forget();
        }

        private async UniTask EffectPlay(CancellationToken token, EffectClip clip, Vector3 pos)
        {
            token.ThrowIfCancellationRequested();

            //開始
            isPlaying = true;
            var t = transform;
            t.position = clip.BacePosition + pos;
            t.rotation = clip.BaceRotation;
            t.localScale = clip.StartScale;
            renderer.material = clip.EffectMaterial;
            mesh.mesh = clip.ObjectMesh;

            //スケール変更
            await transform.DOScale(clip.EndScale, clip.Time).ToUniTask(cancellationToken: token);

            //終了
            t.localScale = Vector3.zero;
            isPlaying = false;
            onStopEffect.OnNext(Unit.Default);
        }
    }
}
