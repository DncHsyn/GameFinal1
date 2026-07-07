using UnityEngine;
public class BasketController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [Header("Movement")]
    [SerializeField] private float minX = -2.6f;
    [SerializeField] private float maxX = 2.6f;
    [SerializeField] private float moveSpeed = 20f;
    private Vector3 targetPosition;
    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
             targetPosition = transform.position;
    }
    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive)
            return;
        bool hasInput = false;
        Vector2 screenPosition = Vector2.zero;
        if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
            hasInput = true;
        }
        else if (Input.GetMouseButton(0))
        {
            screenPosition = Input.mousePosition;
            hasInput = true;
        }
        if (hasInput)
        {
            float cameraDistance = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, cameraDistance)
            );
            float clampedX = Mathf.Clamp(worldPosition.x, minX, maxX);
            targetPosition = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}