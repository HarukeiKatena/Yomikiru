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
            inputEvent.OnMove.Subscribe(move.OnMove);
            inputEvent.OnSprint.Subscribe(move.OnSprint);
            inputEvent.OnLook.Subscribe(camera.OnLook);
            inputEvent.OnJump.Subscribe(_ => move.OnJump());
            inputEvent.OnAttack.Subscribe(_ => attack.OnAttack());
        }
    }
}