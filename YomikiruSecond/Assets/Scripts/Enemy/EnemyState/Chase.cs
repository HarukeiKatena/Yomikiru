using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Yomikiru.Character.Enemy.StateMachine;
using UniRx;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class EnemyStateChase : EnemyState<AIEnemyBase>
    {
        private bool isPlayerAttacked;
        private IDisposable disposablePlayerAttack;
        private float chaseTime;
        public EnemyStateChase(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            chaseTime = 0;
            isPlayerAttacked = false;

            if(enemy.PlayerAttack is object) disposablePlayerAttack = enemy.PlayerAttack.OnAttack.Subscribe(_ => OnPlayerAttack());
            enemy.Move.SetDestination(enemy.PlayerAttack.transform.position);
            ChaseUpdate(enemy.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void OnExit()
        {
            disposablePlayerAttack?.Dispose();
        }

        public override void Update()
        {
            if(enemy.Move.GetReachDestination())
            {
                enemy.StateMachine.CurrentState = new EnemyStateAttack(enemy);
            }
            else if(chaseTime > enemy.ChaseLimitTime)
            {
                enemy.Move.StopAgent();

            }
            chaseTime += Time.deltaTime;
        }

        void OnPlayerAttack()
        {
            enemy.Move.SetDestination(enemy.PlayerAttack.transform.position);
            isPlayerAttacked = false;
            chaseTime = 0;
        }

        private async UniTaskVoid ChaseUpdate(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(enemy.WattingTimeOnPlayerLost), cancellationToken: token);
            enemy.StateMachine.CurrentState = new EnemyStateSearch(enemy);
        }
        
    }

}
