using UnityEngine;

public class KeyPressAudio : MonoBehaviour
{
    [Header("Movement Key Sounds")]
    public AudioClip wKeySound;
    public AudioClip aKeySound;
    public AudioClip sKeySound;
    public AudioClip dKeySound;

    [Header("Action Key Sounds")]
    public AudioClip spaceKeySound;
    public AudioClip ctrlKeySound;

    private AudioSource audioSource;
    private AudioSource oneShotAudioSource; // For one-time sounds

    private bool isWPressed = false;
    private bool isAPressed = false;
    private bool isSPressed = false;
    private bool isDPressed = false;

    void Start()
    {
        // Create main audio source for continuous playback
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        // Create separate audio source for one-shot sounds
        oneShotAudioSource = gameObject.AddComponent<AudioSource>();
        oneShotAudioSource.loop = false;
    }

    void Update()
    {
        // Handle WASD keys (continuous play while pressed)
        HandleKey(KeyCode.W, ref isWPressed, wKeySound);
        HandleKey(KeyCode.A, ref isAPressed, aKeySound);
        HandleKey(KeyCode.S, ref isSPressed, sKeySound);
        HandleKey(KeyCode.D, ref isDPressed, dKeySound);

        // Handle Space key (one-time play)
        if (Input.GetKeyDown(KeyCode.Space) && spaceKeySound != null)
        {
            oneShotAudioSource.PlayOneShot(spaceKeySound);
        }

        // Handle Ctrl key (one-time play)
        if (Input.GetKeyDown(KeyCode.LeftControl) && ctrlKeySound != null)
        {
            oneShotAudioSource.PlayOneShot(ctrlKeySound);
        }
    }

    private void HandleKey(KeyCode key, ref bool isPressed, AudioClip clip)
    {
        if (Input.GetKeyDown(key) && clip != null && !isPressed)
        {
            isPressed = true;
            audioSource.clip = clip;
            audioSource.Play();
        }
        else if (Input.GetKeyUp(key))
        {
            isPressed = false;

            // Only stop if no other movement keys are pressed
            if (!(isWPressed || isAPressed || isSPressed || isDPressed))
            {
                audioSource.Stop();
            }
            else if (isPressed)
            {
                // Switch to the newly pressed key's sound
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}