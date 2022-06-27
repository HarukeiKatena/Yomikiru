using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yomikiru.Effect
{
    public class EffectManager : MonoBehaviour
    {
        // 外部パラメーター
        [Header("Data Table")]
        [SerializeField] private EffectTable _table = null;

        [Header("Object Pool")]
        [SerializeField] private Transform _effect = null;

        // 内部パラメーター
        private List<List<EffectMono>> _instanceList = new List<List<EffectMono>>();
        private bool _isInitialized = false;

        private void Start()
        {
            _isInitialized = false;

            foreach(var data in _table._effectList)
            {
                var _instances = new List<EffectMono>();
                _instanceList.Add(_instances);
                
                for (int j = 0; j < data.playMax; j++)
                {
                    var obj = Instantiate(data.prehub, _effect);
                    var effect = obj.GetComponent<EffectMono>();

                    obj.transform.position = data.prehub.transform.position;
                    effect.SetEffect(data);
                    _instances.Add(effect);
                }
            }

            _isInitialized = true;
        }

        private EffectMono FindEffect(string name)
        {
            var index = _table._effectList.FindIndex(n => n.name == name);
            if (index < 0 || index >= _instanceList.Count) return null;

            return _instanceList[index].Find(n => n.isPlaying == false);
        }

        public void Play(string name, Vector3 pos)
        {
            if (_isInitialized is false) return;

            var effect = FindEffect(name);
            if (effect is null) return;
            
            effect.Play(pos);
        }

        public void Play(string name, Transform parent)
        {
            if (_isInitialized is false) return;

            var effect = FindEffect(name);
            if (effect is null) return;
            
            effect.Play(parent);
        }
    }
}
