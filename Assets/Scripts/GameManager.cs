using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using UnityEngine.Audio;
using System.Linq;

[Serializable]
public class SceneDialoguesMapping {
    public string sceneName;
    public DialogueLine[] dialogueLines;
    public string nextScene;
}

[Serializable]
public class SceneMusicMapping
{

    public string sceneName;
    public AudioResource audioResource;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private AudioSource audioSource;
    private static GameManager _instance;

    [SerializeField] private SceneMusicMapping[] sceneMusicMappings;
    [SerializeField] private SceneDialoguesMapping[] sceneDialogueMappings;
    public string menuScene;

    private AudioResource currentAudioResource;
    private AudioResource nextAudioResource;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // try to find existing instance
                _instance = FindFirstObjectByType<GameManager>();

                if (_instance == null)
                {
                    GameObject gmPrefab = Resources.Load<GameObject>("GameManager");

                    if (gmPrefab != null)
                    {
                        _instance = Instantiate(gmPrefab).GetComponent<GameManager>();
                        _instance.name = "GameManager";
                        DontDestroyOnLoad(_instance);

                        _instance.InitializeDefaultValues();
                    }
                    else
                    {
                        Debug.LogError("GameManager prefab not found in Resources!");
                    }
                }
            }

            return _instance;
        }
    }

    private float[] audioSamples = new float[512];
    public float audioAmplitude {
        get
        {
            audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.Blackman);
            
            float amplitude = 0f;
            for (int i = 0; i < audioSamples.Length; i++)
            {
                amplitude += audioSamples[i];
            }

            return amplitude;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }

        Debug.Log("GameMsanager is destroyed");
    }


    private void Start()
    {
        if (fadeImage == null)
        {
            Debug.LogError("Image comopnent not found!");
            return;
        }

        fadeImage.gameObject.SetActive(true);

        StartCoroutine(FadeInGraphic(2f));

        currentAudioResource = GetMusicForScene($"Scenes/{SceneManager.GetActiveScene().name}");

        if (currentAudioResource) {
            audioSource.resource = currentAudioResource;
            
            StartCoroutine(FadeInMusic(currentAudioResource, 1f));
        }
    }

    void Update()
    {
        //Debug.Log(audioAmplitude);
    }

    private AudioResource GetMusicForScene(string sceneName)
    {
        var mapping = sceneMusicMappings.FirstOrDefault(m => m.sceneName == sceneName);

        return mapping?.audioResource;
    }

    private void UpdateSceneMusic(string newScenePath)
    {
        currentAudioResource = GetMusicForScene($"Scenes/{SceneManager.GetActiveScene().name}");
        nextAudioResource = GetMusicForScene(newScenePath);
    }

    void StartLevel()
    {
        LevelManager.Instance.NewGame();
    }

    public void GoToMenuScene()
    {
        TransitionToScene(menuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void InitializeDefaultValues()
    {
#if UNITY_EDITOR
        Debug.Log("GameManager: Using test initialization");
        // ... test values ...
        menuScene = "Scenes/Menu";
#endif
    }

    public void TransitionToScene(string sceneName)
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        // TODO: stop user input
        UpdateSceneMusic(sceneName);
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        bool isSameMusic = currentAudioResource == nextAudioResource && currentAudioResource != null;

        // Only fade out music if changing to a different track
        if (currentAudioResource != null && !isSameMusic) {
            yield return StartCoroutine(FadeOutMusic(1f));
        }

        yield return StartCoroutine(FadeOutGraphic(1f));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeInGraphic(1f));

        if (nextAudioResource != null && !isSameMusic) {
            yield return StartCoroutine(FadeInMusic(nextAudioResource, 1f));
        }

        var dialogueScene = sceneDialogueMappings.FirstOrDefault(m => m.sceneName == sceneName);

        if (dialogueScene != null) {
            // auto start dialogue
            Dialogue.Instance.StartDialogue(dialogueScene.dialogueLines);
            Dialogue.Instance.SetNextScene(dialogueScene.nextScene);
        } else {
            Debug.Log("Could be level scene");

        }
    }

    private IEnumerator FadeOutGraphic(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(elapsedTime / duration));
            yield return null;
        }
    }

    private IEnumerator FadeInGraphic(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(1 - (elapsedTime / duration)));
            yield return null;
        }
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }
        audioSource.Stop(); // play till end
        audioSource.volume = startVolume; // reset volume
    }

    private IEnumerator FadeInMusic(AudioResource audioResource, float duration)
    {   
        audioSource.resource = audioResource;
        audioSource.Play();
        audioSource.volume = 0;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
    }
}