using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI targetText;
    public RectTransform healthContainer;
    public Image sweepCoolDownIndicator;

    public Image flashImage;
    public float flashDuration = 0.5f;
    public float flashAlpha = 1f;

    [SerializeField] private GameObject heartIconPrefab;
    [SerializeField] private GameObject victoryScreenPrefab;
    [SerializeField] private GameObject gameoverScreenPrefab;

    void OnValidate()
    {
        if (scoreText == null)
        {
            Debug.LogError("scoreText is not set in LevelUI");
        }

        if (sweepCoolDownIndicator == null)
        {
            Debug.LogError("sweepCoolDownIndicator is not set in LevelUI");
        }

        if (targetText == null)
        {
            Debug.LogError("targetText is not set in LevelUI");
        }

        if (healthContainer == null)
        {
            Debug.LogError("healthContainer is not set in LevelUI");
        }

        if (heartIconPrefab == null)
        {
            Debug.LogError("heartIconPrefab is not set");
        }
    }

    private void Start()
    {
        flashImage?.gameObject.SetActive(false);
    }

    public void fillHearts(int count)
    {
        // Clear existing hearts
        foreach (Transform child in healthContainer) {
            Destroy(child.gameObject);
        }

        // Create new hearts based on count
        for (int i = 0; i < count; i++) {
            GameObject heart = Instantiate(heartIconPrefab, healthContainer);
            heart.transform.SetAsLastSibling();
        }
    }

    public void subtractHeart()
    {
        if (healthContainer.childCount > 0)
        {
            Transform lastHeart = healthContainer.GetChild(healthContainer.childCount - 1);
            Destroy(lastHeart.gameObject);
        }
    }

    public void UpdateItemText(int found, int total)
    {
        targetText.text = $"Items found: {found}/{total}";
    }

    public void ResetCooldown(float duration)
    {
        sweepCoolDownIndicator.fillAmount = 0;

        StartCoroutine(ResetCoolDownCoroutine(duration));
    }

    IEnumerator ResetCoolDownCoroutine(float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            sweepCoolDownIndicator.fillAmount = (float)elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sweepCoolDownIndicator.fillAmount = 1;
    }

    private void showScreen(GameObject screen)
    {
        screen.transform.SetParent(transform, false);
        screen.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void showVictoryScreen()
    {
        GameObject victoryScreen = Instantiate(victoryScreenPrefab);

        showScreen(victoryScreen);
    }

    public void showGameOverScreen()
    {
        GameObject gameoverScreen = Instantiate(gameoverScreenPrefab);

        showScreen(gameoverScreen);

        var rect = gameoverScreen.GetComponent<RectTransform>();

        rect.offsetMin = new Vector2(0f, 0f);
        rect.offsetMax = new Vector2(-894f, 0f);
    }

    public void TriggerFlash()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        flashImage.gameObject.SetActive(true); // Show the flash image
        Color color = flashImage.color;
        color.a = flashAlpha;
        flashImage.color = color;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, flashAlpha, elapsedTime / (flashDuration / 2));
            flashImage.color = color;
            yield return null;
        }

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(flashAlpha, 0, elapsedTime / (flashDuration / 2));
            flashImage.color = color;
            yield return null;
        }

        flashImage.gameObject.SetActive(false); // Hide the image after flashing
    }
}
