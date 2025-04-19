using UnityEngine;
using UnityEngine.UI; // For Image component

public class ScopeController : MonoBehaviour
{
    [Header("References")]
    public GameObject[] images; // UI Images to fade in/out while scoping
    public Animator gunAnimator;

    [Header("Scope Settings")]
    [Range(0f, 1f)] public float scopedWeight = 0.5f;
    public float lerpSpeed = 5f;

    private float currentWeight = 0f;
    private int scopeLayerIndex = 1;
    private float currentAlpha = 1f; // 1 = fully visible, 0 = fully hidden

    void Update()
    {
        bool isScoped = Input.GetMouseButton(1); // Right Mouse Button

        // Lerp animator layer weight
        float targetWeight = isScoped ? scopedWeight : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * lerpSpeed);

        if (gunAnimator != null)
        {
            gunAnimator.SetLayerWeight(scopeLayerIndex, currentWeight);
        }

        // Lerp alpha of UI images
        float targetAlpha = isScoped ? 0f : 1f;
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * lerpSpeed);

        foreach (GameObject imageObj in images)
        {
            if (imageObj != null)
            {
                Image img = imageObj.GetComponent<Image>();
                if (img != null)
                {
                    Color color = img.color;
                    color.a = currentAlpha;
                    img.color = color;
                }
            }
        }
    }
}
