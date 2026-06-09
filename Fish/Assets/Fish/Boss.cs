using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string bossType;                     // 类型标识
    public float speed = 5f;                   // 移动速度
    public int health = 10;                    // 生命值
    [Range(0f, 1f)] public float catchProbability = 0.8f;  // 被击中的成功捕捉概率（1为必中）

    private Vector2 moveDirection;
    private SpriteRenderer sr;
    private Collider2D col;
    private bool isDead = false;

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
            }
        }
    }

    public void Init(Vector2 dir)
    {
        moveDirection = dir.normalized;
        if (sr != null)
            sr.flipX = dir.x < 0;
    }

    void Update()
    {
        if (isDead) return;
        // 配合冻结技能
        if (Fishself.IsFrozen) return;

        // 使用Translate移动，受Time.timeScale影响
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 超出屏幕销毁
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.2f || viewPos.x > 1.2f || viewPos.y < -0.2f || viewPos.y > 1.2f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        col.enabled = false;

        // 通知BossManager移除Debuff
        if (BossManager.Instance != null)
            BossManager.Instance.OnBossDefeated(this);
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