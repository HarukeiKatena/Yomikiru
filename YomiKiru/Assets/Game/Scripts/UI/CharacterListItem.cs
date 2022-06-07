using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    [RequireComponent(typeof(Button))]
    public class CharacterListItem : MonoBehaviour
    {

        [field: SerializeField] public CharacterInfo Character;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI labelName;

        public Button.ButtonClickedEvent OnClick => button.onClick;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            DisplayCharacterInfo();
        }

        private void DisplayCharacterInfo()
        {
            if (Character == null) return;
            if (labelName != null) labelName.text = Character.Name;
        }

    }

}
