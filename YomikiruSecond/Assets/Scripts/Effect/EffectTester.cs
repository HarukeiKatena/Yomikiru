using UnityEngine;

namespace Yomikiru.Effect
{
    public class EffectTester : MonoBehaviour
    {
        [SerializeField] private EffectEcho effect = null;
        [SerializeField] private string effectName = null;
        [SerializeField] private float duration = 0.0f;

        private float time = 0.0f;

        private void Start()
        {
            effect.Request(0, "", this.transform.position);
            time = Time.time;
        }

        private void Update()
        {
            if (time + duration < Time.time)
            {
                effect.Request(0, "", this.transform.position);
                time = Time.time;
            }
        }
    }
}
