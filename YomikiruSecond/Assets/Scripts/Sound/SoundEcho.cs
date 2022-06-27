using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Yomikiru.Sound;

[CreateAssetMenu(menuName = "ScriptableObjects/SoundEcho")]
public class SoundEcho : ScriptableObject
{
    public struct SoundEchoObject
    {
        public int PlayerIndex;
        public SoundManager.SoundObject Sound;

        public SoundEchoObject(int index, SoundManager.SoundObject soundObject)
        {
            PlayerIndex = index;
            Sound = soundObject;
        }
    }

    public IObservable<SoundEchoObject> OnSoundSE => onSoundSe;
    public IObservable<SoundEchoObject> OnSoundBGM => onSoundBgm;
    public IObservable<Unit> SoundStop => soundStop;

    private Subject<SoundEchoObject> onSoundSe = new Subject<SoundEchoObject>();
    private Subject<SoundEchoObject> onSoundBgm = new Subject<SoundEchoObject>();
    private Subject<Unit> soundStop = new Subject<Unit>();

    //SEをリクエスト
    public void RequestSE(
        int PlayerIndex,
        string Name, Vector3 Position, float Delay = 0.0f)
    {
        onSoundSe.OnNext(new SoundEchoObject(
            PlayerIndex,
            new SoundManager.SoundObject(Name, Position, Delay)));
    }

    //BGMをリクエスト
    public void RequestBGM(int PlayerIndex, string Name)
    {
        onSoundBgm.OnNext(new SoundEchoObject(
            PlayerIndex,
            new SoundManager.SoundObject(Name)));
    }

    public void RequestStop()
    {
        soundStop.OnNext(Unit.Default);
    }
}
