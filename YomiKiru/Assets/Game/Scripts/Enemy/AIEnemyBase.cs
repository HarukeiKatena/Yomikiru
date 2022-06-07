using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

using Cysharp.Threading.Tasks;


namespace Enemy{
    public class AIEnemyBase : MonoBehaviour
    {
        [SerializeField]protected GameObject DeathPrefub;

        private Subject<Unit> _startGame = new Subject<Unit>();
        public IObservable<Unit> _onStartGame
        {
            get { return _startGame; }
        }

        public bool _startGameFlag;

        public IObservable<int> DieEvent => _dieEvent;
        private Subject<int> _dieEvent = new Subject<int>();

        void Start()
        {
            _startGameFlag = false;
            WaitForGameStart().Forget();
        }

        async UniTaskVoid WaitForGameStart()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(10));
            _startGame.OnNext(Unit.Default);
            _startGameFlag = true;
        }

        public void Die(){
            var effect = Instantiate(DeathPrefub, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Destroy(effect, 1.5f);

            _dieEvent.OnNext(3);
            _dieEvent.OnCompleted();
        }
    }
}
