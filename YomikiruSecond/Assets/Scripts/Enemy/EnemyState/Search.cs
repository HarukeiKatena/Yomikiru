using System;
using UnityEngine;
using Yomikiru.Character.Enemy.StateMachine;
using UniRx;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class Search : EnemyState<Yomikiru.Character.Enemy.AIEnemyBase>
    {
        bool playerAttacking;

        IDisposable disposablePlayerAttack;
        public Search(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Search");
            enemy.Move.SetRandomDestination();
            enemy.Move.RestartAgent();
            playerAttacking = false;

            disposablePlayerAttack = enemy.PlayerAttack?.OnAttack.Subscribe(_ => playerAttacking = true);
        }

        public override void OnExit()
        {
            disposablePlayerAttack?.Dispose();
        }

        public override void Update()
        {
            if(enemy.Move.GetReachDestination())
            {
                enemy.StateMachine.CurrentState = enemy.AttackState;
            }
            else if(playerAttacking)
            {
                enemy.StateMachine.CurrentState = enemy.ChaseState;
            }
        }

        private void OnPlayerAttack()
        {
            
        }
    }

}
