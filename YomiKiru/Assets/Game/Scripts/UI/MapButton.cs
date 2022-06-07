using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    [RequireComponent(typeof(Button))]
    public class MapButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {

        [SerializeField] private MapInfo map;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI labelName;
        [SerializeField] private Image banner;
        [SerializeField] private AspectRatioFitter bannerFitter;

        public MapInfo Map
        {
            get => map;
            set
            {
                map = value;
                DisplayMapInfo();
            }
        }

        public Button.ButtonClickedEvent OnClick => button.onClick;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            DisplayMapInfo();
        }

        private void DisplayMapInfo()
        {
            if (Map == null) return;
            if (labelName != null)    labelName.text = Map.Name;
            if (banner != null)       banner.sprite  = Map.Banner;
            if (bannerFitter != null) bannerFitter.aspectRatio = Map.Banner.rect.width / Map.Banner.rect.height;
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
