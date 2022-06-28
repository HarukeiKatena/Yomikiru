using UniRx.Toolkit;
using UnityEngine;

namespace Yomikiru.Effect
{
    public class EffectPool : ObjectPool<EffectOperation>
    {
        private GameObject prefab;
        private Transform parent;

        public EffectPool(GameObject effectPrefab, Transform ObjectParent)
        {
            prefab = effectPrefab;
            parent = ObjectParent;
        }

        protected override EffectOperation CreateInstance()
        {
            return GameObject.Instantiate(prefab, parent, true).GetComponent<EffectOperation>();
        }
    }
}
