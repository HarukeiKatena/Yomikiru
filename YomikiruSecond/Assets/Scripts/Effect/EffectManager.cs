using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Yomikiru.Effect
{
    public class EffectManager : MonoBehaviour
    {
        // 外部パラメーター
        [Header("Data Table")]
        [SerializeField] private EffectTable dataTable;

        [Header("Object Pool")]
        [SerializeField] private Transform objectPool;

        [Header("EffectEcho")]
        [SerializeField] private EffectEcho effectEcho;

        // 内部パラメーター
        private List<List<EffectMono>> instanceList = new List<List<EffectMono>>();
        private bool isInitialized = false;

        private void Start()
        {
            isInitialized = false;

            foreach(var data in dataTable._effectList)
            {
                var _instances = new List<EffectMono>();
                instanceList.Add(_instances);

                for (int j = 0; j < data.playMax; j++)
                {
                    var obj = Instantiate(data.prehub, objectPool);
                    var effect = obj.GetComponent<EffectMono>();

                    obj.transform.position = data.prehub.transform.position;
                    effect.SetEffect(data);
                    _instances.Add(effect);
                }
            }

            isInitialized = true;

            effectEcho.OnEcho.Subscribe(x => {
                Play(x.EffectName, x.Position);
            }).AddTo(this);
        }

        private EffectMono FindEffect(string Name)
        {
            var index = dataTable._effectList.FindIndex(n => n.name == Name);
            if (index < 0 || index >= instanceList.Count) return null;

            return instanceList[index].Find(n => n.isPlaying == false);
        }

        public void Play(string Name, Vector3 Position)
        {
            if (isInitialized is false) return;

            var effect = FindEffect(Name);
            if (effect is null) return;

            effect.Play(Position);
        }

        public void Play(string Name, Transform parent)
        {
            if (isInitialized is false) return;

            var effect = FindEffect(Name);
            if (effect is null) return;

            effect.Play(parent);
        }
    }
}
