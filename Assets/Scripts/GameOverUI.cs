using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
