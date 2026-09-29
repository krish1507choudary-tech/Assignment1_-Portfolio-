using UnityEngine;

public class Trisl : MonoBehaviour
{
    [Header("Trail Settings")]
    public float meshRefreshRate = 0.05f;
    public float meshDestroyDelay = 0.6f;
    public float ghostMoveSpeed = 2f;

    [Header("Trail Opacity")]
    public float nearOpacity = 0.6f;
    public float farOpacity = 0.05f;
    public float fadeDistance = 4f;

    [Header("Trail Material")]
    public Material trailMaterial;

    private SkinnedMeshRenderer[] renderers;
    private float timer;
    private bool trailActive;

    void Start()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (!trailActive)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            CreateGhost();
            timer = meshRefreshRate;
        }
    }

    public void StartTrail()
    {
        trailActive = true;
        timer = 0f;
    }

    public void StopTrail()
    {
        trailActive = false;
        timer = 0f;
    }

    void CreateGhost()
    {
        foreach (SkinnedMeshRenderer source in renderers)
        {
            GameObject ghost = new GameObject("Trail Ghost");

            ghost.transform.position = source.transform.position;
            ghost.transform.rotation = source.transform.rotation;
            ghost.transform.localScale = source.transform.localScale;

            Mesh mesh = new Mesh();

            source.BakeMesh(mesh);

            MeshFilter filter =
                ghost.AddComponent<MeshFilter>();

            filter.sharedMesh = mesh;

            MeshRenderer meshRenderer =
                ghost.AddComponent<MeshRenderer>();

            Material ghostMaterial =
                new Material(trailMaterial);

            meshRenderer.material = ghostMaterial;

            Color color = ghostMaterial.color;
            color.a = nearOpacity;
            ghostMaterial.color = color;

            TrailGhostMovement movement =
                ghost.AddComponent<TrailGhostMovement>();

            movement.speed = ghostMoveSpeed;
            movement.player = transform;
            movement.material = ghostMaterial;
            movement.nearOpacity = nearOpacity;
            movement.farOpacity = farOpacity;
            movement.fadeDistance = fadeDistance;

            StartCoroutine(FadeGhost(
                ghost,
                ghostMaterial,
                meshDestroyDelay
            ));
        }
    }

    System.Collections.IEnumerator FadeGhost(
        GameObject ghost,
        Material material,
        float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            if (ghost == null)
                yield break;

            yield return null;
        }

        if (material != null)
            Destroy(material);

        if (ghost != null)
            Destroy(ghost);
    }
}


public class TrailGhostMovement : MonoBehaviour
{
    public float speed = 2f;

    public Transform player;
    public Material material;

    public float nearOpacity = 0.6f;
    public float farOpacity = 0.05f;
    public float fadeDistance = 4f;

    void Update()
    {
        transform.position +=
            -transform.forward *
            speed *
            Time.deltaTime;

        if (player == null || material == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        float fade =
            Mathf.Clamp01(
                distance / fadeDistance
            );

        float alpha =
            Mathf.Lerp(
                nearOpacity,
                farOpacity,
                fade
            );

        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }
}