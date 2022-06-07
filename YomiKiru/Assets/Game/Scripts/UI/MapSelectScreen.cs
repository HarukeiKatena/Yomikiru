using System;
using UnityEngine;
using UniRx;

namespace Yomikiru.UI
{
    
    public class MapSelectScreen : NewGameScreen
    {
        
        [SerializeField] private MapButton[] buttons;
        
        private readonly Subject<MapInfo> onMapSelect = new Subject<MapInfo>();

        public IObservable<MapInfo> OnMapSelect => onMapSelect;

        private void Start()
        {
            foreach (var btn in buttons)
            {
                btn.OnClick.AddListener(() => onMapSelect.OnNext(btn.Map));
            }
        }

    }

}
