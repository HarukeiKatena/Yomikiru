using System.Collections;
using System.Collections.Generic;
using System;
using Player;
using UnityEngine;
using UniRx;
using Yomikiru.Input;

namespace Yomikiru.Character
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(InputEvent))]
    public class PlayerAttack : MonoBehaviour
    {
        // 内部コンポーネント
        private Character character;
        private CharacterData table;
        private InputEvent inputEvent;

        private void Awake()
        {
            TryGetComponent(out character);
            TryGetComponent(out inputEvent);
        }

        private void Start()
        {
            table = character.Table;

            inputEvent.OnAttack.Subscribe(_ => AttackStart());
        }

        public void AttackStart()
        {

        }
    }
}
