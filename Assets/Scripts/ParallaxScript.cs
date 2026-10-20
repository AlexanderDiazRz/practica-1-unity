using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector2 parallaxMultiplier = new Vector2(0.15f, 0.05f);

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform != null) lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(
            deltaMovement.x * parallaxMultiplier.x,
            deltaMovement.y * parallaxMultiplier.y,
            0f
        );
        lastCameraPosition = cameraTransform.position;
    }
}
