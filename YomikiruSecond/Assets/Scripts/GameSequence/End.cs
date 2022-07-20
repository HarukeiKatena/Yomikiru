using System;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Yomikiru;
using Yomikiru.Controller;

public class End : MonoBehaviour
{
    [SerializeField] private MatchInfo matchInfo;
    [SerializeField] private CinemachineVirtualCamera endTargetVirtualCamera;
    [SerializeField] private Yomikiru.UI.EndDisplay display;
    [SerializeField] private ControllerManager controllerManager;
    [SerializeField] private float cameraMoveDelay = 1.0f;
    [SerializeField] private float endWaitTime = 2.0f;
    [SerializeField] private string nextScene = "ResultScene";

    private void Start()
    {
        endTargetVirtualCamera.Priority = 0;

        matchInfo.OnStateChange.
            Where(x => x == MatchState.Finished).Subscribe(x =>
            {
                var cts = new CancellationTokenSource();
                EndSequence(cts.Token).Forget();
            });
    }

    private async UniTask EndSequence(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        display.DisplayAsync().Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(cameraMoveDelay), cancellationToken: token);
        endTargetVirtualCamera.Priority = 20;

        await UniTask.Delay(TimeSpan.FromSeconds(endWaitTime), cancellationToken: token);

        //とりあえずこっちに書く
        controllerManager.ClearPlayerDevice();

        GameManager.Instance.LoadScene(nextScene, GameManager.LoadingScreenType.DARK);
    }
}
