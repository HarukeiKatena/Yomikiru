using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Yomikiru.Landmine
{
    public class LandmineLight : MonoBehaviour
    {

        [SerializeField] private float duration;
        [SerializeField] private float fadeDuration;
        [SerializeField] private Light light;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;

        private float lightIntensity;
        private bool isPlaying;

        private void Awake()
        {
            lightIntensity = light.intensity;
        }

        private void OnTriggerEnter(Collider other)
        {
            Character.Character chara;
            if (!other.TryGetComponent(out chara)) return;

            // chara でなんかしてもいいよ

            PlayAsync().Forget();
        }

        private async UniTaskVoid PlayAsync()
        {
            if (isPlaying) return;
            isPlaying = true;

            audioSource.PlayOneShot(audioClip);

            if(particles != null)
            {
                particles.Play();
            }

            light.enabled = true;
            await DOTween.Sequence()
                .Append(light.DOIntensity(lightIntensity, fadeDuration).From(0))
                .Append(light.DOIntensity(0, fadeDuration).SetDelay(duration));
            light.enabled = false;

            isPlaying = false;
        }

    }
}
