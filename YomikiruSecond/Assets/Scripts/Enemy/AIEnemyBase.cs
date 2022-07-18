using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UniRx;
using Yomikiru.Characte.Management;
using Yomikiru.Character.Enemy.StateMachine;
using Yomikiru.Character.Enemy.State;

namespace Yomikiru.Character.Enemy
{
    [RequireComponent(typeof(EnemyMove))]
    public class AIEnemyBase : MonoBehaviour
    {
        // state
        public EnemyStateMachine<AIEnemyBase> StateMachine { get; private set; } = new EnemyStateMachine<AIEnemyBase>();
        
        // parameter
        [SerializeField] private MatchInfo matchInfo;

        // player info
        private CharacterManagement characterManagement;
        public CharacterManagement CharacterManagement
        { 
            get => characterManagement;
            set 
            { 
                characterManagement = value;
                playerAttack = characterManagement.GetCharacterObject(this.gameObject).GetComponent<PlayerAttack>();
            }
        }
        private PlayerAttack playerAttack;
        public PlayerAttack PlayerAttack => playerAttack;

        // enemy info
        public PlayerAttack Attack { get; private set; }
        public EnemyMove Move { get; private set; }

        [SerializeField] private float attackTime;
        public float AttackTime { get => attackTime; }
        [SerializeField] private float chaseLimitTime;
        public float ChaseLimitTime { get => chaseLimitTime; }
        [SerializeField] private float wattingTimeOnPlayerLost;
        public float WattingTimeOnPlayerLost { get => wattingTimeOnPlayerLost; }

        private void Awake()
        {
            StateMachine.CurrentState = new EnemyStateNone(this);
            matchInfo.OnStateChange.Subscribe(state => OnMatchStateChange(state));

            Attack = GetComponent<PlayerAttack>();
            Move = GetComponent<EnemyMove>();
        }

        private void Update()
        {
            StateMachine.Update(); 
        }

        private void OnMatchStateChange(MatchState state)
        {
            if(state == MatchState.Ingame) return;
            if(matchInfo.Gamemode.Gamemode == Gamemode.SOLO)
            {
                StateMachine.CurrentState = new EnemyStateSearch(this);
            }
        }
    }
}