using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UniRx;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Yomikiru.Effect.Echo
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectEcho")]
    public class EffectEcho : ScriptableObject
    {
        public struct EchoObje
        {
            public int PlayerIndex;
            public Vector3 Position;

            public EchoObje(int Index, Vector3 Posi)
            {
                Position = Posi;
                PlayerIndex = Index;
            }
        }

        public EffectManager Effect;
        public IObservable<EchoObje> Echo => echo;
        public IObservable<Unit> EchoStop => echoStop;

        private readonly Subject<EchoObje> echo = new Subject<EchoObje>();
        private readonly Subject<Unit> echoStop = new Subject<Unit>();

        void RequestEchoAndPlay(string Name, Vector3 Posi, int PlayerIndex)
        {
            Effect.Play(Name, Posi);
            echo.OnNext(new EchoObje(Index: PlayerIndex, Posi: Posi));
        }

        void RequestStop()
        {
            echoStop.OnNext(Unit.Default);
        }

    }
}
