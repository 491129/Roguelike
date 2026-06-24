using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject wavePanel;          // 海浪 Panel
    [SerializeField] private GameObject wavePanel00;          // 海浪 Panel
    [SerializeField] private CanvasGroup waveCanvasGroup;   // 海浪 Panel 上的 CanvasGroup

    [Header("过渡设置")]
    [SerializeField] private float waveDuration = 5f;        // 海浪停留时间
    [SerializeField] private float fadeOutDuration = 1f;     // 从左往右消失的时间
    [SerializeField] private string sceneToLoad = "SampleScene";

    private AsyncOperation loadOp;
    void Start()
    {
        wavePanel.SetActive(false);
        wavePanel00.SetActive(false);
    }

    public void startGame()
    {
        // wavePanel.SetActive(true);
        StartCoroutine(Wave00());
        waveCanvasGroup.alpha = 1f;   // 确保完全显示

        // 异步加载场景
        loadOp = SceneManager.LoadSceneAsync(sceneToLoad);
        loadOp.allowSceneActivation = false;

        StartCoroutine(TransitionRoutine());
    }
    IEnumerator TransitionRoutine()
    {
        // 1. 海浪播放最少时间
        yield return new WaitForSeconds(waveDuration);

        // 2. 等待场景加载到 90%
        while (loadOp.progress < 0.9f)
            yield return null;

        // 3. 淡出动画
        yield return StartCoroutine(FadeOut());

        // 4. 激活新场景
        loadOp.allowSceneActivation = true;
    }
    IEnumerator Wave00()
    {
        wavePanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        wavePanel.SetActive(true);
    }
    IEnumerator FadeOut()
    {
        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            waveCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeOutDuration);
            yield return null;
        }
        waveCanvasGroup.alpha = endAlpha;
        wavePanel.SetActive(false);
    }

    public void exitGame()
    {
        // 在编辑器中停止播放，在构建后退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
