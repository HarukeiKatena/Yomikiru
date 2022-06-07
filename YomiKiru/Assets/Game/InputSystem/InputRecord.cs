using Player;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Enemy;
public class InputRecord : MonoBehaviour
{
    [Header("キャラクター移動")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool attack;
    public bool escape;

    public IReadOnlyReactiveProperty<Vector2> Move => _move;
    public IReadOnlyReactiveProperty<Vector2> Look => _look;
    public IReadOnlyReactiveProperty<bool> Attack => _attack;
    public IReadOnlyReactiveProperty<bool> Jump => _jump;
    public IReadOnlyReactiveProperty<bool> Sprint => _sprint;
    public IReadOnlyReactiveProperty<bool> Escape => _escape;

    private readonly ReactiveProperty<Vector2> _move = new ReactiveProperty<Vector2>();
    private readonly ReactiveProperty<Vector2> _look = new ReactiveProperty<Vector2>();
    private readonly ReactiveProperty<bool> _attack = new BoolReactiveProperty();
    private readonly ReactiveProperty<bool> _jump = new BoolReactiveProperty();
    private readonly ReactiveProperty<bool> _sprint = new BoolReactiveProperty();
    private readonly ReactiveProperty<bool> _escape = new BoolReactiveProperty();

    private bool _IsInput = false;//入力状態

    private void Start()
    {
        _move.AddTo(this);
        _look.AddTo(this);
        _attack.AddTo(this);
        _jump.AddTo(this);
        _escape.AddTo(this);

        gameObject.GetComponent<PlayerPropSetting>().intro.IntroIvent.
            Where(x => x == IntroSequence.INTRO_END).
            Subscribe(_ =>
            {
                _IsInput = true;
                gameObject.GetComponent<CharacterController>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
            }).AddTo(this);
        if(TryGetComponent<Die>(out var characterDie)){
            characterDie.DieIvent.Subscribe(x => Cursor.lockState = CursorLockMode.None).AddTo(this);
        }
        else if(TryGetComponent<AIEnemyBase>(out var enemy)) {
            enemy.DieEvent.Subscribe(x => Cursor.lockState = CursorLockMode.None).AddTo(this);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpInput(context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintInput(context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackInput(context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        EscapeInput(context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);
    }


    //入力処理
    public void MoveInput(Vector2 newMoveDirection)
    {
        if(!_IsInput)
            return;

        move = newMoveDirection;
        _move.Value = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        if (!_IsInput)
            return;

        look = newLookDirection;
        _look.Value = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        if (!_IsInput)
            return;

        jump = newJumpState;
        _jump.Value = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        if (!_IsInput)
            return;

        sprint = newSprintState;
        _sprint.Value = newSprintState;
    }

    public void AttackInput(bool newAttackState)
    {
        if (!_IsInput)
            return;

        attack = newAttackState;
        _attack.Value = newAttackState;
    }

    public void EscapeInput(bool newEscapeState)
    {
        if (!_IsInput)
            return;

        escape = newEscapeState;
        _escape.Value = newEscapeState;
    }

}