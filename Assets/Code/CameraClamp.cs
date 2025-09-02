using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    public Transform player;      // Player ที่กล้องติดตาม
    public BoxCollider2D clampArea;  // พื้นที่เฉพาะที่กล้องจะถูกจำกัด

    private Vector3 offset;
    private float halfHeight;
    private float halfWidth;
    private Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        offset = transform.position - player.position;

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate() {
        Vector3 target = player.position + offset;

        float minX = clampArea.bounds.min.x + halfWidth;
        float maxX = clampArea.bounds.max.x - halfWidth;
        float minY = clampArea.bounds.min.y + halfHeight;
        float maxY = clampArea.bounds.max.y - halfHeight;

        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.y = Mathf.Clamp(target.y, minY, maxY);

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }
}
