using System;
using System.Collections;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Yomikiru;
using Yomikiru.Sound;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private MatchInfo matchInfo;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private LightSetting lightSetting;
    [SerializeField] private PlayerManagement playerManagement;
    [SerializeField] private Yomikiru.UI.IntroDisplay display;
    [SerializeField] private SoundManager soundManager;

    [FormerlySerializedAs("CameraMoveSpeed")]
    [Header("設定")]
    [Tooltip("イントロカメラの移動速度")]
    [SerializeField] private float cameraMoveSpeed = 5.0f;
    [Tooltip("複数人プレイの場合画面が分割される速度")]
    [SerializeField] private float cameraRectSlideSpeed = 2.0f;
    [Tooltip("カメラが開始位置に移動して操作出来るようになるまでの待ち時間")]
    [SerializeField] private float startControlTime = 2.5f;
    [Tooltip("ライトフェード速度")]
    [SerializeField] private float lightFadeSpeed = 1.0f;
    [Tooltip("ゲームシーン開始時にすぐ動かないようにするための待ち時間")]
    [SerializeField] private float introStartCoolTime = 0.5f;
    [Tooltip("イントロ開始時に流れる曲")]
    [SerializeField] private AudioClip bgmIntroAudio;
    [Tooltip("操作可能になるタイミングでなるSE")]
    [SerializeField] private AudioClip seGameStartAudio;

    public static int INTRO_START = 0;
    public static int INTRO_END = 1;
    public IObservable<int> IntroIvent => introIvent;
    private readonly Subject<int> introIvent = new Subject<int>();





    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Intro());
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