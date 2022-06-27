using System;
using UniRx;
using UnityEngine;

namespace Yomikiru.Effect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectEcho")]
    public class EffectEcho : ScriptableObject
    {
        public struct EffectObject
        {
            public string PlayerName;
        }

        public IObservable<EffectObject> OnEffect => onEffect;

        private Subject<EffectObject> onEffect = new Subject<EffectObject>();

        void Request()
        {

        }

        void RequestStop()
        {

        }
    }
}
