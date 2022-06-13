using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

using Cysharp.Threading.Tasks;


namespace Yomikiru.Enemy
{
    public class AIEnemyBase : MonoBehaviour
    {
        [SerializeField] private GameObject deathPrefab;

        private Subject<Unit> startGame = new Subject<Unit>();
        public IObservable<Unit> OnStartGame
        {
            get { return startGame; }
        }

        public bool StartGameFlag;

        public IObservable<int> DieEvent => dieEvent;
        private Subject<int> dieEvent = new Subject<int>();

        void Start()
        {
            StartGameFlag = false;
            WaitForGameStart().Forget();
        }

        async UniTaskVoid WaitForGameStart()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(10));
            startGame.OnNext(Unit.Default);
            StartGameFlag = true;
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
