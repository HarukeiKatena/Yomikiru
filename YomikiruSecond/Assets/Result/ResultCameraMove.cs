using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  //DOTweenを使うときはこのusingを入れる

public class ResultCameraMove : MonoBehaviour
{
    private Camera resultCamera;  //カメラを格納
    public GameObject playerObj;  //プレイヤーオブジェクトを格納

    Vector3[] camPath =
    {
    new Vector3(0.0f, 5.0f, 4.0f),
    new Vector3(0.0f, 1.5f, 4.0f),
    new Vector3(1.0f, 1.5f, 3.5f),
    new Vector3(2.0f, 1.5f, 3.0f),
    new Vector3(3.0f, 1.5f, 2.5f),
    new Vector3(4.0f, 1.5f, 2.0f),
    new Vector3(3.0f, 1.5f, 2.5f),
    new Vector3(2.0f, 1.5f, 3.0f),
    new Vector3(1.0f, 1.5f, 3.5f),
    new Vector3(0.0f, 3.0f, 4.0f),
    };

    // Start is called before the first frame update
    void Start()
    {
        resultCamera = GetComponent<Camera>();  //カメラを取得
        resultCamera.transform.position = new Vector3(0.0f, 5.5f, 4.5f);

        //移動 cameraの初期位置は(0.0 ,4.0, 3.5)
        resultCamera.transform.DOPath(camPath, 4.0f).SetDelay(1.0f).SetEase(Ease.InSine);
    }

    // Update is called once per frame
    void Update()
    {
        resultCamera.transform.LookAt(new Vector3(playerObj.transform.position.x, playerObj.transform.position.y + 1.0f, playerObj.transform.position.z));  //常にプレイヤーの方を向く
    }
}
