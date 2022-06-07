using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    [RequireComponent(typeof(Button))]
    public class GamemodeButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {

        [field: SerializeField] public GamemodeInfo Gamemode { get; private set; }

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI labelName;
        [SerializeField] private TextMeshProUGUI labelDescription;
        [SerializeField] private Image banner;

        public Button.ButtonClickedEvent OnClick => button.onClick;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            DisplayGamemodeInfo();
        }

        private void DisplayGamemodeInfo()
        {
            if (Gamemode == null) return;
            if (labelName != null)        labelName.text        = Gamemode.Name;
            if (labelDescription != null) labelDescription.text = Gamemode.Description;
            if (banner != null)           banner.sprite         = Gamemode.Banner;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutCubic);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutCubic);
        }

    }

}
