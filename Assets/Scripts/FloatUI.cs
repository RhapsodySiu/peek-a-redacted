using UnityEngine;

public class FloatUI : MonoBehaviour
{
    public float amplitudeMultiplier = 100f; // Multiplier for audio amplitude
    public float smoothSpeed = 2f;        // How quickly to smooth between movements
    public float returnSpeed = 1f;        // How quickly the UI returns to original position

    // Fallback float
    public float speed = 2f;
    public float floatAmplitude = 30f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private float currentOffset = 0f;
    private float targetOffset = 0f;

    public bool useAudio = true;

    [SerializeField] private float amplitudeThreshold = 0.21f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (useAudio)
        {
            // Get audio amplitude from GameManager
            float audioAmplitude = GameManager.Instance.audioAmplitude;

            if (audioAmplitude < 0)
            {
                useAudio = false;
            }

            // Calculate target offset based on audio amplitude
            if (audioAmplitude > amplitudeThreshold)
            {
                targetOffset = -(audioAmplitude * amplitudeMultiplier);
            }
            else
            {
                targetOffset = 0f;
            }

            // Smoothly move towards target
            float currentSpeed = (targetOffset != 0f) ? smoothSpeed : returnSpeed;
            currentOffset = Mathf.Lerp(currentOffset, targetOffset, currentSpeed * Time.deltaTime);

            // Apply position
            rectTransform.anchoredPosition = new Vector2(
                originalPosition.x,
                originalPosition.y + currentOffset
            );
        }
        else
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float offset = Mathf.Lerp(-floatAmplitude, floatAmplitude, smoothT);

            // Apply position
            rectTransform.anchoredPosition = new Vector2(
                originalPosition.x,
                originalPosition.y + offset
            );

        }
    }
}
