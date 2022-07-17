using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Yomikiru.Character.Enemy.StateMachine;
using UniRx;

namespace Yomikiru.Character.Enemy.State
{
    public sealed class Chase : EnemyState<AIEnemyBase>
    {
        private bool playerAttacking;
        private IDisposable disposablePlayerAttack;
        private float chaseTime;
        public Chase(AIEnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            chaseTime = 0;
            playerAttacking = false;

            disposablePlayerAttack = enemy.PlayerAttack?.OnAttack.Subscribe(_ => playerAttacking = true);
            enemy.Move.SetDestinationForPlayer(enemy.PlayerAttack.transform.position);
            ChaseUpdate(enemy.Cts.Token).Forget();
        }

        public override void OnExit()
        {
            disposablePlayerAttack?.Dispose();
        }

        public override void Update()
        {
            chaseTime += Time.deltaTime;
        }

        private async UniTaskVoid ChaseUpdate(CancellationToken token)
        {
            while(true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                if(enemy.Move.GetReachDestination())
                {
                    enemy.StateMachine.CurrentState = enemy.AttackState;
                    break;
                }
                else if(playerAttacking)
                {
                    enemy.Move.SetDestinationForPlayer(enemy.PlayerAttack.transform.position);
                    playerAttacking = false;
                    chaseTime = 0;
                }
                else if(chaseTime > enemy.ChaseLimitTime)
                {
                    enemy.Move.StopAgent();
                    await UniTask.Delay(TimeSpan.FromSeconds(enemy.WattingTimeOnPlayerLost), cancellationToken: token);
                    enemy.StateMachine.CurrentState = enemy.SearchState;
                    break;
                }
            }
        }
        
    }

}
