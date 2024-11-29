using UnityEngine;

public class FloatUI : MonoBehaviour
{
    public float amplitudeMultiplier = 100f; // Multiplier for audio amplitude
    public float smoothSpeed = 2f;        // How quickly to smooth between movements
    public float returnSpeed = 1f;        // How quickly the UI returns to original position
    
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private float currentOffset = 0f;
    private float targetOffset = 0f;

    [SerializeField] private float amplitudeThreshold = 0.21f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Get audio amplitude from GameManager
        float audioAmplitude = GameManager.Instance.audioAmplitude;
        
        // Calculate target offset based on audio amplitude
        if (audioAmplitude > amplitudeThreshold) {
            targetOffset = -(audioAmplitude * amplitudeMultiplier);
        } else {
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
}
