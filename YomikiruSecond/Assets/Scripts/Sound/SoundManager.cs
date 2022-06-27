using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Yomikiru.Sound
{
    public class SoundManager : MonoBehaviour
    {
        public struct SoundObject
        {
            public string ClipName;
            public Vector3 Position;
            public float Delay;

            public SoundObject(string Name, Vector3 Posi, float Delay = 0.0f)
            {
                ClipName = Name;
                Position = Posi;
                this.Delay = Delay;
            }

            public SoundObject(string Name)
            {
                ClipName = Name;
                Position = Vector3.zero;
                Delay = 0.0f;
            }
        }

        // 外部パラメーター
        [Header("Data Table")]
        [SerializeField] private SoundTable dataTable;

        [Header("Object Pool")]
        [SerializeField] private Transform objectPoolParentSE;
        [SerializeField] private Transform objectPoolParentBGM;

        [Header("SoundEcho")]
        [SerializeField] private SoundEcho soundEcho;

        // 内部パラメーター
        private List<SoundMono> seInsntaces = new List<SoundMono>();
        private SoundMono[] bgmInstances = new SoundMono[2];
        private bool bgmSwapped = false;

        private void Start()
        {
            for (int i = 0; i < dataTable._sePlayMax; i++)
            {
                var obj = Instantiate(dataTable._seTemplate, objectPoolParentSE);
                seInsntaces.Add(obj.GetComponent<SoundMono>());
            }
            for (int i = 0; i < 2; i++)
            {
                var obj = Instantiate(dataTable._bgmTemplate, objectPoolParentBGM);
                bgmInstances[i] = obj.GetComponent<SoundMono>();
            }

            soundEcho.OnSoundSE.Subscribe(x => {
                PlaySE(x.Sound);
            });

            soundEcho.OnSoundBGM.Subscribe(x => {
                PlayBGM(x.Sound.ClipName);
            });
        }

        public void PlaySE(string clipName, Vector3 pos, float delay = 0.0f)
        {
            var sound = seInsntaces.Find(n => n.isPlaying is false);
            if (sound is null) return;

            var data = dataTable._seList.Find(n => n.clip.name == clipName);
            if (data.clip is null) return;

            sound.SetData(data);
            sound.Play(pos, delay);
        }

        public void PlaySE(string clipName, Transform parent, float delay = 0.0f)
        {
            var sound = seInsntaces.Find(n => n.isPlaying is false);
            if (sound is null) return;

            var data = dataTable._seList.Find(n => n.clip.name == clipName);
            if (data.clip is null) return;

            sound.SetData(data);
            sound.Play(parent, delay);
        }

        public void PlaySE(SoundObject soundObject)
        {
            var sound = seInsntaces.Find(n => n.isPlaying is false);
            if (sound is null) return;

            var data = dataTable._seList.Find(n => n.clip.name == soundObject.ClipName);
            if (data.clip is null) return;

            sound.SetData(data);
            sound.Play(soundObject.Position, soundObject.Delay);
        }

        public void PlayBGM(string clipName)
        {
            var data = dataTable._bgmList.Find(n => n.clip.name == clipName);
            if (data.clip is null) return;

            var before = bgmInstances[Convert.ToInt32( bgmSwapped)].GetComponent<SoundMono>();
            var after  = bgmInstances[Convert.ToInt32(!bgmSwapped)].GetComponent<SoundMono>();

            bgmSwapped = !bgmSwapped;

            after.SetData(data);

            before.FadeOut(dataTable._bgmSwapTime, before.volume, 0.0f);
            after.FadeIn(dataTable._bgmSwapTime, 0.0f, data.volume);

            after.Play(Vector3.zero);

            before.Stop();
        }
    }
}
