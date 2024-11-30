using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance {
        get
        {
            if (_instance == null)
            {
                GameObject dPrefab = Resources.Load<GameObject>("DialogueManager");

                if (dPrefab != null)
                {
                    _instance = Instantiate(dPrefab).GetComponent<Dialogue>();
                    _instance.name = "DialogueManager";
                    DontDestroyOnLoad(_instance);
                } else
                {
                    Debug.LogError("DialogueManager prefab not found in Resources!");
                }
            }

            return _instance;
        }
    }
    private static Dialogue _instance;

    [SerializeField] private Image backPortrait;
    [SerializeField] private Image centerPortrait;
    [SerializeField] private TextMeshProUGUI nameComponent;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private DialogueLine[] activeLines = new DialogueLine[0];
    public float textSpeed;

    private Sprite _defaultAvatarSprite;
    [SerializeField] private Sprite smileAvatarSprite;
    [SerializeField] private Image avatar;

    private int index;
    private string nextScene;

    // Whether the dialogue mode is in progress
    private bool inProgress = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("Destroy duplicate Dialogue");
            DestroyImmediate(gameObject);

            return;
             
        }
        
        Debug.Log("Create dialogue singleton");
        _instance = this;
        DontDestroyOnLoad(gameObject);
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

        inProgress = false;
        backPortrait.gameObject.SetActive(false);
        centerPortrait.gameObject.SetActive(false);
        textComponent.text = string.Empty;
        nameComponent.text = string.Empty;
        _defaultAvatarSprite = avatar.sprite;
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
        avatar?.gameObject.SetActive(false);
        Debug.Log("StartDialogue");
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
            if (currentLine.useCenterPortrait) 
            {
                backPortrait.gameObject.SetActive(false);
                centerPortrait.gameObject.SetActive(true);
                centerPortrait.sprite = currentLine.characterImage;
            }
            else
            {
                backPortrait.gameObject.SetActive(true);
                centerPortrait.gameObject.SetActive(false);
                backPortrait.sprite = currentLine.characterImage;
            }
        }
        else
        {
            centerPortrait.gameObject.SetActive(false);
            backPortrait.gameObject.SetActive(false);
        }

        avatar?.gameObject.SetActive(currentLine.characterName == "");

        if (currentLine.characterName == "" && avatar != null)
        {
            avatar.sprite = (currentLine.isSmile ? smileAvatarSprite : _defaultAvatarSprite);
        }

        foreach (char c in currentLine.line.ToCharArray())
        {
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
