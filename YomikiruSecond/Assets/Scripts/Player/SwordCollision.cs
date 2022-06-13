using System.Collections;
using Player;
using UnityEngine;
using Yomikiru.Enemy;

public class SwordCollision : MonoBehaviour
{

    [SerializeField]
    private GameObject self;

    [SerializeField]
    private PlayerPropSetting playerProp;

    [SerializeField]
    private GameScriptableObject gameScriptableObject;

    private void OnTriggerEnter(Collider other) {
        if(self == other.gameObject) return;
        if(other.gameObject.TryGetComponent<Die>(out var p)){
            p.DoDie();
            gameScriptableObject.WinnerPlayerIndex = playerProp.PlayerIndex;
        }
        else if(other.gameObject.TryGetComponent<AIEnemyBase>(out var enemy)){
            enemy.Die();
            gameScriptableObject.WinnerPlayerIndex = 3;
        }

        //勝者
        gameScriptableObject.WinnerPlayerIndex = playerProp.PlayerIndex;
    }
}
