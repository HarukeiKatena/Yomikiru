using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Yomikiru.Enemy;
using Player;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using Yomikiru;

public class EndSequence : MonoBehaviour
{
    [SerializeField] private MatchInfo matchInfo;

    [SerializeField]
    protected PlayerManagement playerManagement;

    [SerializeField]
    protected CinemachineVirtualCamera endTargetVirtualCamera;

    [SerializeField]
    protected Yomikiru.UI.EndDisplay display;

    [SerializeField]
    protected float CameraMoveDelay = 1.0f;

    [SerializeField]
    protected float EndWaitTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var c in playerManagement.Character)
        {
            if(c.TryGetComponent<Die>(out var charactor)){
                charactor.DieIvent.Subscribe(x => { StartCoroutine(SceneChange(x)); }).AddTo(this);
            }
            else if(c.TryGetComponent<AIEnemyBase>(out var enemy)) {
                enemy.DieEvent.Subscribe(x => { StartCoroutine(SceneChange(x)); }).AddTo(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator SceneChange(int playerindex)
    {
        matchInfo.State.Value = MatchState.Finished;

        display.DisplayAsync().Forget();

        yield return new WaitForSeconds(CameraMoveDelay);
        endTargetVirtualCamera.Priority = 20;

        yield return new WaitForSeconds(EndWaitTime);

        //CheckControllerでやったらうまく動かなかったからとりあえずこっちに書く
        ControllerManagement.ClearPlayerDevice();

        GameManager.Instance.LoadScene("Results", GameManager.LoadingScreenType.DARK);
    }
}
