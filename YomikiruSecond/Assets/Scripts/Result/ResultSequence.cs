using System;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Yomikiru.Result
{
    public class ResultSequence : MonoBehaviour
    {
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private CinemachineDollyCart dollyCart;
        [SerializeField] private float cameraMoveTime = 20.0f;
        [SerializeField] private string nextSceneName = "Title";

        public IObservable<Unit> OnStopCamera => onStopCamera;
        private Subject<Unit> onStopCamera = new Subject<Unit>();

        private void Start()
        {
            playerAnimator.SetBool("isWin", true);

            Result(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask Result(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            //カメラ移動
            await DOVirtual.Float(0.0f, 1.0f, cameraMoveTime, x => dollyCart.m_Position = x).SetEase(Ease.OutCubic)
                .ToUniTask(cancellationToken: token);

            onStopCamera.OnNext(Unit.Default);
            onStopCamera.OnCompleted();

            await UniTask.Delay(3, cancellationToken: token);

            GameManager.Instance.LoadScene(nextSceneName, GameManager.LoadingScreenType.DARK);
        }
    }
}
