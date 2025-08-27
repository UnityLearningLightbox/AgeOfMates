using System.Collections.Generic;
using UnityEngine;

public class TEST_SoundStep : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioSource footStepsAudioSource;
    [SerializeField] AudioClip[] footStepsClips;

    [Header("FootSteps Timings")]
    [SerializeField] float walkStepsDelay;
    [SerializeField] float runStepsDelay;

    [Header("RayCast")]
    [SerializeField] float rayCastLength = 0.1f;
    [SerializeField] Transform raycastOrigin;
    private bool enableRaycast;

    [Header("References")]
    [SerializeField] TademiusController characterController;
    private Rigidbody rb;
    private float stepTimer = 0f;

    private Dictionary<string, AudioClip[]> surfaceClipsDictionary;

    private string currentSurfaceTag = "Default";

    [System.Serializable]
    public class SurfaceFootstepsClips
    {
        public string surfaceTags;
        public AudioClip[] clisp;
    }
    [SerializeField] SurfaceFootstepsClips[] surfaceFootstepsClips;

    private void Start()
    {
        InitialSettings();
    }

    private void Update()
    {
        AudioSettings();
        enableRaycast = (Time.frameCount % 2 == 0); // raycast en frames pares

        //if(enableRaycast)
        //{
        //    UpdateSurfaceTag();
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Vector3 start = transform.position + Vector3.up;
        //Vector3 end = start + Vector3.down * rayCastLength;
        //Gizmos.DrawLine(start, end);
        Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + Vector3.down * rayCastLength);
    }

    void InitialSettings()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (characterController == null)
        {
            characterController = GetComponent<TademiusController>();
        }

        if (footStepsAudioSource == null)
        {
            footStepsAudioSource = GetComponent<AudioSource>();
        }

        //surfaceClipsDictionary = new Dictionary<string, AudioClip[]>();

        //foreach(var entry in surfaceClipsDictionary)
        //{
        //    if (!surfaceClipsDictionary.ContainsKey(entry.Key))
        //    {
        //        surfaceClipsDictionary.Add(entry.Key, entry.Value);
        //    }
        //}
    }

    void AudioSettings()
    {
        if (rb == null || characterController == null || footStepsAudioSource == null || footStepsClips.Length == 0) return;
        bool isMoving = rb.linearVelocity.magnitude > 0.2f; // Si detectamos cualquier tipo de fuerza que altere nuestra posicion de reposo => nos movemos: bool = true
        bool isRunning = PlayerIsRunning();
        bool isGrounded = PlayerIsGrounded();

        bool inputPressed =
            Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.RightArrow);

        if (inputPressed)
        {
            stepTimer += Time.deltaTime;
            float delay = isRunning ? runStepsDelay : walkStepsDelay;

            if (stepTimer >= delay)
            {
                PlayStepsSound();
                stepTimer = 0f;
            }

        }
        else
        {
            stepTimer = Mathf.Min(stepTimer, walkStepsDelay);
        }
    }

    void PlayStepsSound()
    {
        int index = Random.Range(0, footStepsClips.Length);
        footStepsAudioSource.clip = footStepsClips[index];
        footStepsAudioSource.pitch = Random.Range(0.8f, 1.2f);
        footStepsAudioSource.Play();
    }

    void PlaySpecificStepSound()
    {
        if (footStepsAudioSource.isPlaying) return;

        string surfaceTag = DetectSurfaceTag();
        AudioClip[] selectedClips;

        if (surfaceClipsDictionary != null && surfaceClipsDictionary.TryGetValue(surfaceTag, out selectedClips) && selectedClips.Length > 0)
        {
            int index = Random.Range(0, selectedClips.Length);
            footStepsAudioSource.clip = footStepsClips[index];
            footStepsAudioSource.Play();

        }
        else
        {
            int index = Random.Range(0, footStepsClips.Length);
            footStepsAudioSource.clip = footStepsClips[index];
            footStepsAudioSource.Play();
        }

        footStepsAudioSource.pitch = Random.Range(0.8f, 1.2f);
        footStepsAudioSource.Play();
    }

    void UpdateSurfaceTag()
    {
        if (raycastOrigin == null) return;

        Ray ray = new Ray(raycastOrigin.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayCastLength))
        {
            currentSurfaceTag = hit.collider.tag;

        }
        else
        {
            currentSurfaceTag = "Default";
        }
    }

    bool PlayerIsGrounded()
    {
        return characterController != null && PlayerRest();
    }

    bool PlayerIsRunning()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized; //
        return input.magnitude > 0.1f && Input.GetKey(KeyCode.LeftShift);
    }

    bool PlayerRest()
    {
        return (bool)typeof(TademiusController).GetField("isGrounded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(characterController);
    }

    string DetectSurfaceTag()
    {
        if (rayCastLength == null)
        {
            return "Untagged";
        }

        //Ray ray = new Ray(transform.position + Vector3.up * rayCastLength, Vector3.down);
        Ray ray = new Ray(raycastOrigin.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayCastLength))
        {
            return hit.collider.tag;
        }

        return "Untagged";
    }
}
