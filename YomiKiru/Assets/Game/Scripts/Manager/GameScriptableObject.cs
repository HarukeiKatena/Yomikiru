using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yomikiru;

[CreateAssetMenu(fileName = "GameManager", menuName = "Property/GameManager")]
public class GameScriptableObject : ScriptableObject
{
    [Header("ゲーム処理部分")]
    public GamemodeInfo Gamemode;
    public MapInfo Map;
    [Tooltip("勝者のインデックス 0～")]
    public int WinnerPlayerIndex = -1;
}
