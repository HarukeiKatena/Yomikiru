using UnityEngine;
using Yomikiru.Sound;

public class SoundTester : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager = null;
    [SerializeField] private AudioClip clip = null;
    [SerializeField] private float duration = 0.0f;

    private float time = 0.0f;

    private void Start()
    {
        soundManager.PlaySE(clip.name, this.transform);
        time = Time.time;
    }

    private void Update()
    {
        if (time + duration < Time.time)
        {
            soundManager.PlaySE(clip.name, this.transform);
            time = Time.time;
        }
    }
}
