using System.Collections.Generic;
using UnityEngine;

namespace Yomikiru.Sound
{
    [System.Serializable]
    public struct SoundData
    {
        public AudioClip clip;
        [Range(0.0f, 1.0f)] public float volume;
        [Range(0.0f, 2.0f)] public float pitch;
        [Range(0.0f, 1.0f)] public float spatialBlend;
        public bool loop;
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundTable")]
    public class SoundTable : ScriptableObject
    {
        [Header("Prehub")]
        [SerializeField] public GameObject      _seTemplate     = null;
        [SerializeField] public GameObject      _bgmTemplate    = null;

        [Header("Parameter")]
        [SerializeField] public uint            _sePlayMax      = 0;
        [SerializeField] public float           _bgmSwapTime    = 1.0f;

        [Header("Data")]
        [SerializeField] public List<SoundData> _seList         = new List<SoundData>();
        [SerializeField] public List<SoundData> _bgmList        = new List<SoundData>();
    }
}
