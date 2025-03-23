using UnityEngine;

public class HealthBarRotationLock : MonoBehaviour
{
    // 锁定旋转的方式（可选：固定角度或面向摄像机）
    public bool lockRotation = true;
    public bool faceCamera = false; // 仅3D游戏需要
    private Quaternion originalRotation;

    private void Start()
    {
        // 记录初始旋转
        originalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (lockRotation)
        {
            // 强制锁定血条自身旋转
            if (faceCamera)
            {
                // 3D游戏：让血条始终面向摄像机
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
            else
            {
                // 保持初始旋转
                transform.rotation = originalRotation;
            }
        }
    }
}