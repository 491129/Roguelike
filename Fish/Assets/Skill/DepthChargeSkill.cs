using UnityEngine;

public class DepthChargeSkill : MonoBehaviour
{
    public static DepthChargeSkill Instance { get; private set; }

    [Header("基础设定")]
    public GameObject BoomEffectPrefab;
    [SerializeField] private float baseCooldown = 10f;
    [SerializeField] private float baseRadius = 3f;
    [SerializeField] private int baseDamage = 3;

    // 留给后续升级的变量（目前保持与基础一致）
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentRadius;
    [HideInInspector] public int currentDamage;

    // 后续Type3属性预留
    [HideInInspector] public int explosionCount = 1;
    [HideInInspector] public bool hasDotZone = false;
    [HideInInspector] public float dotDuration = 5f;

    private SkillButton boundButton;   // 绑定的技能按钮
    public static bool isUsed = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // 初始化
        currentCooldown = baseCooldown;
        currentRadius = baseRadius;
        currentDamage = baseDamage;
    }

    /// <summary>
    /// 由管理器调用，绑定按钮，以便控制冷却
    /// </summary>
    public void BindButton(SkillButton button)
    {
        boundButton = button;
    }

    /// <summary>
    /// 使用技能（按钮点击时调用）
    /// </summary>
    public void Use()
    {
        if (boundButton == null) return;

        // 获取鼠标世界坐标
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 范围检测
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, currentRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Fish"))
            {
                FishAttrbute fish = hit.GetComponent<FishAttrbute>();
                if (fish != null && !fish.isDead)
                {
                    FishAttrbute.Instance.HandleBulletHit();
                }
            }
        }

        // 触发按钮冷却
        boundButton.StartCooldown();
    }
         public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        boundButton?.SetCooldownDuration(currentCooldown);
        Debug.Log($"深水炸弹冷却减少 {seconds} 秒，当前冷却：{currentCooldown}");
    }

}