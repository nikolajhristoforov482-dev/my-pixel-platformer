using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    // Жесткое и синхронное следование кадр в кадр в LateUpdate
    void LateUpdate()
    {
        if (target == null) return;

        // Камера железно стоит на позиции игрока с заданным смещением
        transform.position = target.position + offset;
    }
}
