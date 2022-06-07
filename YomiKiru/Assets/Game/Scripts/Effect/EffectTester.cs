using UnityEngine;

namespace Yomikiru.Effect
{
    public class EffectTester : MonoBehaviour
    {
        [SerializeField] private EffectManager manager = null;
        [SerializeField] private string effectName = null;
        [SerializeField] private float duration = 0.0f;

        private float time = 0.0f;

        private void Start()
        {
            manager.Play(effectName, transform.position + Vector3.up * 0.1f);
            time = Time.time;
        }

        private void Update()
        {
            if (time + duration < Time.time)
            {
                manager.Play(effectName, transform.position + Vector3.up * 0.1f);
                time = Time.time;
            }
        }
    }
}
