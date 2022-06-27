using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Toolkit;
using UnityEngine;
using Yomikiru.Effect;

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
                effect.Play(x.Clip);
                effect.OnStopEffect.First().Subscribe(_ => pool.Return(effect));//
            }).AddTo(this);
        }
    }
}
