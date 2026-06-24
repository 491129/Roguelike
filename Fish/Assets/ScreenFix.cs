using UnityEngine;

public class ScreenFix : MonoBehaviour
{
    void Start()
    {
        // 设置为全屏窗口模式（不独占全屏，仍可 Alt+Tab）
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}