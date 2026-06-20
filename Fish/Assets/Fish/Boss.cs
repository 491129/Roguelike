using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss Instance { get; private set; }
    public string bossType;                     // 类型标识
    public float speed = 5f;                   // 移动速度
    public int health = 10;                    // 生命值
    [Range(0f, 1f)] public float catchProbability = 0.8f;  // 被击中的成功捕捉概率（1为必中）

    private Vector2 moveDirection;
    private SpriteRenderer sr;
    private Collider2D col;
    public bool isDead = false;

    [Header("停留设置")]
    [SerializeField] private float stopDistance = 1.5f;   // 距中心点多近时停留
    [SerializeField] private float minStayDuration = 3f;
    [SerializeField] private float maxStayDuration = 5f;
    public static float skillDuration = 0;
    // 停留相关
    private Transform centerPoint;
    private bool hasStopped = false;
    private bool isStaying = false;
    private Vector2 savedDirection;   // 保存原方向，停留结束后恢复

    //SkillShopManager skillShopManager;
    public bool bossDefeated = false;
    public int[] catchRates = new int[5] { 8000, 8500, 9000, 9500, 9800 }; // 万分比，可在预制体修改
    public int goldReward = 30000000;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Wall"))
        {
            Debug.Log("WALL!!!");
            Die();
        }
        else if (collision.CompareTag("Bullet"))
        {
            HandleBulletHit();
        }
    }
    public void HandleBulletHit()
    {
        if (isDead) return;

        // 锁定检查：只有被锁定的 Boss 才能被击中
        if (LockSkill.Instance != null && LockSkill.Instance.IsLockModeActive && LockSkill.Instance.LockedTarget != null)
        {
            // 注意：LockedTarget 现在可以存储 Boss，需要修改 LockSkill
            if (this != LockSkill.Instance.LockedTarget)
                return;
        }

        // 概率逃脱
        int lvl = ALLCannon.currentLevel;
        lvl = Mathf.Clamp(lvl, 0, catchRates.Length - 1);
        float prob = catchRates[lvl] / 10000f;
        if (Random.value > prob)
        {
            Debug.Log($"{bossType} 闪避了攻击");
            return;
        }
        Die();
        bossDefeated = true;
    }

    public void Init(Vector2 dir)
    {
        moveDirection = dir.normalized;
        savedDirection = moveDirection;   // 保存初始方向，供停留后恢复

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SetCenter(Transform center)
    {
        centerPoint = center;
    }

    void Update()
    {
        if (isDead) return;
        if (Fishself.IsFrozen) return;

        // 检查是否到达中心点附近且尚未停留
        if (!hasStopped && centerPoint != null)
        {
            float dist = Vector2.Distance(transform.position, centerPoint.position);
            if (dist < stopDistance)
            {
                StartCoroutine(StayRoutine());
            }
        }

        // 只有不在停留状态时才移动
        if (!isStaying)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        }
    }

    IEnumerator StayRoutine()
    {
        hasStopped = true;
        isStaying = true;
        moveDirection = Vector2.zero;   // 停止移动

        float stayDuration = Random.Range(minStayDuration, maxStayDuration) + skillDuration;
        Debug.Log($"{bossType} 在中心点停留 {stayDuration} 秒");
        yield return new WaitForSeconds(stayDuration);

        moveDirection = savedDirection; // 恢复原方向
        isStaying = false;
    }
   

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (LockSkill.Instance != null)
            LockSkill.Instance.OnTargetDied();
        col.enabled = false;
        GameManager.AddCoin(goldReward);      // 击杀奖励
        // 通知BossManager移除Debuff
        if (BossManager.Instance != null)
            BossManager.Instance.OnBossDefeated(this);
        SkillShopManager.Instance.BJRefreshAllSlots();
        Debug.Log("DEAD");
        StartCoroutine(DieAfterDelay(0.5f));
    }

    IEnumerator DieAfterDelay(float delay)
    {
        // 此处可播放死亡特效
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
 
}