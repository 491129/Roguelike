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
    [SerializeField] private GameObject[] bossPrefabs;

    [Header("设置")]
    [SerializeField] private float warningDuration = 30f;
    [SerializeField] private float bossDuration = 60f;

    private float[] triggerPercents = { 0.05f, 0.50f, 0.99f };
    private bool[] triggered = new bool[3];
    private List<ActiveBoss> activeBosses = new List<ActiveBoss>();

    [Header("关闭技能")]
    private SkillButtonManager skillButtonManager;
    public Sprite disSprite;
    private Sprite fabSprite;
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
        float elapsed = timerUI.TimeElapsed;
        float total = timerUI.TotalTime;

        for (int i = 0; i < triggerPercents.Length; i++)
        {
            if (triggered[i]) continue;
            float spawnTime = total * (1f - triggerPercents[i]);
            float warnTime = spawnTime - warningDuration;

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

        var (pos, dir) = GetRandomSpawnData();
        GameObject bossPrefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
        GameObject bossObj = Instantiate(bossPrefab, pos, Quaternion.identity);
        Boss boss = bossObj.GetComponent<Boss>();

        if (boss != null)
        {
            boss.Init(dir);

            // 应用 Debuff
            switch (boss.bossType)
            {
                case "FreezeTurtle":
                    cannon.ApplyDebuff("Freeze");
                    break;
                case "ThreeHeadShark":
                    //AllCannon.DisableButtonsByBoss();
                    int buttonLength= skillButtonManager.skillButtons.Length;
                    int DisableNum=Random.Range(0,buttonLength);
                    fabSprite = skillButtonManager.skillButtons[DisableNum].activeSprite;
                    skillButtonManager.skillButtons[DisableNum].Activate(disSprite);
                   
                    break;
            }

            // 修正：字段名 bossType，而不是 type
            activeBosses.Add(new ActiveBoss { obj = bossObj, bossType = boss.bossType });
        }

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
            Destroy(bossObj);
        }
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
            case "ThreeHeadShark":
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