using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("出生点设置")]
    [SerializeField] private Transform[] spawnPoints;   // 拖入所有出生点空物体
    [SerializeField] private Transform centerPoint;

    [Header("引用")]
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private Cannon cannon;               // 拖入炮台脚本
    [SerializeField] private ALLCannon AllCannon;   // 拖入挂有 ALLCannon 的物体
    [SerializeField] private GameObject warningUI;         // 预警 UI 物体
    [SerializeField] private GameObject[] bossPrefabs;     // 四种 Boss 预制体

    [Header("设置")]
    [SerializeField] private float warningDuration = 30f;
    [SerializeField] private float bossDuration = 60f;

    // 内部状态
    private float[] triggerPercents = { 0.05f, 0.50f, 0.99f }; // 剩余百分比
    private bool[] triggered = new bool[3];                // 是否已进入预警流程

    private List<ActiveBoss> activeBosses = new List<ActiveBoss>();

    class ActiveBoss
    {
        public GameObject obj;
        public string bossType;
    }
    void Update()
    {
        float elapsed = timerUI.TimeElapsed;
        float total = timerUI.TotalTime;

        for (int i = 0; i < triggerPercents.Length; i++)
        {
            if (triggered[i]) continue;

            float spawnTime = total * (1f - triggerPercents[i]);   // Boss 出现的时间点
            float warnTime = spawnTime - warningDuration;          // 预警开始的时间点

            if (elapsed >= warnTime)
            {
                triggered[i] = true;
                StartCoroutine(BossSequence(i));
            }
        }
    }
    IEnumerator BossSequence(int index)
    {
        // 预警动画...
        warningUI.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        warningUI.SetActive(false);

        // 随机获取出生点和方向
        var (pos, dir) = GetRandomSpawnData();

        // 随机选一个 Boss 预制体
        GameObject bossPrefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
        GameObject bossObj = Instantiate(bossPrefab, pos, Quaternion.identity);
        Boss boss = bossObj.GetComponent<Boss>();

        // 初始化 Boss，赋予方向
        if (boss != null)
        {
            boss.Init(dir);  
        }
        // 3. 应用 Debuff
        if (boss != null)
        {
            switch (boss.bossType)
            {
                case "FreezeTurtle":
                    cannon.ApplyDebuff("Freeze");
                    break;
                case "ThreeHeadShark":
                    AllCannon.DisableButtonsByBoss();
                    break;
                    // 其他类型（速度快、难捕捉）可能无需 Debuff，仅靠预制体属性
            }
        }

        // 4. Boss 持续一段时间后游走
        yield return new WaitForSeconds(bossDuration);

        // 5. 移除 Boss 和 Debuff
        if (boss != null)
        {
            switch (boss.bossType)
            {
                case "FreezeTurtle":
                    cannon.RemoveDebuff("Freeze");
                    break;
                case "ThreeHeadShark":
                    AllCannon.EnableButtonsByBoss();
                    break;
            }
        }
        Destroy(bossObj);
    }
    private (Vector3 position, Vector2 direction) GetRandomSpawnData()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("没有设置 Boss 出生点！");
            return (Vector3.zero, Vector2.right);
        }

        Transform chosen = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = chosen.position;
        Vector3 center = centerPoint != null ? centerPoint.position : Vector3.zero;

        // 计算从出生点指向中心的方向（2D）
        Vector2 dir = (center - spawnPos).normalized;
        return (spawnPos, dir);
    }
}