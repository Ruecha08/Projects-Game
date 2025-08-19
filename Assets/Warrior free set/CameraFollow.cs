using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ตัวแปรที่ใช้สำหรับกล้อง
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // เพิ่มตัวแปรสำหรับ PlayerMovement เพื่อเข้าถึงข้อมูลการเคลื่อนที่
    public PlayerMovement playerMovement;

    void FixedUpdate()
    {
        // หากไม่มีเป้าหมาย (target) ให้หยุดการทำงานทันที
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Target is not assigned!");
            return;
        }

        // ตรวจสอบว่า playerMovement ถูกกำหนดค่าแล้วและ Player กำลัง Dash อยู่
        if (playerMovement != null && playerMovement.IsDashing())
        {
            // หากกำลัง Dash อยู่ ให้กล้องไม่เคลื่อนที่ตาม
            return;
        }

        // คำนวณตำแหน่งที่ต้องการให้กล้องอยู่
        Vector3 desiredPosition = target.position + offset;

        // เคลื่อนที่กล้องไปยังตำแหน่งที่ต้องการอย่างนุ่มนวล
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // ทำให้กล้องหันหน้าไปมองที่ Player
        transform.LookAt(target);
    }
}
