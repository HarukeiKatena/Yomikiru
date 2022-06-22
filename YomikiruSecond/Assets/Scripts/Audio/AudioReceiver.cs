using UnityEngine;
using UniRx;

namespace Yomikiru.Audio
{
    public sealed class AudioReceiver : MonoBehaviour
    {

        [SerializeField] private AudioChannel channel;
        [SerializeField] private bool ignoreSameCue;
        [SerializeField] private bool allowLoop;
        [SerializeField] private AudioSource source;

        private void Awake()
        {
            channel.OnRequest.Subscribe(cue =>
            {
                if (ignoreSameCue && source.clip == cue.Clip) return;
                source.clip = cue.Clip;
                source.loop = allowLoop && cue.Loop;
                source.Play();
            });
            channel.OnStopRequest.Subscribe(cue => { source.Stop(); });
        }

    }
}
