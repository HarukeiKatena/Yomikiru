using UnityEngine;
using UnityEngine.InputSystem;
using Yomikiru.Effect;

public class EffectTest : MonoBehaviour
{
    //チャンネルと使いたいエフェクトのクリップを持っておく
    [SerializeField] private EffectChannel channel;
    [SerializeField] private EffectClip clip;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            //Requestで再生できる(第一引数は自分のオブジェクト名を入れる、第二引数にはクリップ)
            channel.Request("", clip);
        }
    }
}
