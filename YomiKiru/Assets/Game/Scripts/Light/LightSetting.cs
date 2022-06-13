/*-------------------------------------------------------
 * 
 *  [LightSetting]
 *  Author : 出合　翔太
 *  ライトの設定
 * 
--------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Light))]
public class LightSetting : MonoBehaviour
{
    [Header("フィールドの明るさ調整")]
    [SerializeField] [Range(0.0f, 0.5f)] private float _IntensityEndValue;

    private Light _light;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void LightFade(float duration)
    {
        _light.DOIntensity(_IntensityEndValue, duration);
    }
}
