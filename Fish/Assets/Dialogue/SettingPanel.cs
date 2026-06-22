using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button closeButton;   // 可选

    private void Start()
    {
        // 初始化滑块值为当前音量
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        exitButton.onClick.AddListener(OnExitClicked);
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    /// <summary>
    /// 显示设置面板（由设置按钮调用）
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        // 如果需要暂停游戏，可取消注释下一行
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 隐藏设置面板
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;   // 如果之前暂停了，恢复
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    private void OnExitClicked()
    {
        // 在编辑器中停止播放，在构建后退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}