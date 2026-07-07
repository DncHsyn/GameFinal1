using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public enum ItemKind
    {
        Apple = 0,
        Fire = 1
    }

    [Header("Gameplay")]
    [SerializeField] private ItemKind itemKind = ItemKind.Apple;
    [SerializeField] private float fallSpeed = 3.5f;
    [SerializeField] private float destroyY = -6f;

    private bool m_IsConsumed;

    private void Update()
    {
        if (m_IsConsumed)
            return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (transform.position.y <= destroyY)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCollect(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other.gameObject);
    }

    private void TryCollect(GameObject otherObject)
    {
        if (m_IsConsumed || GameManager.Instance == null || !GameManager.Instance.IsGameActive)
            return;

        if (otherObject.GetComponent<BasketController>() == null && !otherObject.CompareTag("Basket") && !otherObject.CompareTag("Player"))
            return;

        m_IsConsumed = true;

        if (itemKind == ItemKind.Apple)
            GameManager.Instance.AddScore(1);
        else
            GameManager.Instance.LoseLife(1);

        Destroy(gameObject);
    }

    public void Configure(ItemKind _itemKind, float _fallSpeed, float _destroyY)
    {
        itemKind = _itemKind;
        fallSpeed = _fallSpeed;
        destroyY = _destroyY;
    }

    private void ConfigureByName(object payload)
    {
        if (payload is not object[] values || values.Length < 3)
            return;

        string kindName = values[0] as string;
        float configuredSpeed = values[1] is float speed ? speed : fallSpeed;
        float configuredDestroyY = values[2] is float y ? y : destroyY;

        ItemKind configuredKind = kindName == "Fire" ? ItemKind.Fire : ItemKind.Apple;
        Configure(configuredKind, configuredSpeed, configuredDestroyY);
    }
}
