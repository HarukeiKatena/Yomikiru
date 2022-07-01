using System;
using System.Collections;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Yomikiru;
using Yomikiru.Audio;
using Yomikiru.Characte.Management;
using Yomikiru.Sound;

public class IntroSequence : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private LightSetting lightSetting;
    [SerializeField] private PlayerManagement playerManagement;
    [SerializeField] private Yomikiru.UI.IntroDisplay display;
    [SerializeField] private SoundManager soundManager;

    [SerializeField] private MatchInfo matchInfo;
    [SerializeField] private CharacterManagement characterManagement;
    [SerializeField] private AudioChannel audioChannel;


    [Header("設定")]
    [SerializeField] private float cameraMoveSpeed = 5.0f; //カメラの移動速度
    [SerializeField] private float cameraRectSlideSpeed = 2.0f; //複数人プレイ時画面が分割される速度
    [SerializeField] private float lightFadeSpeed = 1.0f; //ライトのフェード速度
    [SerializeField] private float startControlTime = 2.5f; //イントロアニメが終わって操作可能までの時間
    [SerializeField] private float introStartCoolTime = 0.5f; //シーン開始時すぐ動かないようにする時間
    [SerializeField] private AudioClip bgmIntroAudio; //イントロ開始時に流れる曲
    [SerializeField] private AudioClip seGameStartAudio; //操作可能になるタイミングの曲

    public static int INTRO_START = 0;
    public static int INTRO_END = 1;
    public IObservable<int> IntroIvent => introIvent;
    private readonly Subject<int> introIvent = new Subject<int>();





    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Intro());
    }

    private async UniTask IntroS()
    {
        matchInfo.State = MatchState.Intro;

        //一時的にカメラを保持して全画面にする
        Camera camera = characterManagement.GetCharacterCamera(0);
        Rect rect = new Rect(camera.rect);
        camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        await UniTask.Delay(TimeSpan.FromSeconds(introStartCoolTime));

        //ここでサウンド流す処理

        //イントロ開始

    }

    IEnumerator Intro()
    {
        matchInfo.State = MatchState.Intro;

        //一時的に保持して全画面表記にする
        Camera camera = playerManagement.CharacterCamera[0].transform.GetChild(1).GetComponent<Camera>();
        Rect rect = new Rect();
        rect = camera.rect;
        camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        yield return new WaitForSeconds(introStartCoolTime);

        soundManager.PlaySE(bgmIntroAudio.name, camera.transform);

        //イントロ開始
        introIvent.OnNext(INTRO_START);
        display.DisplayModeAsync(matchInfo.Gamemode).Forget();

        //カメラ移動開始
        yield return DOVirtual.Float(0.0f, 1.0f, cameraMoveSpeed, v => dollyCart.m_Position = v).SetEase(Ease.OutCubic).WaitForCompletion();

        //カメラのRectを基に戻す
        camera.DORect(rect, cameraRectSlideSpeed);

        //カメラ移動終わり
        virtualCamera.Priority = 0;
        lightSetting.LightFade(lightFadeSpeed);
        display.DisplayReadyAsync().Forget();

        //コントロール可能までの待ち時間
        yield return new WaitForSeconds(startControlTime);

        matchInfo.State = MatchState.Ingame;
        introIvent.OnNext(INTRO_END);
        introIvent.OnCompleted();
        display.DisplayGoAsync().Forget();

        //流れない
        soundManager.PlaySE(seGameStartAudio.name, camera.transform);
    }
}