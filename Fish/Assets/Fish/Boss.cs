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
    private bool isDead = false;

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
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        // 避免物理系统干扰，设为运动学刚体
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = true;
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
            // 所有Boss均有概率逃脱，catchProbability表示成功造成伤害的概率
            if (Random.value > catchProbability)
            {
                // 逃脱：此次攻击无效，可加入视觉反馈
                Debug.Log($"{bossType} 闪避了攻击");
                return;
            }

            health -= 2;
            if (health <= 0)
            {
                Die();
                bossDefeated = true;
            }
        }
    }

    public void Init(Vector2 dir)
    {
        moveDirection = dir.normalized;

        // 计算方向角度（相对于 X 轴正方向）
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // 旋转 Boss，使其右方（默认朝向）指向移动方向
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 不再需要 flipX，移除原有 sr.flipX 相关代码
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
        col.enabled = false;

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