using UnityEngine;

public class EndMenu : MonoBehaviour
{
    public void GoToMenuScene() {
        Time.timeScale = 1;
        
        GameManager.Instance.GoToMenuScene();
    }
}
