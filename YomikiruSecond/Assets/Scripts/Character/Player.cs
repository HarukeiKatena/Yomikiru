using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
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
        // 公開パラメータ
        public bool DebugFlag = false;

        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private InputEvent inputEvent;
        private PlayerMove move;
        private PlayerAttack attack;
        private PlayerCamera camera;
        private List<IDisposable> eventList = new List<IDisposable>();

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
            table = character.Table;

            table.Match.OnStateChange.Subscribe(state =>
            {
                switch (state)
                {
                    case MatchState.Intro:
                        //DisableEvents();
                        break;
                    case MatchState.Ingame:
                        EnableEvents();
                        break;
                    case MatchState.Finished:
                        DisableEvents();
                        break;
                    default:
                        break;
                }
            });

            if (DebugFlag)
            {
                EnableEvents();
            }
        }

        public void EnableEvents()
        {
            if (eventList.Count != 0) return;

            eventList.Add(inputEvent.OnMove.Subscribe(move.OnMoveInput));
            eventList.Add(inputEvent.OnSprint.Subscribe(move.OnSprintInput));
            eventList.Add(inputEvent.OnLook.Subscribe(camera.OnLookInput));
            eventList.Add(inputEvent.OnJump.Subscribe(_ => move.OnJumpInput()));
            eventList.Add(inputEvent.OnAttack.Subscribe(_ => attack.OnAttack()));
        }

        public void DisableEvents()
        {
            if (eventList.Count == 0) return;

            foreach (var e in eventList)
            {
                e.Dispose();
            }

            eventList.Clear();

            move.OnMoveInput(Vector2.zero);
            camera.OnLookInput(Vector2.zero);
        }

        public void Die()
        {
            table.Match.State = MatchState.Finished;
        }
    }
}