/*-------------------------------------------------------
 * 
 *  [LightSetting]
 *  Author : �o���@�đ�
 *  ���C�g�̐ݒ�
 * 
--------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Light))]
public class LightSetting : MonoBehaviour
{
    [Header("�t�B�[���h�̖��邳����")]
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
