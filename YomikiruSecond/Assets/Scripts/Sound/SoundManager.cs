using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Yomikiru.Sound
{
    public class SoundManager : MonoBehaviour
    {
        // 外部パラメーター
        [Header("Data Table")]
        [SerializeField] private SoundTable _table = null;

        [Header("Object Pool")]
        [SerializeField] private Transform _se = null;
        [SerializeField] private Transform _bgm = null;

        // 内部パラメーター
        private List<SoundMono> _seInsntaces = new List<SoundMono>();
        private SoundMono[] _bgmInstances = new SoundMono[2];
        private bool _bgmSwapped = false;

        private void Start()
        {
            for (int i = 0; i < _table._sePlayMax; i++)
            {
                var obj = Instantiate(_table._seTemplate, _se);
                _seInsntaces.Add(obj.GetComponent<SoundMono>());
            }
            for (int i = 0; i < 2; i++)
            {
                var obj = Instantiate(_table._bgmTemplate, _bgm);
                _bgmInstances[i] = obj.GetComponent<SoundMono>();
            }
        }

        public void PlaySE(string clipName, Vector3 pos, float delay = 0.0f)
        {
            var sound = _seInsntaces.Find(n => n.isPlaying is false);
            if (sound is null) return;

            var data = _table._seList.Find(n => n.clip.name == clipName);
            if (data.clip is null) return;

            sound.SetData(data);
            sound.Play(pos, delay);
        }

        public void PlaySE(string clipName, Transform parent, float delay = 0.0f)
        {
            var sound = _seInsntaces.Find(n => n.isPlaying is false);
            if (sound is null) return;

            var data = _table._seList.Find(n => n.clip.name == clipName);
            if (data.clip is null) return;

            sound.SetData(data);
            sound.Play(parent, delay);
        }

        public void PlayBGM(string clipName)
        {
            var data = _table._bgmList.Find(n => n.clip.name == clipName);
            if (data.clip is null) return;

            var before = _bgmInstances[Convert.ToInt32( _bgmSwapped)].GetComponent<SoundMono>();
            var after  = _bgmInstances[Convert.ToInt32(!_bgmSwapped)].GetComponent<SoundMono>();

            _bgmSwapped = !_bgmSwapped;

            after.SetData(data);

            before.FadeOut(_table._bgmSwapTime, before.volume, 0.0f);
            after.FadeIn(_table._bgmSwapTime, 0.0f, data.volume);

            after.Play(Vector3.zero);

            before.Stop();
        }
    }
}
