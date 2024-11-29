using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance { get; private set; }

    [SerializeField] private Image backPortrait;
    [SerializeField] private Image centerPortrait;
    [SerializeField] private TextMeshProUGUI nameComponent;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private DialogueLine[] activeLines = new DialogueLine[0];
    public float textSpeed;

    [SerializeField] private Image avatar;

    private int index;
    private string nextScene;

    // Whether the dialogue mode is in progress
    private bool inProgress = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("Create dialogue singleton");
            Instance = this;
            DontDestroyOnLoad(gameObject); // Check whether to delete it 
        }
        else
        {
            Debug.Log("Destroy dialogue singleton");
            Destroy(gameObject);
        }
    }

    public void SetNextScene(string sceneName)
    {
        nextScene = sceneName;
    }

    void Start()
    {
        if (textComponent == null || nameComponent == null) {
            Debug.LogError("Text/Name component not assigned");
            return;
        }

        textComponent.text = string.Empty;
        nameComponent.text = string.Empty;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && inProgress)
        {
            if (textComponent.text == activeLines[index].line)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = activeLines[index].line;
            }
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        if (lines.Length > 0)
        {
            activeLines = lines;
        }

        if (activeLines.Length > 0)
        {
            index = 0;

            inProgress = true;

            StartCoroutine(TypeLine());
        }
    }

    IEnumerator TypeLine()
    {
        var currentLine = activeLines[index];
        Debug.Log("typeLine");
        nameComponent.text = currentLine.characterName == null ? string.Empty : currentLine.characterName;
        if (currentLine.characterImage != null)
        {
            centerPortrait.sprite = currentLine.characterImage;
        }

        avatar?.gameObject.SetActive(currentLine.characterName == "" ? false : true);

        foreach (char c in currentLine.line.ToCharArray())
        {
            Debug.Log(c);
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        Debug.Log("nextLine");
        if (index < activeLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            centerPortrait.gameObject.SetActive(false);

            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        inProgress = false;
        if (nextScene != null)
        {
            GameManager.Instance.TransitionToScene(nextScene);
        }
        else
        {
            Debug.Log("No callback for ended dialogue");
        }
    }
}
