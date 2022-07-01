using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        [Header("設定")]
        [SerializeField] private float cameraMoveSpeed = 5.0f; //カメラの移動速度
        [SerializeField] private float cameraRectSlideSpeed = 2.0f; //複数人プレイ時画面が分割される速度
        [SerializeField] private float lightFadeSpeed = 1.0f; //ライトのフェード速度
        [SerializeField] private float startControlTime = 2.5f; //イントロアニメが終わって操作可能までの時間
        [SerializeField] private float introStartCoolTime = 0.5f; //シーン開始時すぐ動かないようにする時間
        [SerializeField] private AudioClip bgmIntroAudio; //イントロ開始時に流れる曲
        [SerializeField] private AudioClip seGameStartAudio; //操作可能になるタイミングの曲


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
    }
}
