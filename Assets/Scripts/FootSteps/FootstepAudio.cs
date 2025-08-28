using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [System.Serializable]
    public struct SurfaceAudio
    {
        public SurfaceType.Surface surface;
        public AudioClip[] footstepClips;
    }

    public SurfaceAudio[] surfaceAudios;
    public float footstepInterval = 0.5f;

    private float footstepTimer;

    private void Update()
    {
        if (footstepTimer > 0f)
            footstepTimer -= Time.deltaTime;
    }

    public void TryPlayFootstep(Vector3 origin, bool isRunning)
    {
        if (footstepTimer > 0f)
            return;

        // Cambiar el intervalo en función de si corre o camina
        float adjustedInterval = isRunning ? footstepInterval * 0.5f : footstepInterval;
        footstepTimer = adjustedInterval;

        Ray ray = new Ray(origin + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            SurfaceType surfaceType = hit.collider.GetComponent<SurfaceType>();
            if (surfaceType != null)
            {
                PlayFootstep(surfaceType.surfaceType);
            }
            else
            {
                PlayFootstep(SurfaceType.Surface.Default);
            }
        }
        else
        {
            PlayFootstep(SurfaceType.Surface.water);
        }
    }

    void PlayFootstep(SurfaceType.Surface surface)
    {
        foreach (var s in surfaceAudios)
        {
            if (s.surface == surface && s.footstepClips.Length > 0)
            {
                AudioClip clip = s.footstepClips[Random.Range(0, s.footstepClips.Length)];
                audioSource.PlayOneShot(clip);
                return;
            }
        }
    }
}
