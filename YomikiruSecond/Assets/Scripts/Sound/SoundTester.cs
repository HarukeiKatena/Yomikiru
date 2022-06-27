using UnityEngine;
using Yomikiru.Sound;

public class SoundTester : MonoBehaviour
{
    [SerializeField] private SoundEcho sound = null;
    [SerializeField] private AudioClip clip = null;
    [SerializeField] private float duration = 0.0f;

    private float time = 0.0f;

    private void Start()
    {
        sound.RequestSE(0, "clip.name", this.transform.position);
        time = Time.time;
    }

    private void Update()
    {
        if (time + duration < Time.time)
        {
            sound.RequestSE(0, "clip.name", this.transform.position);
            time = Time.time;
        }
    }
}
