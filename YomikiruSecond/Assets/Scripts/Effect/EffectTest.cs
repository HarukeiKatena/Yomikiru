using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yomikiru.Effect;

public class EffectTest : MonoBehaviour
{
    [SerializeField] private EffectChannel channel;
    [SerializeField] private EffectClip clip;

    // Start is called before the first frame update
    void Start()
    {
        channel.Request("", clip);
    }
}
