using UnityEngine;
using Yomikiru.Input;
using UniRx;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    [RequireComponent(typeof(PlayerMove))]
    [RequireComponent(typeof(PlayerAttack))]
    [RequireComponent(typeof(PlayerCamera))]
    public class Player : MonoBehaviour
    {
        private Character character;
        private InputEvent inputEvent;
        private PlayerMove move;
        private PlayerAttack attack;
        private PlayerCamera camera;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out inputEvent);
            TryGetComponent(out move);
            TryGetComponent(out attack);
            TryGetComponent(out camera);
        }

        private void Start()
        {
            inputEvent.OnMove.Subscribe(move.OnMoveInput);
            inputEvent.OnSprint.Subscribe(move.OnSprintInput);
            inputEvent.OnLook.Subscribe(camera.OnLook);
            inputEvent.OnJump.Subscribe(_ => move.OnJumpInput());
            inputEvent.OnAttack.Subscribe(_ => attack.Attack());
        }
    }
}