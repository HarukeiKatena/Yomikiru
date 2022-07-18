using System;
using UnityEngine;
using Yomikiru.Character.Enemy.StateMachine;
using UniRx;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class EnemyStateSearch : EnemyState<Yomikiru.Character.Enemy.AIEnemyBase>
    {
        IDisposable disposablePlayerAttack;
        public EnemyStateSearch(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Search");
            enemy.Move.SetRandomDestination();
            enemy.Move.RestartAgent();

            if(enemy.PlayerAttack is object) disposablePlayerAttack = enemy.PlayerAttack.OnAttack.Subscribe(_ => enemy.StateMachine.CurrentState = new EnemyStateChase(enemy));
        }

        public override void OnExit()
        {
            disposablePlayerAttack?.Dispose();
        }

        public override void Update()
        {
            if(enemy.Move.GetReachDestination())
            {
                enemy.StateMachine.CurrentState =  new EnemyStateAttack(enemy);
            }
        }
    }

}
