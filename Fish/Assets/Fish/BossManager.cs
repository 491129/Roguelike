using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [Header("出生点设置")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform centerPoint;

    [Header("引用")]
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private Cannon cannon;
    [SerializeField] private ALLCannon AllCannon;
    [SerializeField] private GameObject warningUI;
    [Header("预警 UI（三张图）")]
    [SerializeField] private Image warningImage1;       // 第一张图
    [SerializeField] private Image warningImage2;       // 第二张图
    [SerializeField] private Image warningImage3;       // 第三张图
    [SerializeField] private Text warningText;          // 第二张图上的文字（子物体）

    [Header("设置")]
    [SerializeField] private float warningDuration = 30f;
    [SerializeField] private float bossDuration = 60f;

    private float[] triggerPercents = { 0.05f, 0.50f, 0.99f };
    private bool[] triggered = new bool[3];
    private List<ActiveBoss> activeBosses = new List<ActiveBoss>();

    [Header("关闭技能")]
   // private SkillButtonManager skillButtonManager;
    private SkillButton disabledButton;
    [Header("Boss 预制体")]
    [SerializeField] private GameObject[] normalBossPrefabs;   // 4个常规Boss
    [SerializeField] private GameObject finalBossPrefab;        // 章鱼

    [Header("Boss 阶段时间 (分钟)")]
    [SerializeField] private float[] battleStartMinutes = new float[] { 4f, 8.5f, 16f };
    private bool[] battleTriggered = new bool[3];   // 3个阶段

    [Header("最终Boss鱼群生成")]
    public Transform fishParent;               // 与 fish.cs 中的 fishParent 相同
    public GameObject[] commonFishPrefabs;     // 拖入几种普通鱼预制体
    [Header("最终Boss出生点")]
    [SerializeField] private Transform finalBossSpawnPoint;   // 拖入屏幕上方中间的出生点
    [Header("最终Boss鱼群出生点")]
    [SerializeField] private Transform[] finalBossFishSpawnPoints;  // 拖入场景中放在屏幕左边的空物体

    class ActiveBoss
    {
        public GameObject obj;
        public string bossType;
    }

    void Awake()
    {
        Instance = this;
    }

    //void Update()
    //{
    //    float elapsedMinutes = timerUI.TimeElapsed / 60f;

    //    for (int i = 0; i < battleStartMinutes.Length; i++)
    //    {
    //        if (battleTriggered[i]) continue;
    //        if (elapsedMinutes >= battleStartMinutes[i])
    //        {
    //            battleTriggered[i] = true;
    //            StartCoroutine(FinalBossSequence());
    //        }
    //    }
    //}
    void Update()
    {
        float elapsedMinutes = timerUI.TimeElapsed / 60f;
        for (int i = 0; i < battleStartMinutes.Length; i++)
        {
            if (battleTriggered[i]) continue;
            if (elapsedMinutes >= battleStartMinutes[i])
            {
                battleTriggered[i] = true;
                if (i == battleStartMinutes.Length - 1)   // 最后一个元素是最终Boss
                    StartCoroutine(FinalBossSequence());
                else
                    StartCoroutine(BossSequence(i));
            }
        }
    }
    IEnumerator BossSequence(int index)
    {
        Debug.Log($"[BossManager] 开始生成 Boss，index={index}");
        yield return StartCoroutine(WarningSequence(warningDuration));
        yield return new WaitForSeconds(3f);
        GameObject bossPrefab;
        bossPrefab = normalBossPrefabs[Random.Range(0, normalBossPrefabs.Length)];

        var (pos, dir) = GetRandomSpawnData();
        GameObject bossObj = Instantiate(bossPrefab, pos, Quaternion.identity);
        Boss boss = bossObj.GetComponent<Boss>();

        if (boss != null)
        {
            boss.Init(dir);

            boss.SetCenter(centerPoint);   // 传递中心点，让其知道何时停留
            // 应用 Debuff
            switch (boss.bossType)
            {
                case "FreezeTurtle":
                    cannon.ApplyDebuff("Freeze");
                    break;
                case "KillerWhale":
                    SkillButtonManager sm = SkillButtonManager.Instance;
                    if (sm == null) break;

                    // 获取当前已激活的技能按钮列表（只随机禁用一个已激活的）
                    List<SkillButton> activeButtons = new List<SkillButton>();
                    for (int i = 0; i < sm.skillButtons.Length; i++)
                    {
                        if (sm.skillButtons[i].IsActive)  // 需要在 SkillButton 中公开 IsActive
                            activeButtons.Add(sm.skillButtons[i]);
                    }

                    if (activeButtons.Count > 0)
                    {
                        int randIndex = Random.Range(0, activeButtons.Count);
                        SkillButton targetBtn = activeButtons[randIndex];
                        targetBtn.DisableByBoss();

                        // 保存这个引用，以便 Boss 死亡时恢复
                        disabledButton = targetBtn; // 需要在 BossManager 中定义 private SkillButton disabledButton;
                    }
                    break;
                    
            }
            activeBosses.Add(new ActiveBoss { obj = bossObj, bossType = boss.bossType });
        }
        BGMPlayer.PlayBoss();

        // 等待 Boss 持续时间或直到被击败
        float timer = 0f;
        while (timer < bossDuration && bossObj != null)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 如果 Boss 还活着，移除 Debuff 并销毁
        if (bossObj != null)
        {
            RemoveBossDebuff(bossObj);
           // Destroy(bossObj);
        }
        BGMPlayer.PlayNormal();
        Debug.Log($"[BossManager] Boss 生成位置：{pos}，方向：{dir}");
    }
    IEnumerator FinalBossSequence()
    {
        yield return StartCoroutine(WarningSequence(warningDuration));
        yield return new WaitForSeconds(3f);
        Vector3 spawnPos = finalBossSpawnPoint.position;
        Vector2 dir = Vector2.down;  // 最终Boss强制向下

        GameObject bossObj = Instantiate(finalBossPrefab, spawnPos, Quaternion.identity);
        Boss boss = bossObj.GetComponent<Boss>();
        if (boss != null)
        {
            boss.Init(dir);
            boss.SetCenter(centerPoint);
            // 应用Debuff等（章鱼没有Debuff，可跳过）
            // 动态赋值鱼群出生点（关键！）
            if (finalBossFishSpawnPoints.Length > 0)
                boss.fishSpawnPoints = finalBossFishSpawnPoints;

        }

        BGMPlayer.PlayBoss();

        // 等待Boss被击败或离开
        while (bossObj != null && boss != null && !boss.bossDefeated)
            yield return null;

        BGMPlayer.PlayNormal();
    }
    /// <summary>
    /// 当 Boss 被玩家击败时调用，立即移除负面效果
    /// </summary>
    public void OnBossDefeated(Boss defeatedBoss)
    {
        if (defeatedBoss == null) return;
        RemoveBossDebuff(defeatedBoss.gameObject);
        activeBosses.RemoveAll(b => b.obj == defeatedBoss.gameObject);
    }

    private void RemoveBossDebuff(GameObject bossObj)
    {
        if (bossObj == null) return;
        Boss boss = bossObj.GetComponent<Boss>();
        if (boss == null) return;
        switch (boss.bossType)
        {
            case "FreezeTurtle":
                cannon.RemoveDebuff("Freeze");
                break;
            case "KillerWhale":
                if (disabledButton != null)
                {
                    disabledButton.RestoreByBoss();
                    disabledButton = null;
                }
                // 如果你的三头鲨还有其他效果（如禁用升级按钮），也在这里恢复
                // AllCannon.EnableButtonsByBoss();
                break;
        }
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
        Vector2 dir = (center - spawnPos).normalized;
        return (spawnPos, dir);
    }
    public void SpawnFishWave(Transform[] points, int count)
    {
        Debug.Log($"[BossManager] SpawnFishWave count={count}");
        StartCoroutine(SpawnWaveRoutine(points, count));
    }

    IEnumerator SpawnWaveRoutine(Transform[] points, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"[BossManager] 生成第 {i + 1} 条鱼");
            if (points == null || points.Length == 0) break;
            Transform sp = points[Random.Range(0, points.Length)];
            if (sp == null) continue;
            if (commonFishPrefabs.Length == 0) continue;
            GameObject prefab = commonFishPrefabs[Random.Range(0, commonFishPrefabs.Length)];
            if (prefab == null) continue;

            GameObject fishObj = Instantiate(prefab, sp.position, Quaternion.identity);
            fishObj.transform.SetParent(fishParent);

            Fishself fishMove = fishObj.GetComponent<Fishself>();
            fishMove?.SetDirection(Vector2.right);

            yield return new WaitForSeconds(0.15f);
        }
    }
    IEnumerator WarningSequence(float totalDuration)
    {
        float segmentTime = totalDuration / 3f;
        // 初始状态：全部隐藏，透明度为0
        warningImage1.gameObject.SetActive(true);
        warningImage2.gameObject.SetActive(true);
        warningImage3.gameObject.SetActive(true);
        warningText.gameObject.SetActive(true);

        warningImage1.color = new Color(1, 1, 1, 0);
        warningImage2.color = new Color(1, 1, 1, 0);
        warningImage3.color = new Color(1, 1, 1, 0);
        warningText.color = new Color(1, 1, 1, 0);

        // 第一阶段：第一张图淡入（0.5秒） -> 持续1秒 -> 淡出（0.5秒）
        warningImage1.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(1.2f);   // 0.5淡入 + 1秒停留
        warningImage1.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // 第二阶段：第二张图 + 文字淡入（0.5秒）
        warningImage2.DOFade(1f, 0.5f);
        warningText.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(1.5f);     // 显示1秒

        // 第三阶段：第二张图淡出（0.5秒），同时第三张图淡入（0.5秒）
        warningImage2.DOFade(0f, 0.5f);
        warningText.DOFade(0f, 0.5f);
        warningImage3.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.8f);     // 第三张图展示1秒

        // 全部隐藏，准备下一次预警
        warningImage3.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        warningImage1.gameObject.SetActive(false);
        warningImage2.gameObject.SetActive(false);
        warningImage3.gameObject.SetActive(false);
        warningText.gameObject.SetActive(false);
    }
}