using UnityEngine;

public static class FruitCatcherBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeScene()
    {
        EnsureGameManager();
        EnsureBasketController();
        EnsureRainSpawner();
    }

    private static void EnsureGameManager()
    {
        GameManager existingManager = Object.FindFirstObjectByType<GameManager>();
        if (existingManager != null)
            return;

        GameObject managerObject = GameObject.Find("GameManager");
        if (managerObject == null)
            managerObject = new GameObject("GameManager");

        if (managerObject.GetComponent<AudioSource>() == null)
            managerObject.AddComponent<AudioSource>();

        managerObject.AddComponent<GameManager>();
    }

    private static void EnsureBasketController()
    {
        BasketController basketController = Object.FindFirstObjectByType<BasketController>();
        if (basketController != null)
            return;

        SpriteRenderer[] sprites = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (SpriteRenderer spriteRenderer in sprites)
        {
            if (spriteRenderer == null)
                continue;

            string lowerName = spriteRenderer.gameObject.name.ToLowerInvariant();
            if (!lowerName.Contains("basket") && lowerName != "2_0")
                continue;

            if (spriteRenderer.gameObject.GetComponent<BoxCollider2D>() == null)
                continue;

            spriteRenderer.gameObject.AddComponent<BasketController>();
            return;
        }
    }

    private static void EnsureRainSpawner()
    {
        if (Object.FindFirstObjectByType<FruitRainSpawner>() != null)
            return;

        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (manager != null)
        {
            manager.gameObject.AddComponent<FruitRainSpawner>();
            return;
        }

        GameObject spawnerObject = new GameObject("FruitRainSpawner");
        spawnerObject.AddComponent<FruitRainSpawner>();
    }
}
