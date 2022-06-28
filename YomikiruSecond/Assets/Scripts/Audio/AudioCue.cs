using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yomikiru.Audio
{
    [CreateAssetMenu(fileName = "New Audio Cue", menuName = "ScriptableObject/Audio/AudioCue")]
    public sealed class AudioCue : ScriptableObject
    {

        [SerializeField] private bool loop;
        [SerializeField] private AudioClip clip;

        public bool Loop => loop;
        public AudioClip Clip => clip;

    }
}
