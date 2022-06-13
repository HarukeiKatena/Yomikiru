using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Player
{
    public class Die : MonoBehaviour
    {
        [SerializeField]protected GameObject DeathPrefub;
        public IObservable<int> DieIvent => die;
        private Subject<int> die = new Subject<int>();

        private PlayerPropSetting _playerProp;

        void Start()
        {
            _playerProp = gameObject.GetComponent<PlayerPropSetting>();
        }

        public void DoDie()
        {
            die.OnNext(_playerProp.PlayerIndex);
            die.OnCompleted();
            var effect = Instantiate(DeathPrefub, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Destroy(effect, 1.5f);
        }
    }
}
