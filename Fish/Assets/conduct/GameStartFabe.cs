using System.Collections;
using UnityEngine;

public class GameStartFabe : MonoBehaviour
{
    [SerializeField] private CanvasGroup waveCanvasGroup;  // 海浪 Panel 的 CanvasGroup
    [SerializeField] private float fadeDuration = 1.5f;    // 淡出时长

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            waveCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        waveCanvasGroup.alpha = 0f;
        // 可选：淡出结束后隐藏或销毁 Panel
        gameObject.SetActive(false);
    }
}