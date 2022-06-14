using System;
using UnityEngine;
using UniRx;
using Yomikiru;

[CreateAssetMenu(fileName = "MatchInfo", menuName = "ScriptableObject/MatchInfo")]
public class MatchInfo : ScriptableObject
{

    private readonly ReactiveProperty<MatchState> state = new ReactiveProperty<MatchState>(MatchState.None);

    public GamemodeInfo Gamemode;
    public MapInfo Map;
    public MatchState State
    {
        get => state.Value;
        set => state.Value = value;
    }

    [Tooltip("勝者のインデックス 0～")]
    public int WinnerPlayerIndex = -1;

    public IObservable<MatchState> OnStateChange => state;

}
