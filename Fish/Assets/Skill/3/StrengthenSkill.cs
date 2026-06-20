using System.Collections;
using UnityEngine;

public class StrengthenSkill : MonoBehaviour
{
    public static StrengthenSkill Instance { get; private set; }

    [Header("基础数值")]
    [SerializeField] private float baseCooldown = 10f;       // 冷却时间
    [SerializeField] private float baseDuration = 8f;        // 持续时间
    [SerializeField] private float baseBoostRate = 1.5f;     // 强化倍率

    // 当前数值（可升级）
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentDuration;
    [HideInInspector] public float currentBoostRate;

    private SkillButton boundButton;
    private bool isActive = false;

    // 保存原始值，用于恢复
    private float originalCannonMultiplier;
    private float originalCatchMultiplier;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
        currentBoostRate = baseBoostRate;
    }

    public void BindButton(SkillButton btn) => boundButton = btn;

    public void Use()
    {
        if (isActive || boundButton == null) return;

        isActive = true;
        boundButton.StartCooldown();   // 立即冷却，按钮变灰
        StartCoroutine(BuffRoutine());
    }

    IEnumerator BuffRoutine()
    {
        // 保存当前加成值
        if (FishAttrbute.Instance != null)
            originalCatchMultiplier = FishAttrbute.CatchRateMultiplier;
        if (Cannon.Instance != null)
            originalCannonMultiplier = Cannon.Instance.attackSpeedMultiplier;

        // 应用强化
        if (FishAttrbute.Instance != null)
            FishAttrbute.CatchRateMultiplier *= currentBoostRate;
        if (Cannon.Instance != null)
            Cannon.Instance.attackSpeedMultiplier *= currentBoostRate;

        Debug.Log($"强化启动：持续 {currentDuration} 秒，倍率 x{currentBoostRate}");

        yield return new WaitForSeconds(currentDuration);

        // 恢复原值
        if (FishAttrbute.Instance != null)
            FishAttrbute.CatchRateMultiplier = originalCatchMultiplier;
        if (Cannon.Instance != null)
            Cannon.Instance.attackSpeedMultiplier = originalCannonMultiplier;

        Debug.Log("强化结束，状态恢复");

        isActive = false;
        // 冷却已在开始时就启动，此处无需再次操作
    }

    // ----- 升级方法，供商城调用 -----
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        boundButton?.SetCooldownDuration(currentCooldown);
        Debug.Log($"强化冷却减少 {seconds} 秒，当前冷却：{currentCooldown}");
    }

    public void IncreaseDuration(float extraSeconds)
    {
        currentDuration += extraSeconds;
        Debug.Log($"强化持续时间增加 {extraSeconds} 秒，当前持续：{currentDuration}");
    }

    public void IncreaseBoostRate(float delta)
    {
        currentBoostRate += delta;
        Debug.Log($"强化倍率提升 {delta}，当前倍率：{currentBoostRate}");
    }
}