using UnityEngine;

public class MenuScene : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.TransitionToScene("Scenes/Dialogue0");
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
