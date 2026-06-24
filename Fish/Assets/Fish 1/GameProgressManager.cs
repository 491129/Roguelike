using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [SerializeField] private TimerUI timerUI;   // 你的倒计时脚本
    private float scaleFactor = 8f / 17.5f;
    private (float startMin, int count, float interval)[] stages;

    void Awake()
    {
        Instance = this;
        InitializeStages();
    }
    // 阶段定义 (开始时间分钟，每波鱼数量，刷新间隔秒)
    void InitializeStages()
    {
        stages = new (float, int, float)[]
        {
            // 原数据乘以 scaleFactor，开始分钟和间隔都缩放
            (0f,               18, 1.5f * scaleFactor),
            (1f * scaleFactor, 16, 1.8f * scaleFactor),
            (3f * scaleFactor, 14, 2f   * scaleFactor),
            (3.5f * scaleFactor, 12, 2.2f * scaleFactor),
            (4f * scaleFactor, 8,  2.5f * scaleFactor),
            (4.5f * scaleFactor, 10, 2.3f * scaleFactor),
            (5f * scaleFactor, 14, 2f   * scaleFactor),
            (8f * scaleFactor, 12, 2.2f * scaleFactor),
            (8.5f * scaleFactor, 8,  2.5f * scaleFactor),
            (9f * scaleFactor, 10, 2.3f * scaleFactor),
            (9.5f * scaleFactor, 16, 1.8f * scaleFactor),
            (14f * scaleFactor, 14, 2f   * scaleFactor),
            (15.5f * scaleFactor, 10, 2.5f * scaleFactor),
            (16f * scaleFactor, 6,  3f   * scaleFactor)
        };
    }


    /// <summary>
    /// 根据当前游戏时间返回 (每波数量, 刷新间隔)
    /// </summary>
    public (int count, float interval) GetCurrentWaveParams()
    {
        float minutes = timerUI.TimeElapsed / 60f;
        for (int i = stages.Length - 1; i >= 0; i--)
        {
            if (minutes >= stages[i].startMin)
                return (stages[i].count, stages[i].interval);
        }
        return (10, 2f); // fallback
    }
}