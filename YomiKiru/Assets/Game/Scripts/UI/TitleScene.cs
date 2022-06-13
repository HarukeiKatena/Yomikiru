using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Yomikiru.UI
{

    public class TitleScene : MonoBehaviour
    {

        [SerializeField] private Button buttonPlay;
        [SerializeField] private Button buttonQuit;

        private void Start()
        {
            buttonPlay.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadScene("NewGame");
            });
            buttonQuit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
            EventSystem.current.SetSelectedGameObject(buttonPlay.gameObject);
        }

    }

}