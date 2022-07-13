using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Yomikiru; //DOTweenを使うときはこのusingを入れる

public class FadeController : MonoBehaviour
{
    [SerializeField] private Text WinnerText;
    [field: SerializeField] public MatchInfo matchInfo { get; private set; }
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        WinnerText = GameObject.Find("WinnerText").GetComponent<Text>();  //WinnerTextを取得
        WinnerText.text =  "勝者 " + (matchInfo.WinnerPlayerIndex + 1) + "P";  //勝者を表示

        ResultSequence(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask ResultSequence(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        //勝利アニメーション
        animator.SetBool("isWin", true);

        //勝利テキストのアニメーション
        WinnerText.rectTransform.DOLocalMove(new Vector3(-610, 440, 0), 1.5f).SetEase(Ease.InOutSine).ToUniTask(cancellationToken: token);
    }
}
