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
        [SerializeField] private MatchInfo matchInfo;
        [SerializeField] private float attackTime;
        [SerializeField] private float chaseLimitTime;
        [SerializeField] private float waitingTimeOnPlayerLost;

        // player info
        private CharacterManagement characterManagement;
        public CharacterManagement CharacterManagement
        { 
            get => characterManagement;
            set 
            { 
                characterManagement = value;
                playerAttack = characterManagement.GetCharacterObject(gameObject).GetComponent<PlayerAttack>();
            }
        }
        private PlayerAttack playerAttack;
        public PlayerAttack PlayerAttack => playerAttack;

        public PlayerAttack Attack { get; private set; }
        public EnemyMove Move { get; private set; }

        public float AttackTime => attackTime;
        public float ChaseLimitTime => chaseLimitTime;
        public float WaitingTimeOnPlayerLost => waitingTimeOnPlayerLost;

        public EnemyStateMachine<AIEnemyBase> StateMachine { get; private set; } = new EnemyStateMachine<AIEnemyBase>();

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