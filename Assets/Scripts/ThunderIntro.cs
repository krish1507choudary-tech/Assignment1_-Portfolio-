using UnityEngine;
using System.Collections;

public class ThunderIntro : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public ParticleSystem thunderEffect;
    public AudioSource thunderSound;

    [Header("Timing")]
    public float thunderDelay = 8.5f;
    public float playerRevealDelay = 0.15f;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // Hide player
        if (player != null)
        {
            player.SetActive(false);
        }

        // Make sure thunder is stopped
        if (thunderEffect != null)
        {
            thunderEffect.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        // Wait 8.5 seconds
        yield return new WaitForSeconds(thunderDelay);

        // THUNDER + SOUND AT THE SAME TIME
        if (thunderEffect != null)
        {
            thunderEffect.Play();
        }

        if (thunderSound != null)
        {
            thunderSound.Play();
        }

        // Small delay before player appears
        yield return new WaitForSeconds(playerRevealDelay);

        // Reveal player
        if (player != null)
        {
            player.SetActive(true);
        }
    }
}