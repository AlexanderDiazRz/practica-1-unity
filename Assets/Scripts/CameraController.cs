using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetive;
    public float cameraSpeed = 0.08f;
    public Vector3 movement = new Vector3(0f, 3.1f, -10f);
    [Range(0f, 1f)] public float verticalFollow = 0.15f;
    public float verticalDeadZone = 1.5f;

    private float initialPlayerY;
    private float initialCameraY;

    void Start()
    {
        if (objetive == null) return;

        initialPlayerY = objetive.position.y;
        initialCameraY = transform.position.y;
    }

    void LateUpdate()
    {
        if (objetive == null) return;

        float playerRise = Mathf.Max(0f, objetive.position.y - initialPlayerY - verticalDeadZone);
        float positionY = initialCameraY + playerRise * verticalFollow;
        Vector3 positionTo = new Vector3(objetive.position.x + movement.x, positionY, movement.z);
        Vector3 positionSlow = Vector3.Lerp(transform.position, positionTo, cameraSpeed);
        transform.position = positionSlow;
    }
}
