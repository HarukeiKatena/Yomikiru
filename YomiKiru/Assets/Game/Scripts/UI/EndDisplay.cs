using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

namespace Yomikiru.UI
{

    public class EndDisplay : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI endText;

        public async UniTask DisplayAsync()
        {
            endText.gameObject.SetActive(true);
            await DOTween.Sequence()
                .Join(DOVirtual.Float(1, 0, 0.2f, p => endText.alpha = Mathf.Round(p)).SetEase(Ease.Flash, 10))
                .Join(DOVirtual.Float(0, 75, 2, p => endText.characterSpacing = p).SetEase(Ease.OutQuart))
                .Join(endText.transform.DOScale(1, 2).From(0.8f).SetEase(Ease.OutQuart));
        }

    }

}
