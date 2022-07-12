using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;  //DOTweenを使うときはこのusingを入れる

public class FadeController : MonoBehaviour
{
    private Image FadePanel;  //フェード用のパネルを格納
    private float alpha = 1.0f;
    public float fadeSpeed;  //フェード速度
    public float fadeWaitTime;  //フェードアウトまでの待ち時間
    public float transitionWaitTime;  //シーン切り替えまでの待ち時間
    private float fadeTime = 0.0f;

    private Text WinnerText;  //勝者テキストを格納
    public int winner = 0;  //勝者 1Pなら1、2Pなら2を指定

    public Animator anim;  //アニメーターを格納

    //ボタンを格納
    private GameObject RetryButton;
    private GameObject TitleButton;
    private string NextSceneName;  //どのシーンに遷移するか


    private enum FADE_STATUS
    {
        IDLE,
        FADE_IN,
        CHAR_ANIM,
        FADE_WAIT,
        FADE_OUT,
        TRANSITION_WAIT,
        FINISH,
    }
    private FADE_STATUS fadeStatus;


    // Start is called before the first frame update
    void Start()
    {
        FadePanel = GetComponent<Image>();  //Imageを取得
        WinnerText = GameObject.Find("WinnerText").GetComponent<Text>();  //WinnerTextを取得
        WinnerText.text =  "勝者 " + winner + "P";  //勝者を表示

        //ボタンを取得
        RetryButton = GameObject.Find("RetryButton");
        TitleButton = GameObject.Find("TitleButton");
        //ボタンを非表示に
        RetryButton.SetActive(false);
        TitleButton.SetActive(false);

        fadeStatus = FADE_STATUS.IDLE;  //ステートを初期化
    }

    // Update is called once per frame
    void Update()
    {
        FadePanel.color = new Color(1, 1, 1, alpha);

        switch (fadeStatus)
        {
            case FADE_STATUS.IDLE:
                WinnerText.rectTransform.DOLocalMove(new Vector3(-610, 440, 0), 1.5f).SetEase(Ease.InOutSine);  //勝者テキストを移動
                fadeStatus = FADE_STATUS.FADE_IN;
                break;

            case FADE_STATUS.FADE_IN:
                if (alpha > 0.0f)
                {
                    alpha -= fadeSpeed * Time.deltaTime;
                }
                else
                {
                    alpha = 0.0f;
                    fadeStatus = FADE_STATUS.CHAR_ANIM;
                }
                break;

            case FADE_STATUS.CHAR_ANIM:
                //勝利アニメーションを再生
                anim.SetBool("isWin", true);
                fadeStatus = FADE_STATUS.FADE_WAIT;
                break;

            case FADE_STATUS.FADE_WAIT:
                //ボタンを表示
                RetryButton.SetActive(true);
                TitleButton.SetActive(true);
                break;

            case FADE_STATUS.FADE_OUT:
                if (alpha < 1.0f)
                {
                    alpha += fadeSpeed * Time.deltaTime;
                }
                else
                {
                    alpha = 1.0f;
                    fadeStatus = FADE_STATUS.TRANSITION_WAIT;
                }
                break;

            case FADE_STATUS.TRANSITION_WAIT:
                fadeTime += Time.deltaTime;
                if (fadeTime > transitionWaitTime)
                {
                    fadeTime = 0.0f;
                    fadeStatus = FADE_STATUS.FINISH;
                }
                break;

            case FADE_STATUS.FINISH:
                SceneManager.LoadScene(NextSceneName);  //名前で指定した次のシーンへ
                break;

            default:
                break;
        }
    }

    //リトライボタンを押したときの挙動
    public void OnClickRetryButton()
    {
        NextSceneName = "GameScene";  //Todo:ここにゲームシーン名を入れる
        //ボタンを非表示に
        RetryButton.SetActive(false);
        TitleButton.SetActive(false);
        fadeStatus = FADE_STATUS.FADE_OUT;
    }

    //タイトルボタンを押したときの挙動
    public void OnClickTitleButton()
    {
        NextSceneName = "TitleScene";  //Todo:ここにタイトルシーン名を入れる
        //ボタンを非表示に
        RetryButton.SetActive(false);
        TitleButton.SetActive(false);
        fadeStatus = FADE_STATUS.FADE_OUT;
    }
}
