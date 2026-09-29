using UnityEngine;
using System.Collections;

public class BrokenWallDissolve : MonoBehaviour
{
    [Header("Dissolve")]
    public float dissolveDelay = 1.5f;
    public float dissolveDuration = 3f;

    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        StartCoroutine(DissolveEffect());
    }

    IEnumerator DissolveEffect()
    {
        // Let the broken pieces fall
        yield return new WaitForSeconds(dissolveDelay);

        float timer = 0f;

        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;

            float amount = Mathf.Lerp(
                0.64f,
                0f,
                timer / dissolveDuration
            );

            foreach (Renderer rend in renderers)
            {
                Material mat = rend.material;

                if (mat.HasProperty("_Cutoff"))
                {
                    mat.SetFloat(
                        "_Cutoff",
                        amount
                    );
                }
            }

            yield return null;
        }

        // Make sure it reaches 0
        foreach (Renderer rend in renderers)
        {
            Material mat = rend.material;

            if (mat.HasProperty("_Cutoff"))
            {
                mat.SetFloat("_Cutoff", 0f);
            }
        }

        // Destroy after dissolve
        Destroy(gameObject);
    }
}