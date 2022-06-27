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
    [RequireComponent(typeof(Transform), typeof(MeshFilter), typeof(MeshRenderer))]
    public class EffectOperation : MonoBehaviour
    {
        public bool isPlaying { get; private set; } = false;

        private EffectClip clip;
        private MeshFilter mesh;
        private MeshRenderer renderer;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private void Start()
        {
            TryGetComponent(out mesh);
            TryGetComponent(out renderer);
        }

        //再生する
        public void Play(EffectClip clip)
        {
            this.clip = clip;
            EffectPlay(cancellationTokenSource.Token).Forget();
        }

        private async UniTask EffectPlay(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            //開始
            isPlaying = true;
            var t = transform;
            t.position = clip.BacePosition;
            t.rotation = clip.BaceRotation;
            t.localScale = clip.StartScale;
            mesh.mesh = clip.ObjectMesh;
            renderer.material = clip.EffectMaterial;

            //スケール変更
            await transform.DOScale(clip.EndScale, clip.Time).ToUniTask(cancellationToken: token);

            //終了
            t.localScale = Vector3.zero;
            isPlaying = false;
        }
    }
}
