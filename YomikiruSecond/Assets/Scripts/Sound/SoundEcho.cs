using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SoundEcho")]
public class SoundEcho : ScriptableObject
{
    public struct SoundObject
    {
        public int PlayerIndex;
        public Vector3 Position;

        public SoundObject(int Index, Vector3 Posi)
        {
            PlayerIndex = Index;
            Position = Posi;
        }
    }

    public IObservable<SoundObject> Sound => sound;
    public IObservable<Unit> SoundStop => soundStop;

    private Subject<SoundObject> sound = new Subject<SoundObject>();
    private Subject<Unit> soundStop = new Subject<Unit>();

    public void RequestEcho(int PlayerIndex, Vector3 Position)
    {
        sound.OnNext(new SoundObject(Index: PlayerIndex, Posi: Position));
    }

    public void RequestStop()
    {
        soundStop.OnNext(Unit.Default);
    }
}
