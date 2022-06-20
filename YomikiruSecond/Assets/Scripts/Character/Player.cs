using UnityEngine;
using UniRx;
using Cinemachine;
using UniRx.Triggers;
using UnityEngine.Rendering;
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
            this.UpdateAsObservable().Subscribe(playerMove.MoveUpdate);
            this.UpdateAsObservable().Subscribe(playerCamera.CameraUpdate);
            this.FixedUpdateAsObservable().Subscribe(playerJump.FallFixedUpdate);

            inputEvent.OnJump.Subscribe(playerJump.JumpStart);
        }
    }
}