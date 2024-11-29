using UnityEngine;

public class DialgoueSceneTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start dialgoue scene test");
        Dialogue.Instance.StartDialogue(new DialogueLine[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
