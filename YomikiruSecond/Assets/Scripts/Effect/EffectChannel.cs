using System;
using UniRx;
using UnityEngine;

namespace Yomikiru.Effect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectChannel")]
    public class EffectChannel : ScriptableObject
    {
        public struct EffectObject
        {
            public string CharacterName;
            public EffectClip Clip;

            public EffectObject(string name, EffectClip clip)
            {
                CharacterName = name;
                Clip = clip;
            }
        }

        public IObservable<EffectObject> OnEffect => onEffect;

        private Subject<EffectObject> onEffect = new Subject<EffectObject>();

        public void Request(string characterName, EffectClip clip)
        {
            onEffect.OnNext(new EffectObject(characterName, clip));
        }
    }
}
