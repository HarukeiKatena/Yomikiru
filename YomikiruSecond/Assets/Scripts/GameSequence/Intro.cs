using System;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Yomikiru.Audio;
using Yomikiru.Characte.Management;

namespace Yomikiru.Intro
{
    public class Intro : MonoBehaviour
    {
        [SerializeField] private MatchInfo matchInfo;
        [SerializeField] private CharacterManagement characterManagement;
        [SerializeField] private AudioChannel audioChannel;
        [SerializeField] private Yomikiru.UI.IntroDisplay display;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CinemachineDollyCart dollyCart;
        [SerializeField] private LightSetting lightSetting;

        [Header("設定")]
        [SerializeField] private float cameraMoveSpeed = 5.0f; //カメラの移動速度
        [SerializeField] private float cameraRectSlideSpeed = 2.0f; //複数人プレイ時画面が分割される速度
        [SerializeField] private float lightFadeSpeed = 1.0f; //ライトのフェード速度
        [SerializeField] private float startControlTime = 2.5f; //イントロアニメが終わって操作可能までの時間
        [SerializeField] private float introStartCoolTime = 0.5f; //シーン開始時すぐ動かないようにする時間
        [SerializeField] private AudioCue introAudio; //イントロ開始時に流れる曲
        [SerializeField] private AudioCue gameStartAudio; //操作可能になるタイミングの曲

        private void Start()
        {
            var cts = new CancellationTokenSource();
            IntroSequence(cts.Token).Forget();
        }


        private async UniTask IntroSequence(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            matchInfo.State = MatchState.Intro;

            //一時的にカメラを保持して全画面にする
            characterManagement.CharacterList[0].transform.Find("Eye").Find("Camera").TryGetComponent(out Camera camera);
            Rect rect = new Rect(camera.rect);
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

            //シーン開幕時少し待つ
            await UniTask.Delay(TimeSpan.FromSeconds(introStartCoolTime), cancellationToken: token);

            //イントロ開始時の音を流す
            //audioChannel.Request(introAudio);

            //イントロ開始
            display.DisplayModeAsync(matchInfo.Gamemode).Forget();

            //カメラ移動
            await DOVirtual.Float(0.0f, 1.0f, cameraMoveSpeed, v => { dollyCart.m_Position = v; }).SetEase(Ease.OutCubic)
                .ToUniTask(cancellationToken: token);

            //カメラのRectを元に戻す
            camera.DORect(rect, cameraRectSlideSpeed);

            //カメラ移動終わり
            virtualCamera.Priority = 0;
            lightSetting.LightFade(lightFadeSpeed);
            display.DisplayReadyAsync().Forget();

            //コントロール可能までの待ち時間
            await UniTask.Delay(TimeSpan.FromSeconds(startControlTime), cancellationToken: token);

            matchInfo.State = MatchState.Ingame;
            display.DisplayGoAsync().Forget();

            //audioChannel.Request(gameStartAudio);
        }
    }
}
