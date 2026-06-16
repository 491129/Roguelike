using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [SerializeField] private TimerUI timerUI;   // 你的倒计时脚本

    // 阶段定义 (开始时间分钟，每波鱼数量，刷新间隔秒)
    private (float startMin, int count, float interval)[] stages = new (float, int, float)[]
    {
        (0f,    18, 1.5f),
        (1f,    16, 1.8f),
        (3f,    14, 2f),
        (3.5f,  12, 2.2f),   // BOSS1预警 3:30=3.5
        (4f,    8,  2.5f),   // BOSS1战中
        (4.5f,  10, 2.3f),   // BOSS1收尾
        (5f,    14, 2f),
        (8f,    12, 2.2f),   // BOSS2预警
        (8.5f,  8,  2.5f),
        (9f,    10, 2.3f),
        (9.5f,  16, 1.8f),
        (14f,   14, 2f),
        (15.5f, 10, 2.5f),   // 最终预警 15:30=15.5
        (16f,   6,  3f)      // 最终BOSS
    };

    void Awake() => Instance = this;

    /// <summary>
    /// 根据当前游戏时间返回 (每波数量, 刷新间隔)
    /// </summary>
    public (int count, float interval) GetCurrentWaveParams()
    {
        float minutes = timerUI.TimeElapsed / 60f;   // TimerUI 需要暴露 TimeElapsed（总秒数）
        for (int i = stages.Length - 1; i >= 0; i--)
        {
            if (minutes >= stages[i].startMin)
                return (stages[i].count, stages[i].interval);
        }
        return (10, 2f); // fallback
    }
}