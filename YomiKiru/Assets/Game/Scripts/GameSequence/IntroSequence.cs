using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Yomikiru.Sound;

public class IntroSequence : MonoBehaviour
{
    [SerializeField]protected GameScriptableObject matchInfo;
    [SerializeField]protected CinemachineVirtualCamera VirtualCamera;
    [SerializeField]protected CinemachineDollyCart DollyCart;
    [SerializeField]protected LightSetting lightSetting;
    [SerializeField]protected PlayerManagement playerManagement;
    [SerializeField]protected Yomikiru.UI.IntroDisplay display;
    [SerializeField]protected SoundManager soundManager;

    [Header("設定")]
    [SerializeField, Tooltip("イントロカメラの移動速度")]
    protected float CameraMoveSpeed = 5.0f;
    [SerializeField, Tooltip("複数人プレイの場合画面が分割される速度")]
    protected float CameraRectSlideSpeed = 2.0f;
    [SerializeField, Tooltip("カメラが開始位置に移動して操作出来るようになるまでの待ち時間")]
    protected float StartControlTime = 2.5f;
    [SerializeField, Tooltip("ライトフェード速度")]
    protected float LightFadeSpeed = 1.0f;
    [SerializeField, Tooltip("ゲームシーン開始時にすぐ動かないようにするための待ち時間")]
    protected float IntroStartCoolTime = 0.5f;
    [SerializeField, Tooltip("イントロ開始時に流れる曲")]
    protected AudioClip bgmIntroAudio;
    [SerializeField, Tooltip("操作可能になるタイミングでなるSE")]
    protected AudioClip seGameStartAudio;

    public static int INTRO_START = 0;
    public static int INTRO_END = 1;
    public IObservable<int> IntroIvent => introIvent;
    private readonly Subject<int> introIvent = new Subject<int>(); 
    

    


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Intro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Intro()
    {
        //一時的に保持して全画面表記にする
        Camera camera = playerManagement.CharacterCamera[0].transform.GetChild(1).GetComponent<Camera>();
        Rect rect = new Rect();
        rect = camera.rect;
        camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        yield return new WaitForSeconds(IntroStartCoolTime);

        soundManager.PlaySE(bgmIntroAudio.name, camera.transform);

        //イントロ開始
        introIvent.OnNext(INTRO_START);
        display.DisplayModeAsync(matchInfo.Gamemode).Forget();

        //カメラ移動開始
        yield return DOVirtual.Float(0.0f, 1.0f, CameraMoveSpeed, v => DollyCart.m_Position = v).SetEase(Ease.OutCubic).WaitForCompletion();

        //カメラのRectを基に戻す
        camera.DORect(rect, CameraRectSlideSpeed);

        //カメラ移動終わり
        VirtualCamera.Priority = 0;
        lightSetting.LightFade(LightFadeSpeed);
        display.DisplayReadyAsync().Forget();

        //コントロール可能までの待ち時間
        yield return new WaitForSeconds(StartControlTime);
        
        introIvent.OnNext(INTRO_END);
        introIvent.OnCompleted();
        display.DisplayGoAsync().Forget();

        //流れない
        soundManager.PlaySE(seGameStartAudio.name, camera.transform);
    }
}