using System;
using UnityEngine;
using UniRx;

namespace Yomikiru.UI
{
    
    public class GamemodeSelectScreen : NewGameScreen
    {
        
        [SerializeField] private GamemodeButton[] buttons;
        
        private readonly Subject<GamemodeInfo> onGamemodeSelect = new Subject<GamemodeInfo>();

        public IObservable<GamemodeInfo> OnGamemodeSelect => onGamemodeSelect;

        private void Start()
        {
            foreach (var btn in buttons)
            {
                btn.OnClick.AddListener(() => onGamemodeSelect.OnNext(btn.Gamemode));
            }
        }

    }

}
