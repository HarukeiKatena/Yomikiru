using System;
using UnityEngine;
using UniRx;

namespace Yomikiru.Audio
{
    [CreateAssetMenu(fileName = "New Audio Channel", menuName = "ScriptableObject/Audio/AudioChannel")]
    public sealed class AudioChannel : ScriptableObject
    {

        private readonly Subject<AudioCue> onRequest = new Subject<AudioCue>();
        private readonly Subject<Unit> onStopRequest = new Subject<Unit>();

        public IObservable<AudioCue> OnRequest => onRequest;
        public IObservable<Unit> OnStopRequest => onStopRequest;

        public void Request(AudioCue cue)
        {
            onRequest.OnNext(cue);
        }

        public void RequestStop()
        {
            onStopRequest.OnNext(Unit.Default);
        }

    }
}
