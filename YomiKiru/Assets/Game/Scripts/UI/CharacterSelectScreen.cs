using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

namespace Yomikiru.UI
{

    public class CharacterSelectScreen : NewGameScreen
    {

        [SerializeField] private CharacterListItem[] listItems;
        
        private readonly Subject<CharacterInfo> onCharacterSelect = new Subject<CharacterInfo>();

        public IObservable<CharacterInfo> OnCharacterSelect => onCharacterSelect;

        private void Start()
        {
            foreach (var listItem in listItems)
            {
                listItem.OnClick.AddListener(() => onCharacterSelect.OnNext(listItem.Character));
            }
        }

    }

}
