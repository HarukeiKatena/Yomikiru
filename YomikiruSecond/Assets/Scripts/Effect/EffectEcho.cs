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
            public int PlayerIndex;
            public Vector3 Position;

            public EchoObject(int Index, Vector3 Posi)
            {
                Position = Posi;
                PlayerIndex = Index;
            }
        }

        public IObservable<EchoObject> Echo => echo;
        public IObservable<Unit> EchoStop => echoStop;

        private readonly Subject<EchoObject> echo = new Subject<EchoObject>();
        private readonly Subject<Unit> echoStop = new Subject<Unit>();

        void RequestEchoAndPlay(Vector3 Position, int PlayerIndex)
        {
            echo.OnNext(new EchoObject(Index: PlayerIndex, Posi: Position));
        }

        void RequestStop()
        {
            echoStop.OnNext(Unit.Default);
        }

    }
}
