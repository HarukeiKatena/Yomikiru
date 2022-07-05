using UniRx;
using UnityEngine;

namespace Yomikiru.Effect
{
    public class EffectManagement : MonoBehaviour
    {
        [Header("EffectSetting")]
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private Transform objectPoolParent;

        [Header("Channel")]
        [SerializeField] private EffectChannel effectChannel;

        private EffectPool pool;

        private void Start()
        {
            pool = new EffectPool(effectPrefab, objectPoolParent);

            effectChannel.OnEffect.Subscribe(x =>
            {
                var effect = pool.Rent();
                effect.Play(x.Clip, x.Position);
                effect.OnStopEffect.First().Subscribe(_ => pool.Return(effect)).AddTo(this);
            }).AddTo(this);
        }
    }
}
