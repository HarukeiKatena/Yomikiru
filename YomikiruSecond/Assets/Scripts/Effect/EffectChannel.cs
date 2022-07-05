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
            public Vector3 Position;

            public EffectObject(string name, EffectClip clip, Vector3 pos)
            {
                CharacterName = name;
                Clip = clip;
                Position = pos;
            }
        }

        public IObservable<EffectObject> OnEffect => onEffect;

        private Subject<EffectObject> onEffect = new Subject<EffectObject>();

        public void Request(string characterName, EffectClip clip, Vector3 pos)
        {
            onEffect.OnNext(new EffectObject(characterName, clip, pos));
        }
    }
}
