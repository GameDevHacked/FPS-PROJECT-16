using UnityEngine;
using UnityEngine.UI;

public class ScopeControllerMobile : MonoBehaviour
{
    [Header("References")]
    public GameObject[] images; // UI Images to fade in/out while scoping
    public Animator gunAnimator;
    public Button scopeButton; // Mobile scope button

    [Header("Scope Settings")]
    [Range(0f, 1f)] public float scopedWeight = 0.5f;
    public float lerpSpeed = 5f;

    private float currentWeight = 0f;
    private int scopeLayerIndex = 1;
    private float currentAlpha = 1f; // 1 = fully visible, 0 = fully hidden
    private bool isScoped = false;

    private void Start()
    {
        // Set up mobile button listeners
        if (scopeButton != null)
        {
            // Using EventTrigger for both press and release
            var trigger = scopeButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            // Pointer Down (start scoping)
            var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => { isScoped = true; });
            trigger.triggers.Add(pointerDown);

            // Pointer Up (stop scoping)
            var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((e) => { isScoped = false; });
            trigger.triggers.Add(pointerUp);
        }
    }

    void Update()
    {
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