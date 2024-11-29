using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public int currentLevel;
    public GameObject victoryScreenPrefab;

    public bool hasWon;
    public bool hasLost;

    // Get reference of player, enemies and goals
    public int score { get; private set; }


    public int remainingLives;

    [SerializeField] private string[] successScenesByLevel = new string[0];
    [SerializeField] private Transform targets;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        if (successScenesByLevel == null) successScenesByLevel = new string[0];

        hasWon = false;
        currentLevel = 0;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void SetLives(int lives)
    {

    }

    void ResetState()
    {

    }

    private void OnValidate()
    {
        if (targets == null)
            Debug.LogError("Targets reference is required", this);
    }

    public void NewGame()
    {
        // Update player tile map reference to mist
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        Player player = FindFirstObjectByType<Player>();

        if (player != null)
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                int layer = tilemap.gameObject.layer;

                if (layer == LayerMask.NameToLayer("Mist"))
                {
                    player.mistTilemap = tilemap;
                }
                else if (layer == LayerMask.NameToLayer("Target"))
                {
                    player.targetTilemap = tilemap;
                }

            }
        }

        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hasWon)
            {
                string nextScene = successScenesByLevel[currentLevel];
                Debug.Log("Should go to success scene" + nextScene);

                if (nextScene != null)
                {
                    GameManager.Instance.TransitionToScene(nextScene);
                }
            }

        }
    }

    void WinGame()
    {
        hasWon = true;
        // Play successful animation and move to next scene
        Debug.Log("TODO");
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            // Freeze the game
            Time.timeScale = 0;
            // player.GetComponent<Movement>().enabled = false;

            GameObject victoryScreen = Instantiate(victoryScreenPrefab);

            victoryScreen.transform.SetParent(canvas.transform, false);

            victoryScreen.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogError("Cannot find active canvas in the scene");
        }
        // Invoke(nameof(Callback), 3.0f);
    }

    public void Lose()
    {
        Time.timeScale = 0;

        // player setactive false
        // if live > 0 show button that reset state
        // else gameover
    }

    // Trigger when user presses restart button after game over
    void RestartGame()
    {
        hasWon = false;
    }

    private void SetScore(int newScore)
    {
        score = newScore;
    }

    public void ItemFound(Target target)
    {
        target.gameObject.SetActive(false);

        SetScore(score + target.points);

        if (!HasRemainingTargets())
        {
            WinGame();
        }
    }
    private bool HasRemainingTargets()
    {
        foreach (Transform target in targets)
        {
            if (target.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }
}
