using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    public class PlayerJoin : MonoBehaviour
    {

        public enum State
        {
            IDLE,
            AWAITING_INPUT,
            JOINED,
        }

        [SerializeField] private string playerName;
        [SerializeField] private TextMeshProUGUI labelPlayerName;
        [SerializeField] private TextMeshProUGUI labelAwaitingInput;
        [SerializeField] private GameObject joined;
        [SerializeField] private Image deviceIcon;
        [SerializeField] private TextMeshProUGUI labelDeviceName;
        [SerializeField] private TextMeshProUGUI labelLeave;
        [SerializeField] private string textAwaitingInput;
        [SerializeField] private string textAwaitingInputKeyboard;
        [SerializeField] private string textLeave;
        [SerializeField] private string textLeaveKeyboard;
        [SerializeField] private Sprite spriteGamepad;
        [SerializeField] private Sprite spriteKeyboard;

        public string PlayerName
        {
            set
            {
                playerName = value;
                labelPlayerName.text = value;
            }
        }

        private void Awake()
        {
            // 名前を表示
            PlayerName = playerName;
        }

        public void DisplayAwaitingInput(bool showKeyboard)
        {
            labelAwaitingInput.text = showKeyboard ? textAwaitingInputKeyboard : textAwaitingInput;
            labelAwaitingInput.gameObject.SetActive(true);
            joined.SetActive(false);
        }

        public void DisplayJoined(InputDevice device, bool displayAsKeyboard)
        {
            labelDeviceName.text = displayAsKeyboard ? "キーボード" : device.displayName;
            deviceIcon.sprite = displayAsKeyboard ? spriteKeyboard : spriteGamepad;
            labelLeave.text = displayAsKeyboard ? textLeaveKeyboard : textLeave;
            labelAwaitingInput.gameObject.SetActive(false);
            joined.SetActive(true);
        }

    }

}
