using System;
using UnityEngine;
using UniRx;

namespace Yomikiru{

    [CreateAssetMenu(fileName = "MatchInfo", menuName = "ScriptableObject/MatchInfo")]
    public class MatchInfo : ScriptableObject
    {

        private readonly ReactiveProperty<MatchState> state = new ReactiveProperty<MatchState>(MatchState.None);

        public GamemodeInfo Gamemode { get; set; }
        public MapInfo Map { get; set; }
        public MatchState State
        {
            get => state.Value;
            set => state.Value = value;
        }

        public int WinnerPlayerIndex { get; set; } = -1;

        public IObservable<MatchState> OnStateChange => state;

    }

}
