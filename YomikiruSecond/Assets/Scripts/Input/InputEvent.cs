using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yomikiru.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputEvent : MonoBehaviour
    {
        // イベント (発行)
        private readonly Subject<Vector2> onMove = new Subject<Vector2>();
        private readonly Subject<Vector2> onLook = new Subject<Vector2>();
        private readonly Subject<Unit> onFire = new Subject<Unit>();
        private readonly Subject<Unit> onJump = new Subject<Unit>();
        private readonly Subject<bool> onDash = new Subject<bool>();

        // イベント (購読)
        public IObservable<Vector2> OnMove => onMove;
        public IObservable<Vector2> OnLook => onLook;
        public IObservable<Unit> OnFire => onFire;
        public IObservable<Unit> OnJump => onJump;
        public IObservable<bool> OnDash => onDash;

        // 内部コンポーネント
        private PlayerInput playerInput;

        private void Awake() => TryGetComponent(out playerInput);

        private void OnEnable()
        {
            playerInput.actions["Move"].performed += OnMoveChange;
            playerInput.actions["Move"].canceled += OnMoveStop;
            playerInput.actions["Look"].performed += OnLookChange;
            playerInput.actions["Look"].canceled += OnLookStop;
            playerInput.actions["Fire"].started += OnFireStart;
            playerInput.actions["Jump"].started += OnJumpStart;
            playerInput.actions["Dash"].started += OnDashChange;
            playerInput.actions["Dash"].canceled += OnDashChange;
        }

        private void OnDisable()
        {
            playerInput.actions["Move"].performed -= OnMoveChange;
            playerInput.actions["Move"].canceled -= OnMoveStop;
            playerInput.actions["Look"].performed -= OnLookChange;
            playerInput.actions["Look"].canceled -= OnLookStop;
            playerInput.actions["Fire"].started -= OnFireStart;
            playerInput.actions["Jump"].started -= OnJumpStart;
            playerInput.actions["Dash"].started -= OnDashChange;
            playerInput.actions["Dash"].canceled -= OnDashChange;
        }

        private void OnMoveChange(InputAction.CallbackContext context)
            => onMove.OnNext(context.ReadValue<Vector2>());

        private void OnMoveStop(InputAction.CallbackContext context)
            => onMove.OnNext(Vector2.zero);

        private void OnLookChange(InputAction.CallbackContext context)
            => onLook.OnNext(context.ReadValue<Vector2>());

        private void OnLookStop(InputAction.CallbackContext context)
            => onLook.OnNext(Vector2.zero);

        private void OnFireStart(InputAction.CallbackContext context)
            => onFire.OnNext(Unit.Default);

        private void OnJumpStart(InputAction.CallbackContext context)
            => onJump.OnNext(Unit.Default);

        private void OnDashChange(InputAction.CallbackContext context)
            => onDash.OnNext(context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);

    }
}
