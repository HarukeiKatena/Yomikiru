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

    [Header("�ݒ�")]
    [SerializeField, Tooltip("�C���g���J�����̈ړ����x")]
    protected float CameraMoveSpeed = 5.0f;
    [SerializeField, Tooltip("�����l�v���C�̏ꍇ��ʂ���������鑬�x")]
    protected float CameraRectSlideSpeed = 2.0f;
    [SerializeField, Tooltip("�J�������J�n�ʒu�Ɉړ����đ���o����悤�ɂȂ�܂ł̑҂�����")]
    protected float StartControlTime = 2.5f;
    [SerializeField, Tooltip("���C�g�t�F�[�h���x")]
    protected float LightFadeSpeed = 1.0f;
    [SerializeField, Tooltip("�Q�[���V�[���J�n���ɂ��������Ȃ��悤�ɂ��邽�߂̑҂�����")]
    protected float IntroStartCoolTime = 0.5f;
    [SerializeField, Tooltip("�C���g���J�n���ɗ�����")]
    protected AudioClip bgmIntroAudio;
    [SerializeField, Tooltip("����\�ɂȂ�^�C�~���O�łȂ�SE")]
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
        //�ꎞ�I�ɕێ����đS��ʕ\�L�ɂ���
        Camera camera = playerManagement.CharacterCamera[0].transform.GetChild(1).GetComponent<Camera>();
        Rect rect = new Rect();
        rect = camera.rect;
        camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        yield return new WaitForSeconds(IntroStartCoolTime);

        soundManager.PlaySE(bgmIntroAudio.name, camera.transform);

        //�C���g���J�n
        introIvent.OnNext(INTRO_START);
        display.DisplayModeAsync(matchInfo.Gamemode).Forget();

        //�J�����ړ��J�n
        yield return DOVirtual.Float(0.0f, 1.0f, CameraMoveSpeed, v => DollyCart.m_Position = v).SetEase(Ease.OutCubic).WaitForCompletion();

        //�J������Rect����ɖ߂�
        camera.DORect(rect, CameraRectSlideSpeed);

        //�J�����ړ��I���
        VirtualCamera.Priority = 0;
        lightSetting.LightFade(LightFadeSpeed);
        display.DisplayReadyAsync().Forget();

        //�R���g���[���\�܂ł̑҂�����
        yield return new WaitForSeconds(StartControlTime);
        
        introIvent.OnNext(INTRO_END);
        introIvent.OnCompleted();
        display.DisplayGoAsync().Forget();

        //����Ȃ�
        soundManager.PlaySE(seGameStartAudio.name, camera.transform);
    }
}