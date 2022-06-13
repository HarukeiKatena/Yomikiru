using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Yomikiru
{
    public sealed class GameManager : MonoBehaviour
    {

        public enum LoadingScreenType
        {
            DEFAULT,
            DARK,
        }

        public static GameManager Instance { get; private set; }

        [SerializeField] private LoadingScreen loadingScreenDefault;
        [SerializeField] private LoadingScreen loadingScreenDark;

        private bool isChangingScene;
        private string lastScene;

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (SceneManager.sceneCount > 1)
            {
                lastScene = SceneManager.GetActiveScene().name;
            }
            else
            {
                SceneManager.LoadScene(0, LoadSceneMode.Additive);
                lastScene = SceneManager.GetSceneByBuildIndex(0).name;
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void LoadScene(string name, LoadingScreenType loadingScreenType = LoadingScreenType.DEFAULT)
        {
            if (isChangingScene) return;
            LoadSceneAsync(lastScene, name, loadingScreenType).Forget();
            lastScene = name;
        }

        public void ReloadScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        private async UniTask LoadSceneAsync(string sceneToUnload, string sceneToLoad, LoadingScreenType loadingScreenType)
        {
            isChangingScene = true;
            LoadingScreen loadingScreen = loadingScreenType switch
            {
                LoadingScreenType.DEFAULT => loadingScreenDefault,
                LoadingScreenType.DARK    => loadingScreenDark,
                _ => throw new ArgumentOutOfRangeException(nameof(loadingScreenType)),
            };
            await loadingScreen.ShowAsync();
            await SceneManager.UnloadSceneAsync(sceneToUnload);
            await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
            await loadingScreen.HideAsync();
            isChangingScene = false;
        }

    }
}
