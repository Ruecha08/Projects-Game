using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    private Vector2 initialPosition;
    private int moveDirection = 1;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // ทำให้ศัตรูเดินไป-มา
        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);

        // ตรวจสอบระยะเพื่อเปลี่ยนทิศทาง
        if (Mathf.Abs(transform.position.x - initialPosition.x) >= patrolRange)
        {
            moveDirection *= -1;
            Flip(); // กลับด้านศัตรู
        }
    }

    void Flip()
    {
        // สลับทิศทางการหันหน้า
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
}