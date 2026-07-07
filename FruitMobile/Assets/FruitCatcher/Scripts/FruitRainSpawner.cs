using UnityEngine;

public class FruitRainSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private GameObject bananaPrefab;
    [SerializeField] private GameObject firePrefab;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 0.9f;
    [SerializeField] private float spawnXPadding = 0.8f;
    [SerializeField] private float spawnYViewport = 1.1f;
    [SerializeField] private float destroyY = -6f;
    [SerializeField] private float appleFallSpeed = 3.6f;
    [SerializeField] private float fireFallSpeed = 4.2f;

    private Camera m_MainCamera;
    private float m_NextSpawnTime;

    private void Awake()
    {
        m_MainCamera = Camera.main;
        EnsurePrefabs();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive)
            return;

        if (applePrefab == null || firePrefab == null)
            return;

        if (Time.time < m_NextSpawnTime)
            return;

        SpawnRandomItem();
        m_NextSpawnTime = Time.time + spawnInterval;
    }

    public void OnGameStarted()
    {
        EnsurePrefabs();
        m_NextSpawnTime = Time.time;
    }

    private void EnsurePrefabs()
    {
        if (applePrefab == null)
            applePrefab = FindSceneTemplate("apple", "elma");

        if (bananaPrefab == null)
            bananaPrefab = FindSceneTemplate("banana", "muz");

        if (firePrefab == null)
            firePrefab = FindSceneTemplate("fire");
    }

    private GameObject FindSceneTemplate(params string[] nameParts)
    {
        SpriteRenderer[] spriteRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer == null)
                continue;

            string lowerName = spriteRenderer.gameObject.name.ToLowerInvariant();
            foreach (string part in nameParts)
            {
                if (!lowerName.Contains(part))
                    continue;

                return spriteRenderer.gameObject;
            }
        }

        return null;
    }

    private void SpawnRandomItem()
    {
        float randomValue = Random.value;
        bool canSpawnBanana = bananaPrefab != null;
        bool spawnApple = randomValue < 0.45f;
        bool spawnBanana = canSpawnBanana && randomValue >= 0.45f && randomValue < 0.75f;

        GameObject prefab = spawnApple
            ? applePrefab
            : (spawnBanana ? bananaPrefab : firePrefab);

        if (prefab == null)
            return;

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject itemObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        itemObject.name = spawnApple ? "AppleItem" : (spawnBanana ? "BananaItem" : "FireItem");
        itemObject.SetActive(true);

        FallingItem fallingItem = itemObject.GetComponent<FallingItem>();
        if (fallingItem == null)
            fallingItem = itemObject.AddComponent<FallingItem>();

        fallingItem.Configure(
            (spawnApple || spawnBanana) ? FallingItem.ItemKind.Apple : FallingItem.ItemKind.Fire,
            (spawnApple || spawnBanana) ? appleFallSpeed : fireFallSpeed,
            destroyY
        );
    }

    private Vector3 GetSpawnPosition()
    {
        if (m_MainCamera == null)
            m_MainCamera = Camera.main;

        if (m_MainCamera == null)
            return new Vector3(0f, 6f, 0f);

        float cameraDistance = Mathf.Abs(m_MainCamera.transform.position.z);
        float left = m_MainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraDistance)).x;
        float right = m_MainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraDistance)).x;
        float y = m_MainCamera.ViewportToWorldPoint(new Vector3(0f, spawnYViewport, cameraDistance)).y;
        float x = Random.Range(left + spawnXPadding, right - spawnXPadding);
        return new Vector3(x, y, 0f);
    }
}
