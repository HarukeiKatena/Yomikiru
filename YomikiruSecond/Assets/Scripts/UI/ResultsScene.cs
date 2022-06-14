using System;
using System.Linq;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    public class ResultsScene : MonoBehaviour
    {

        [SerializeField] private string nextScene;
        [SerializeField] private CinemachineDollyCart dolly;
        [SerializeField] private TextMeshPro textWinnerName;
        [SerializeField] private Light[] lights;
        [SerializeField] private MatchInfo gameScriptableObject;

        private void Start()
        {
            textWinnerName.text = (gameScriptableObject.WinnerPlayerIndex + 1) + "P";

            Run().Forget();
        }

        private async UniTaskVoid Run()
        {
            var sequence = DOTween.Sequence()
                .Join(DOVirtual.Float(0, 1, 4, v => dolly.m_Position = v).SetEase(Ease.OutCubic))
                .Join(textWinnerName.DOFade(1, 2).From(0).SetDelay(1));
            foreach (var light in lights)
            {
                sequence = sequence.Join(light.DOIntensity(light.intensity, 2).From(0));
            }
            await sequence;
            GameManager.Instance.LoadScene(nextScene);
        }

    }

}
