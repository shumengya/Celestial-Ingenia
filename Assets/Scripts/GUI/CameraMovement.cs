using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    // 摄像机移动的速度
    public float moveSpeed = 5f;

    // 相机缩放的速度
    public float zoomSpeed = 5f;

    // 相机最小和最大的缩放范围（对于透视相机是视野，对于正交相机是正交大小）
    public float minZoom = 10f;
    public float maxZoom = 60f;

    // 震动相关参数
    private bool isShaking = false;
    private Vector3 originalPosition;

    private Camera mainCamera;

    void Start()
    {
        // 获取主相机组件
        mainCamera = GetComponent<Camera>();
        originalPosition = transform.position;
    }

    void Update()
    {
        // 如果正在震动，则跳过普通的移动操作
        if (isShaking)
            return;
            
        // 获取水平和垂直输入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 根据输入计算移动方向
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);

        // 根据移动方向和速度更新摄像机的位置
        transform.Translate(movement * moveSpeed * Time.deltaTime);
        originalPosition = transform.position;

        // 获取鼠标滚轮的输入
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            if (mainCamera.orthographic)
            {
                // 正交相机的缩放处理
                mainCamera.orthographicSize -= scrollInput * zoomSpeed;
                // 限制正交大小在最小和最大缩放范围之间
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                // 透视相机的缩放处理
                mainCamera.fieldOfView -= scrollInput * zoomSpeed;
                // 限制视野在最小和最大缩放范围之间
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);
            }
        }
    }

    /// <summary>
    /// 触发相机震动效果
    /// </summary>
    /// <param name="duration">震动持续时间（秒）</param>
    /// <param name="magnitude">震动幅度</param>
    public void ShakeCamera(float duration = 0.5f, float magnitude = 0.5f)
    {
        // 如果未处于震动状态，开始震动协程
        if (!isShaking)
        {
            StartCoroutine(ShakeCameraCoroutine(duration, magnitude));
        }
    }

    private IEnumerator ShakeCameraCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 计算随机偏移
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // 应用偏移到相机位置
            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 震动结束，恢复原始位置
        transform.position = originalPosition;
        isShaking = false;
    }
}