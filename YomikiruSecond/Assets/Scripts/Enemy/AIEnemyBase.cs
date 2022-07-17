using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UniRx;
using Yomikiru.Characte.Management;
using Yomikiru.Character.Enemy.StateMachine;

namespace Yomikiru.Character.Enemy
{
    [RequireComponent(typeof(EnemyMove))]
    public class AIEnemyBase : MonoBehaviour
    {
        [SerializeField] private GameObject deathPrefab;

        // state
        public State.None NoneState { get; private set; }
        public State.Search SearchState { get; private set; }
        public State.Attack AttackState { get; private set; }
        public State.Chase ChaseState { get; private set; }
        public EnemyStateMachine<AIEnemyBase> StateMachine { get; private set; } = new EnemyStateMachine<AIEnemyBase>();
        
        // parameter
        [SerializeField] private MatchInfo matchInfo;

        //player info
        private PlayerAttack playerAttack;
        public PlayerAttack PlayerAttack { get => playerAttack; set => playerAttack = value; }

        //enemy info
        private PlayerAttack attack;
        public PlayerAttack Attack { get => attack; set => attack = value; }
        private EnemyMove move;
        public EnemyMove Move { get => move; set => move = value; }

        [SerializeField] private float attackTime;
        public float AttackTime { get => attackTime; }
        [SerializeField] private float chaseLimitTime;
        public float ChaseLimitTime { get => chaseLimitTime; }
        [SerializeField] private float wattingTimeOnPlayerLost;
        public float WattingTimeOnPlayerLost { get => wattingTimeOnPlayerLost; }

        //前回あったけどいるかわからない
        private Subject<int> dieEvent = new Subject<int>();
        public IObservable<int> DieEvent => dieEvent;

        public CancellationTokenSource Cts { get; private set; }

        public AIEnemyBase()
        {
            NoneState = new State.None(this);
            SearchState = new State.Search(this);
            AttackState = new State.Attack(this);
            ChaseState = new State.Chase(this);

            Cts = new CancellationTokenSource();
        }

        private void Awake()
        {
            StateMachine.CurrentState = NoneState;
            matchInfo.OnStateChange.Subscribe(_ => StartGame());

            TryGetComponent<PlayerAttack>(out attack);
            TryGetComponent<EnemyMove>(out move);
        }
        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            StateMachine.Update(); 
        }

        private void StartGame()
        {
            if(matchInfo.Gamemode.Gamemode == Gamemode.SOLO && matchInfo.State == MatchState.Ingame)
            {
                StateMachine.CurrentState = SearchState;
            }
        }

        public void Die()
        {
            var effect = Instantiate(deathPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Destroy(effect, 1.5f);

            dieEvent.OnNext(3);
            dieEvent.OnCompleted();
        }
    }
}