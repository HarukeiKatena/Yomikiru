using UnityEngine;
using UniRx;
using Yomikiru;

[CreateAssetMenu(fileName = "MatchInfo", menuName = "ScriptableObject/MatchInfo")]
public class MatchInfo : ScriptableObject
{

    public GamemodeInfo Gamemode;
    public MapInfo Map;
    public readonly ReactiveProperty<MatchState> State = new ReactiveProperty<MatchState>(MatchState.None);

    [Tooltip("勝者のインデックス 0～")]
    public int WinnerPlayerIndex = -1;

}
