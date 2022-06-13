using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Yomikiru
{
    public sealed class LoadingScreen : MonoBehaviour
    {

        [SerializeField] private float fadeInDuration;
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private RectTransform bar;
        private bool isVisible = false;
        private CancellationTokenSource barAnimationCancel;

        public bool IsVisible => isVisible;

        public async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            barAnimationCancel?.Cancel();
            barAnimationCancel = new CancellationTokenSource();
            AnimateBar(barAnimationCancel.Token).Forget();
            await group.DOFade(1, fadeInDuration).From(0);
        }

        public async UniTask HideAsync()
        {
            await group.DOFade(0, fadeOutDuration).From(1);
            gameObject.SetActive(false);
            barAnimationCancel?.Cancel();
        }

        private async UniTaskVoid AnimateBar(CancellationToken cancelToken)
        {
            while (true)
            {
                bar.pivot = new Vector2(0, 0.5f);
                await bar.DOScaleX(1, 0.5f).From(0).SetEase(Ease.OutCubic).WithCancellation(cancelToken);
                if (cancelToken.IsCancellationRequested) break;
                bar.pivot = new Vector2(1, 0.5f);
                await bar.DOScaleX(0, 0.5f).From(1).SetEase(Ease.OutCubic).WithCancellation(cancelToken);
                if (cancelToken.IsCancellationRequested) break;
            }
        }

    }
}
