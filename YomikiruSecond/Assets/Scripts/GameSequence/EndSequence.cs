using System.Collections;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Yomikiru.Enemy;
using Player;
using UnityEngine;
using UniRx;
using Yomikiru;

public class EndSequence : MonoBehaviour
{
    [SerializeField] private MatchInfo matchInfo;
    [SerializeField] private PlayerManagement playerManagement;
    [SerializeField] private CinemachineVirtualCamera endTargetVirtualCamera;
    [SerializeField] private Yomikiru.UI.EndDisplay display;
    [SerializeField] private float cameraMoveDelay = 1.0f;
    [SerializeField] private float endWaitTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var c in playerManagement.Character)
        {
            if(c.TryGetComponent<Die>(out var charactor))
                charactor.DieIvent.Subscribe(x => { StartCoroutine(SceneChange(x)); }).AddTo(this);
            else if(c.TryGetComponent<AIEnemyBase>(out var enemy))
                enemy.DieEvent.Subscribe(x => { StartCoroutine(SceneChange(x)); }).AddTo(this);
        }
    }

    IEnumerator SceneChange(int playerindex)
    {
        matchInfo.State.Value = MatchState.Finished;

        display.DisplayAsync().Forget();

        yield return new WaitForSeconds(cameraMoveDelay);
        endTargetVirtualCamera.Priority = 20;

        yield return new WaitForSeconds(endWaitTime);

        //CheckControllerでやったらうまく動かなかったからとりあえずこっちに書く
        ControllerManagement.ClearPlayerDevice();

        GameManager.Instance.LoadScene("Results", GameManager.LoadingScreenType.DARK);
    }
}
