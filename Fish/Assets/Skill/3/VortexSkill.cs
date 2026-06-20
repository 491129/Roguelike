using System.Collections;
using UnityEngine;

public class VortexSkill : MonoBehaviour
{
    public static VortexSkill Instance { get; private set; }

    [Header("基础属性")]
    [SerializeField] private float baseCooldown = 12f;         // 初始冷却
    [SerializeField] private float baseRadius = 5f;            // 吸附半径
    [SerializeField] private float baseDuration = 6f;          // 持续时间
    [SerializeField] private float pullSpeed = 3f;             // 拉扯速度
    [SerializeField] private GameObject vortexEffectPrefab;    // 漩涡特效预制体

    // 可升级的当前值
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentRadius;
    [HideInInspector] public float currentDuration;

    private SkillButton boundButton;   // 绑定的技能按钮

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentCooldown = baseCooldown;
        currentRadius = baseRadius;
        currentDuration = baseDuration;
    }

    /// <summary>
    /// 由管理器调用，绑定对应的技能按钮
    /// </summary>
    public void BindButton(SkillButton btn) => boundButton = btn;

    /// <summary>
    /// 按钮点击时调用
    /// </summary>
    public void Use()
    {
        if (boundButton == null) return;
        boundButton.StartCooldown();
        StartCoroutine(VortexRoutine());
    }

    IEnumerator VortexRoutine()
    {
        // 在屏幕中心附近随机位置生成漩涡
        Vector3 center = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        center.z = 0;
        Vector2 offset = Random.insideUnitCircle * 3f;
        Vector3 spawnPos = center + new Vector3(offset.x, offset.y, 0);

        // 生成特效
        GameObject effect = null;
        if (vortexEffectPrefab != null)
            effect = Instantiate(vortexEffectPrefab, spawnPos, Quaternion.identity);

        float timer = 0f;
        while (timer < currentDuration)
        {
            // 吸附范围内的鱼（不包含冰冻状态或已死亡的鱼）
            Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPos, currentRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Fish"))
                {
                    FishAttrbute fish = hit.GetComponent<FishAttrbute>();
                    if (fish != null && !fish.isDead && !Fishself.IsFrozen)
                    {
                        Vector2 fishPos = fish.transform.position;
                        Vector2 direction = ((Vector2)spawnPos - fishPos).normalized;
                        fish.transform.Translate(direction * pullSpeed * Time.deltaTime, Space.World);
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 漩涡结束，销毁特效，启动冷却
        if (effect != null) Destroy(effect);
        boundButton.StartCooldown();
    }

    // ----- 升级方法，由商城商品调用 -----
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        boundButton?.SetCooldownDuration(currentCooldown);
        Debug.Log($"电磁漩涡冷却减少 {seconds} 秒，当前冷却：{currentCooldown}");
    }

    public void IncreaseRadius(float multiplier)
    {
        currentRadius *= multiplier;
        Debug.Log($"电磁漩涡范围增加，当前半径：{currentRadius}");
    }

    public void IncreaseDuration(float extraSeconds)
    {
        currentDuration += extraSeconds;
        Debug.Log($"电磁漩涡持续时间增加 {extraSeconds} 秒，当前持续：{currentDuration}");
    }
}