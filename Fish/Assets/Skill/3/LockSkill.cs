using System.Collections;
using UnityEngine;

public class LockSkill : MonoBehaviour
{
    public static LockSkill Instance { get; private set; }

    [Header("基础数值")]
    [SerializeField] private float baseCooldown = 12f;          // 冷却时间
    [SerializeField] private float baseDuration = 8f;           // 锁定持续时间
    [SerializeField] private GameObject lockEffectPrefab;       // 锁定特效（生成在目标身上）

    // 当前数值（可升级）
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentDuration;

    private SkillButton boundButton;
    private bool isLockMode = false;        // 是否处于锁定模式（未选择目标时也允许选择）
    private Component currentTarget;       // 当前锁定目标
    private GameObject currentEffect;       // 目标身上的特效实例
    private Coroutine durationCoroutine;    //目标死亡时停止持续计时
    public static bool BlockFireThisFrame { get; private set; }
    // 供子弹检查：是否有激活的锁定目标
    public Component LockedTarget => currentTarget;
    public bool IsLockModeActive => isLockMode;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Physics2D.queriesHitTriggers = true;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
    }

    public void BindButton(SkillButton btn) => boundButton = btn;

    /// <summary>
    /// 按钮点击时调用
    /// </summary>
    public void Use()
    {
        if (isLockMode || boundButton == null) return;
        ClearTarget();
        isLockMode = true;
        boundButton.StartCooldown();   // 立即冷却
        durationCoroutine = StartCoroutine(LockDurationRoutine());
    }

    // 锁定模式持续时间
    IEnumerator LockDurationRoutine()
    {
        float timer = 0f;
        while (timer < currentDuration)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                TrySelectTarget();
            }

            timer += Time.deltaTime;
            yield return null;
        }
        EndLockMode();
        Debug.Log("锁定时间到");
    }
    public void OnTargetDied()
    {
        if (!isLockMode) return;
        EndLockMode();
        Debug.Log("锁定目标已死亡，技能结束");
    }
    // 尝试从鼠标位置选择鱼
    void TrySelectTarget()
    {
        if (currentTarget != null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, LayerMask.GetMask("Fish"));
        if (hit.collider != null)
        {
            FishAttrbute fish = hit.collider.GetComponent<FishAttrbute>();
            Boss boss = hit.collider.GetComponent<Boss>();
            if (fish != null && !fish.isDead)
            {
                SetTarget(fish);
                // 设置下一帧禁止炮台开火，防止这次点击同时触发子弹
                StartCoroutine(BlockFireOneFrame());
            }
            else if (boss != null && !boss.isDead)
            {
                SetTarget(boss);
            }
        }
    }
    IEnumerator BlockFireOneFrame()
    {
        BlockFireThisFrame = true;
        yield return new WaitForEndOfFrame();
        BlockFireThisFrame = false;
    }
    // 设置锁定目标
    void SetTarget(Component target)
    {
        ClearTarget();
        currentTarget = target;

        if (lockEffectPrefab != null)
        {
            currentEffect = Instantiate(lockEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
        }
        Debug.Log($"锁定目标：{target.name}");
    }

    // 清除当前目标（特效销毁，引用置空）
    void ClearTarget()
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
        currentTarget = null;
    }
    private void EndLockMode()
    {
        if (durationCoroutine != null)
        {
            StopCoroutine(durationCoroutine);
            durationCoroutine = null;
        }
        ClearTarget();          // 移除特效，currentTarget = null
        isLockMode = false;     // 关键
                                // 无需操作按钮冷却，它独立运行
    }
    /// <summary>
    /// 供外部（鱼死亡时）调用，清除锁定目标
    /// </summary>

 

    // ----- 升级方法 -----
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        boundButton?.SetCooldownDuration(currentCooldown);
        Debug.Log($"锁定冷却减少 {seconds} 秒，当前冷却：{currentCooldown}");
    }

    public void IncreaseDuration(float extraSeconds)
    {
        currentDuration += extraSeconds;
        Debug.Log($"锁定持续时间增加 {extraSeconds} 秒，当前持续：{currentDuration}");
    }
}