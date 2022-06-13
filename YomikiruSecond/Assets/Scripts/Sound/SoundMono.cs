using UnityEngine;
using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Yomikiru.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundMono : MonoBehaviour
    {
        private AudioSource _source;

        public float volume
        {
            get { return _source.volume; }
        }

        public bool isPlaying
        {
            get { return _source.isPlaying; }
        }

        private void Awake() => TryGetComponent(out _source);

        public void SetData(SoundData data)
        {
            _source.clip = data.clip;
            _source.volume = data.volume;
            _source.pitch = data.pitch;
            _source.spatialBlend = data.spatialBlend;
            _source.loop = data.loop;
        }

        private async UniTask Play(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            Play();
            await UniTask.Delay((int)(_source.clip.length * 1000.0f), cancellationToken: ct);
            Stop();
        }
        private async UniTask PlayOnDelayed(float delay, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            PlayOnDelayed(delay);
            await Task.Delay((int)((_source.clip.length + delay) * 1000.0f), cancellationToken: ct);
            Stop();
        }

        public async void Play(Vector3 pos, float delay = 0.0f)
        {
            transform.position = pos;

            bool canceled;
            if (delay == 0.0f)
            {
                 canceled = await Play(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            }
            else
            {
                canceled = await PlayOnDelayed(delay, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            }
            if (canceled is true) return;

            transform.position = Vector3.zero;
        }
        public async void Play(Transform parent, float delay = 0.0f)
        {
            var before = transform.parent;
            transform.SetParent(parent, false);

            bool canceled;
            if (delay == 0.0f)
            {
                 canceled = await Play(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            }
            else
            {
                canceled = await PlayOnDelayed(delay, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            }
            if (canceled is true) return;
            
            transform.SetParent(before, false);
        }

        public void Play() => _source.Play();
        public void PlayOnDelayed(float delay) => _source.PlayDelayed(delay);
        public void PlayOneShot(AudioClip clip) => _source.PlayOneShot(clip);
        public void PlayScheduled(double time) => _source.PlayScheduled(time);
        public void Stop() => _source.Stop();
        public void Pause() => _source.Pause();
        public void UnPause() => _source.UnPause();

        public void FadeIn(float time, float endVolume = 1.0f)
        {
            FadeIn(time, _source.volume, endVolume);
        }
        public void FadeOut(float time, float endVolume = 0.0f)
        {
            FadeIn(time, _source.volume, endVolume);
        }
        public void FadeIn(float time, float startVolume, float endVolume)
        {
            _source.DOFade(endVolume, time).OnStart(() => {_source.volume = startVolume;}).SetLink(this.gameObject);
        }
        public void FadeOut(float time, float startVolume, float endVolume)
        {
            _source.DOFade(endVolume, time).OnStart(() => {_source.volume = startVolume;}).SetLink(this.gameObject);
        }
    }
}
