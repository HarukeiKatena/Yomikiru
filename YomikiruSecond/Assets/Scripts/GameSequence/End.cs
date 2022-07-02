using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        matchInfo.OnStateChange.
            Where(x => x == MatchState.Finished).Subscribe(x =>
            {
                EndSequence().Forget();
            });
    }

    private async UniTask EndSequence()
    {
        display.DisplayAsync().Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(cameraMoveDelay));
        endTargetVirtualCamera.Priority = 20;

        await UniTask.Delay(TimeSpan.FromSeconds(endWaitTime));

        //とりあえずこっちに書く
        controllerManager.ClearPlayerDevice();

        GameManager.Instance.LoadScene("Results", GameManager.LoadingScreenType.DARK);
    }
}
