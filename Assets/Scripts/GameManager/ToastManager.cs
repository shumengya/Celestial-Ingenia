using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;
    public GameObject toastPrefab;
    public Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowToast(string message, float duration)
    {
        GameObject toast = Instantiate(toastPrefab, canvas.transform);
        Text toastText = toast.GetComponentInChildren<Text>();
        if (toastText != null)
        {
            toastText.text = message;
        }

        // 设置 Toast 的位置到屏幕正中间的正下方
        RectTransform rectTransform = toast.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.1f); // 调整锚点位置
        rectTransform.anchorMax = new Vector2(0.5f, 0.1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;

        StartCoroutine(HideToast(toast, duration));
    }

    private IEnumerator HideToast(GameObject toast, float duration)
    {
        yield return new WaitForSeconds(duration - 0.5f); // 提前 0.5 秒开始渐变

        CanvasGroup canvasGroup = toast.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = toast.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(toast);
    }
}