using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UniRx;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Yomikiru.Effect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectEcho")]
    public class EffectEcho : ScriptableObject
    {
        public struct EchoObject
        {
            public string EffectName;
            public int PlayerIndex;
            public Vector3 Position;

            public EchoObject(string name, int index, Vector3 position)
            {
                EffectName = name;
                Position = position;
                PlayerIndex = index;
            }
        }

        public IObservable<EchoObject> OnEcho => onEcho;
        public IObservable<Unit> OnEchoStop => onEchoStop;

        private readonly Subject<EchoObject> onEcho = new Subject<EchoObject>();
        private readonly Subject<Unit> onEchoStop = new Subject<Unit>();

        public void Request(int PlayerIndex, string EffectName, Vector3 Position)
        {
            onEcho.OnNext(new EchoObject(EffectName, PlayerIndex, Position));
        }

        void RequestStop()
        {
            onEchoStop.OnNext(Unit.Default);
        }

    }
}
