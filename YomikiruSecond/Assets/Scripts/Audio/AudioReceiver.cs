using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Yomikiru.Audio
{
    public sealed class AudioReceiver : MonoBehaviour
    {

        [SerializeField] private AudioChannel channel;
        [SerializeField] private bool allowLoop;
        [SerializeField] private int sourceCount;
        [SerializeField] private AudioSource sourcePrefab;

        private readonly List<AudioSource> sources = new List<AudioSource>();

        private void Awake()
        {
            sources.Clear();
            for (int i = 0; i < sourceCount; i++)
            {
                sources.Add(Instantiate(sourcePrefab, transform));
            }
            channel.OnRequest.Subscribe(cue =>
            {
                var source = FindIdleSource();
                if(!source) return;
                // 3Dサウンドはここをいじれ
                source.clip = cue.Clip;
                source.loop = allowLoop && cue.Loop;
                source.Play();
            });
            channel.OnStopRequest.Subscribe(_ => { StopAll(); });
        }

        private AudioSource FindIdleSource()
        {
            return sources.Find(source => !source.isPlaying);
        }

        private void StopAll()
        {
            sources.ForEach(source => source.Stop());
        }

    }
}
