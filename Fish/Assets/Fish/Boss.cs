using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss Instance { get; private set; }
    public string bossType;
    public float speed = 5f;
    [Range(0f, 1f)] public float catchProbability = 0.8f;

    [Header("穿梭设置")]
    public static int passes = 3;
    [SerializeField] private float rotationSmoothSpeed = 360f;
    [SerializeField] private float outOfScreenMargin = 0.3f;

    private Vector2 moveDirection;
    private SpriteRenderer sr;
    private Collider2D col;
    public bool isDead = false;

    private Transform centerPoint;
    private enum MoveState { Entering, Shuttling, Leaving, Staying }
    private MoveState currentState = MoveState.Entering;
    private int tripCounter = 0;
    private float targetX;
    private bool movingRight;
    private bool isExiting = false;   // 是否正在离开屏幕（穿梭中的向外阶段）

    public bool bossDefeated = false;
    public int[] catchRates = new int[5] { 8000, 8500, 9000, 9500, 9800 };
    public int goldReward = 30000000;
    [Header("最终Boss (章鱼)")]
    public bool isFinalBoss = false;         // 在预制体 Inspector 中勾选
    public float stayDuration = 60f;         // 停留总时间
    public float skillInterval = 10f;        // 技能触发间隔
    public int[] effectWeights = { 3, 3, 4 }; // 三种效果的权重

    [Header("技能预制体/引用")]
    public Transform[] fishSpawnPoints;      // 左边出生点（至少1个）
    public int summonCount = 50;              // 每次召唤鱼的数量

    [Header("炮台攻速降低")]
    private Animator anim;
    public float slowPercent = 0.2f;         // 降低20%
    public float slowDuration = 5f;
    private bool isPerformingSkill = false;


    private bool effectTriggeredThisPass = false;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        if (collision.CompareTag("Wall"))
        {
            // 最终Boss在离开阶段允许穿墙，直接销毁
            if (isFinalBoss && currentState == MoveState.Leaving)
            {
                Debug.Log("[Boss] 最终Boss离开时碰到Wall，直接销毁");
                Destroy(gameObject);
                return;
            }
            Debug.Log("Boss 撞墙死亡");
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
        if (LockSkill.Instance != null && LockSkill.Instance.IsLockModeActive && LockSkill.Instance.LockedTarget != null)
        {
            if (this != LockSkill.Instance.LockedTarget) return;
        }
        if (bossType == "ThreeHeadShark" && Random.value < 0.5f)
        {
            Debug.Log("三头鲨闪避了攻击");
            return;
        }
        int lvl = ALLCannon.currentLevel;
        lvl = Mathf.Clamp(lvl, 0, catchRates.Length - 1);
        float prob = catchRates[lvl] / 10000f;
        Debug.Log(prob);
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
        if (isFinalBoss)
            moveDirection = Vector2.down; 
        else
            moveDirection = dir.normalized; ;
        currentState = MoveState.Entering;
        tripCounter = 0;
        isExiting = false;
        if (!isFinalBoss)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        Debug.Log($"[Boss] Init 方向={moveDirection}");
    }

    public void SetCenter(Transform center) => centerPoint = center;

    void Update()
    {
        if (isDead) return;
        if (Fishself.IsFrozen) return;

        // 状态机
        switch (currentState)
        {
            case MoveState.Entering: UpdateEntering(); break;
            case MoveState.Shuttling: UpdateShuttling(); break;
            case MoveState.Leaving: break;   // 一直向外飞
            case MoveState.Staying: break; // 停留期间不移动，由协程控制
        }

        // 移动 + 平滑旋转
        if (moveDirection != Vector2.zero && currentState != MoveState.Staying)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
            if (!isFinalBoss)
            {
                float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(0, 0, targetAngle), rotationSmoothSpeed * Time.deltaTime);
            }
        }

        // —— 销毁逻辑 ——
        // 只允许在 Leaving 状态下，且飞出屏幕足够远后才销毁
        if (currentState == MoveState.Leaving)
        {
            Vector3 v = Camera.main.WorldToViewportPoint(transform.position);
            if (v.x < -0.5f || v.x > 1.5f || v.y < -0.5f || v.y > 1.5f)
            {
                Debug.Log("[Boss] 最终离开，销毁");
                Destroy(gameObject);
            }
        }
        // 其他状态绝不销毁，即使飞出屏幕也是为了掉头，不掉头也不销毁
    }

    void UpdateEntering()
    {
        if (!centerPoint) return;
        float dist = Vector2.Distance(transform.position, centerPoint.position);

        if ((dist < 1.5f))
        {
            if (isFinalBoss)
            {
                // 最终Boss：停在中心，开始停留协程
                moveDirection = Vector2.zero;
                currentState = MoveState.Staying;
                Debug.Log("[Boss] 最终Boss进入停留状态");
                StartCoroutine(StayAndSkillRoutine());
            }
            else
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
                // 一旦进入屏幕中间区域，就开始穿梭
                if (viewPos.x > 0.2f && viewPos.x < 0.8f)
                {
                    currentState = MoveState.Shuttling;
                    isExiting = false;

                    if (moveDirection.x >= 0)
                    {
                        movingRight = true;
                        targetX = Camera.main.ViewportToWorldPoint(new Vector3(0.95f, 0.5f, 0)).x;
                    }
                    else
                    {
                        movingRight = false;
                        targetX = Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.5f, 0)).x;
                    }
                    UpdateTargetDirection();
                    Debug.Log($"[Boss] 进入 Shuttling, targetX={targetX}, movingRight={movingRight}");
                }
            }
        }

    }
    IEnumerator StayAndSkillRoutine()
    {
        float timer = 0f;
        while (timer < stayDuration)
        {
            yield return new WaitForSeconds(skillInterval);
            try
            {
                TriggerRandomSkill();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"技能触发异常: {e}");
            }
            timer += skillInterval;
        }
        currentState = MoveState.Leaving;
        moveDirection = Vector2.down;
        Debug.Log("[Boss] 最终Boss停留结束，向下离开");
    }
    void TriggerRandomSkill()
    {
        int total = 0;
        foreach (int w in effectWeights) total += w;
        int rand = Random.Range(0, total);
        int cumulative = 0;
        for (int i = 0; i < effectWeights.Length; i++)
        {
            cumulative += effectWeights[i];
            if (rand < cumulative)
            {
                switch (i)
                {
                    case 0: InkEffect(); Debug.Log("黑雾"); break;
                    case 1: SummonFish(); Debug.Log("鱼群"); break;
                    case 2: TentacleAttack(); Debug.Log("Attack"); break;
                }
                break;
            }
        }
    }
    void InkEffect()
    {
        if (anim != null)
            anim.SetTrigger("Mo");
        TotemManager.Instance?.DisableRandomTotem(5f);
        // 特效：黑雾会在 TotemManager.DisableRandomTotem 中生成
    }

    void SummonFish()
    {
        Debug.Log("[Boss] SummonFish 调用");
        if (fishSpawnPoints == null || fishSpawnPoints.Length == 0)
        {
            Debug.LogError("[Boss] Fish Spawn Points 为空！");
            return;
        }
        if (BossManager.Instance == null)
        {
            Debug.LogError("[Boss] BossManager.Instance 为空！");
            return;
        }
        BossManager.Instance.SpawnFishWave(fishSpawnPoints, summonCount);
    }
    void TentacleAttack()
    {
        // 播放章鱼攻击动画（假设 Animator Controller 中有 Attack 触发器）
        if (anim != null)
            anim.SetTrigger("Attack");

        // 降低炮台攻速
        if (Cannon.Instance != null)
        {
            Cannon.Instance.attackSpeedMultiplier *= (1 - slowPercent);
            StartCoroutine(RestoreSpeedAfterDelay(slowDuration));
        }
    }
    IEnumerator RestoreSpeedAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (Cannon.Instance != null)
            Cannon.Instance.attackSpeedMultiplier /= (1 - slowPercent);
    }
    void UpdateShuttling()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        if (isExiting)
        {
            bool beyondLeft = viewPos.x <= -outOfScreenMargin;
            bool beyondRight = viewPos.x >= 1 + outOfScreenMargin;
            if ((beyondLeft && !movingRight) || (beyondRight && movingRight))
            {
                ReverseDirectionAndReturn();
            }
            return;
        }

        if (Mathf.Abs(transform.position.x - targetX) < 0.2f)
        {
            isExiting = true;
            moveDirection = new Vector2(movingRight ? 1f : -1f, 0f);
            return;
        }

        UpdateTargetDirection();
    }

    void ReverseDirectionAndReturn()
    {
        tripCounter++;
        Debug.Log($"[Boss] 掉头！tripCounter={tripCounter}");

        if (tripCounter >= passes)
        {
            currentState = MoveState.Leaving;
            // 继续当前方向向外飞，不再回头
            Debug.Log("[Boss] 完成所有穿梭，进入 Leaving 状态");
            return;
        }

        // 掉头
        movingRight = !movingRight;
        isExiting = false;
        targetX = movingRight
            ? Camera.main.ViewportToWorldPoint(new Vector3(0.95f, 0.5f, 0)).x
            : Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.5f, 0)).x;
        moveDirection = new Vector2(movingRight ? 1f : -1f, 0f);
        Debug.Log($"[Boss] 掉头后新方向={moveDirection}, 新targetX={targetX}");
    }

    void UpdateTargetDirection()
    {
        Vector2 targetPos = new Vector2(targetX, transform.position.y);
        moveDirection = (targetPos - (Vector2)transform.position).normalized;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("[Boss] 死亡");
        if (LockSkill.Instance != null) LockSkill.Instance.OnTargetDied();
        col.enabled = false;
        GameManager.AddCoin(goldReward);

        if (BossManager.Instance != null) BossManager.Instance.OnBossDefeated(this);
        SkillShopManager.Instance.BJRefreshAllSlots();
        StartCoroutine(DieAfterDelay(0.5f));
    }

    IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}