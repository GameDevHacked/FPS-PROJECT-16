using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ScaleSursorWhileMoving : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float scaleUpFactor = 5f;
    [SerializeField] private float scaleDownFactor = 0.5f;
    [SerializeField] private float lerpSpeed = 5f;
    
    private Vector3 defaultScale;
    private enum ScaleState { Normal, Up, Down }
    private ScaleState currentState;

    void Start()
    {
        if (targetObject != null)
        {
            defaultScale = targetObject.transform.localScale;
        }
    }
    void Update()
    {
        Time.timeScale = 1.5f;

        if (targetObject == null) return;

        // Determine scale state (Ctrl has priority)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            currentState = ScaleState.Down;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || 
                Input.GetKey(KeyCode.Space))
        {
            currentState = ScaleState.Up;
        }
        else
        {
            currentState = ScaleState.Normal;
        }

        // Calculate target scale based on state
        Vector3 target = currentState switch
        {
            ScaleState.Up => defaultScale * scaleUpFactor,
            ScaleState.Down => defaultScale * scaleDownFactor,
            _ => defaultScale
        };
        targetObject.transform.localScale = Vector3.Lerp(
            targetObject.transform.localScale, 
            target, 
            lerpSpeed * Time.deltaTime
        );
    }
}
