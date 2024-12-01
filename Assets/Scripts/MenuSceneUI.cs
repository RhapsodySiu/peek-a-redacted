using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;

    public TextMeshProUGUI debugText;

    public void Start()
    {
        if ((LevelManager.Instance?.currentLevel == 0 && LevelManager.Instance?.hasLost == true)
        || (LevelManager.Instance?.currentLevel > 0))
        {
            level1Button.gameObject.SetActive(true);
        }

        if (LevelManager.Instance?.currentLevel > 0)
        {
            level2Button.gameObject.SetActive(true);
        }
    }

    public void StartGame(string sceneName)
    {
        Debug.Log($"Start game {sceneName}");
        GameManager.Instance.TransitionToScene(sceneName);
    }

    public void GoToLevel1()
    {
        GameManager.Instance.TransitionToScene("Scenes/Level0");
    }

    public void GoToLevel2()
    {
        GameManager.Instance.TransitionToScene("Scenes/Level1");
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    private void Update()
    {
        if (debugText != null)
        {
            debugText.text = GameManager.Instance.audioAmplitude.ToSafeString();
        }
    }
}
