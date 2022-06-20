using UnityEngine;
using UniRx;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    [RequireComponent(typeof(PlayerMove))]
    [RequireComponent(typeof(PlayerJump))]
    [RequireComponent(typeof(PlayerCamera))]
    public class Player : MonoBehaviour
    {
        // 内部コンポーネント
        private InputEvent inputEvent;
        private PlayerMove playerMove;
        private PlayerJump playerJump;
        private PlayerCamera playerCamera;

        private void Awake()
        {
            TryGetComponent(out inputEvent);
            TryGetComponent(out playerMove);
            TryGetComponent(out playerJump);
            TryGetComponent(out playerCamera);
        }

        private void Start()
        {
            inputEvent.OnJump.Subscribe(_ => playerJump.JumpStart());
        }
    }
}
