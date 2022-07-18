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
        private IDisposable disposablePlayerAttack;
        private float chaseTime;
        public EnemyStateChase(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            chaseTime = 0;

            if(enemy.PlayerAttack != null) disposablePlayerAttack = enemy.PlayerAttack.OnAttack.Subscribe(_ => OnPlayerAttack());
            enemy.Move.SetDestination(enemy.PlayerAttack.transform.position);
        }

        public override void OnExit()
        {
            disposablePlayerAttack?.Dispose();
        }

        public override void Update()
        {
            if(enemy.Move.GetAgentIsStopped()) return;
            if(enemy.Move.GetReachDestination())
            {
                enemy.StateMachine.CurrentState = new EnemyStateAttack(enemy);
            }
            else if(chaseTime > enemy.ChaseLimitTime)
            {
                OnChaseOverTime(enemy.GetCancellationTokenOnDestroy()).Forget();
            }
            chaseTime += Time.deltaTime;
        }

        void OnPlayerAttack()
        {
            enemy.Move.SetDestination(enemy.PlayerAttack.transform.position);
            chaseTime = 0;
        }

        private async UniTaskVoid OnChaseOverTime(CancellationToken token)
        {
            enemy.Move.StopAgent();
            await UniTask.Delay(TimeSpan.FromSeconds(enemy.WattingTimeOnPlayerLost), cancellationToken: token);
            enemy.StateMachine.CurrentState = new EnemyStateSearch(enemy);
        }
        
    }

}
