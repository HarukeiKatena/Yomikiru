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
        // イベント（発行）
        private readonly Subject<Vector3> onPlayerAttack = new Subject<Vector3>();

        // イベント（講読）
        public IObservable<Vector3> OnPlayerAttack => onPlayerAttack;

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
            onPlayerAttack.OnNext(transform.position);
        }
    }
}
