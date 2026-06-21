using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    class ActiveBoss
    {
        public GameObject obj;
        public string bossType;
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        float elapsedMinutes = timerUI.TimeElapsed / 60f;

        for (int i = 0; i < battleStartMinutes.Length; i++)
        {
            if (battleTriggered[i]) continue;
            if (elapsedMinutes >= battleStartMinutes[i])
            {
                battleTriggered[i] = true;
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

        GameObject bossPrefab;
        if (index == 2)   // 最终Boss
            bossPrefab = finalBossPrefab;
        else
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
}