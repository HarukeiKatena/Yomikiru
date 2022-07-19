using DG.Tweening;
using UnityEngine;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Light))]
    public class AttackLight : MonoBehaviour
    {
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private GameObject destroyOnComplete;

        void Start()
        {
            var tween = GetComponent<Light>().DOIntensity(0, duration).SetEase(Ease.InQuart).SetLink(gameObject);
            if(destroyOnComplete != null){
                Destroy(destroyOnComplete, duration);
            }
        }
    }
}
