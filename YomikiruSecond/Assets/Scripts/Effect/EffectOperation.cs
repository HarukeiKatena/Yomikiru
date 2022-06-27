using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Yomikiru.Effect;

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
            TryGetComponent(out mesh);
            TryGetComponent(out renderer);
        }

        //再生する
        public void Play(EffectClip clip)
        {
            var t = transform;
            t.position = clip.BacePosition;
            t.rotation = clip.BaceRotation;
            t.localScale = clip.StartScale;
            renderer.material = clip.EffectMaterial;
            mesh.mesh = clip.ObjectMesh;
            EffectPlay(this.GetCancellationTokenOnDestroy(), clip).Forget();
        }

        private async UniTask EffectPlay(CancellationToken token, EffectClip clip)
        {
            token.ThrowIfCancellationRequested();

            //開始
            isPlaying = true;


            //スケール変更
            await transform.DOScale(clip.EndScale, clip.Time).ToUniTask(cancellationToken: token);

            //終了
            transform.localScale = Vector3.zero;
            isPlaying = false;
            onStopEffect.OnNext(Unit.Default);
        }
    }
}
