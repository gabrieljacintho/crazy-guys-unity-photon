using System;
using System.Collections;
#if FMOD
using GabrielBertasso.FMODIntegration;
#endif
using GabrielBertasso.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GabrielBertasso.SceneManagement
{
    [Serializable]
    public enum SceneSelectionMode
    {
        Name,
        Index,
        Next,
        Previous,
        Current
    }

    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneSelectionMode _selectionMode;
        [ShowIf("@_selectionMode == SceneSelectionMode.Name")]
        [SerializeField] private string _sceneName;
        [ShowIf("@_selectionMode == SceneSelectionMode.Index")]
        [SerializeField] private int _sceneIndex;
        [SerializeField] private LoadSceneMode _mode = LoadSceneMode.Single;
        [SerializeField] private float _minAsyncLoadSeconds;
        [SerializeField] private bool _loadOnStart;
        [ShowIf("_loadOnStart")]
        [SerializeField] private bool _async = true;
        [SerializeField] private bool _allowSceneActivation = true;
        [SerializeField] private string _loadingScreenId = "loading";

        private float _asyncLoadStartTime;

        public AsyncOperation AsyncOperation { get; private set; }
        public bool AllowSceneActivation
        {
            get => _allowSceneActivation;
            set => _allowSceneActivation = value;
        }

        public float AsyncLoadProgress => GetAsyncLoadProgress();

        [Space]
        public UnityEvent OnLoadStarted;


        private void Start()
        {
            if (_loadOnStart)
            {
                if (_async)
                {
                    LoadAsync();
                }
                else
                {
                    Load();
                }
            }
        }

        [Button]
        public void Load()
        {
            switch (_selectionMode)
            {
                case SceneSelectionMode.Name:
                    Load(_sceneName);
                    break;

                case SceneSelectionMode.Index:
                    Load(_sceneIndex);
                    break;

                case SceneSelectionMode.Next:
                    int sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    sceneIndex = Mathf.Clamp(++sceneIndex, 0, SceneManager.sceneCountInBuildSettings - 1);
                    Load(sceneIndex);
                    break;

                case SceneSelectionMode.Previous:
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    sceneIndex = Mathf.Clamp(--sceneIndex, 0, SceneManager.sceneCountInBuildSettings - 1);
                    Load(sceneIndex);
                    break;

                case SceneSelectionMode.Current:
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    Load(sceneIndex);
                    break;
            }
        }

        public void Load(string sceneName)
        {
            PreLoad();

            SceneManager.LoadScene(sceneName);
            OnLoadStarted?.Invoke();
        }

        public void Load(int sceneIndex)
        {
            PreLoad();

            SceneManager.LoadScene(sceneIndex);
            OnLoadStarted?.Invoke();
        }

        [Button]
        public void LoadAsync()
        {
            switch (_selectionMode)
            {
                case SceneSelectionMode.Name:
                    StartCoroutine(LoadAsyncRoutine(_sceneName));
                    break;

                case SceneSelectionMode.Index:
                    StartCoroutine(LoadAsyncRoutine(_sceneIndex));
                    break;

                case SceneSelectionMode.Next:
                    int sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    sceneIndex = Mathf.Clamp(++sceneIndex, 0, SceneManager.sceneCountInBuildSettings - 1);
                    StartCoroutine(LoadAsyncRoutine(sceneIndex));
                    break;

                case SceneSelectionMode.Previous:
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    sceneIndex = Mathf.Clamp(--sceneIndex, 0, SceneManager.sceneCountInBuildSettings - 1);
                    StartCoroutine(LoadAsyncRoutine(sceneIndex));
                    break;

                case SceneSelectionMode.Current:
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    StartCoroutine(LoadAsyncRoutine(sceneIndex));
                    break;
            }
        }

        public void LoadAsync(string sceneName)
        {
            StartCoroutine(LoadAsyncRoutine(sceneName));
        }

        public void LoadAsync(int sceneIndex)
        {
            StartCoroutine(LoadAsyncRoutine(sceneIndex));
        }

        private IEnumerator LoadAsyncRoutine(string sceneName)
        {
            PreLoad();

            yield return null;

            Time.timeScale = 1f;
#if FMOD
            FMODAudioManager.SetPaused(true);
#endif

            AsyncOperation = SceneManager.LoadSceneAsync(sceneName, _mode);
            OnLoadStarted?.Invoke();

            yield return PostLoadAsyncRoutine();
        }

        private IEnumerator LoadAsyncRoutine(int sceneIndex)
        {
            PreLoad();

            yield return null;

            Time.timeScale = 1f;
#if FMOD
            FMODAudioManager.SetPaused(true);
#endif

            AsyncOperation = SceneManager.LoadSceneAsync(sceneIndex, _mode);
            OnLoadStarted?.Invoke();

            yield return PostLoadAsyncRoutine();
        }

        private void PreLoad()
        {
            if (!string.IsNullOrEmpty(_loadingScreenId) && UIManager.HasInstance)
            {
                UIManager.Instance.OpenUI(_loadingScreenId);
            }
        }

        private IEnumerator PostLoadAsyncRoutine()
        {
            _asyncLoadStartTime = Time.time;

            if (_minAsyncLoadSeconds > 0f)
            {
                AsyncOperation.allowSceneActivation = false;
                yield return new WaitForSeconds(_minAsyncLoadSeconds);
            }

            if (!AllowSceneActivation)
            {
                AsyncOperation.allowSceneActivation = false;
                yield return new WaitWhile(() => !AllowSceneActivation);
            }

            AsyncOperation.allowSceneActivation = true;
        }

        private float GetAsyncLoadProgress()
        {
            if (AsyncOperation == null)
            {
                return 0f;
            }

            float elapsedSeconds = Time.time - _asyncLoadStartTime;
            float minSecondsProgress = elapsedSeconds / _minAsyncLoadSeconds;

            float loadProgress = AsyncOperation.progress / 0.9f;

            return Mathf.Min(minSecondsProgress, loadProgress);
        }
    }
}