using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    [Header("打击反馈")]
    [SerializeField] private GameObject hitEffectA;      // 特效A：子弹炸开
    [SerializeField] private GameObject hitEffectB;      // 特效B：渔网打开
    [SerializeField] private float hitEffectLife = 1.5f; // 特效播放时长（秒）

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isHitting;

    private Vector2 fireDirection;   // 由外部在激活前设置


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    public void SetDirection(Vector2 dir)
    {
        fireDirection = dir.normalized;
    }


    void OnEnable()
    {
        isHitting = false;
        col.enabled = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.velocity = fireDirection * speed;
        CancelInvoke();
        Invoke(nameof(Deactivate), lifeTime);
    }

    void OnDisable()
    {
        if (rb) rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        CancelInvoke();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHitting) return;
        if (other.CompareTag("Fish") || other.CompareTag("BOSS"))
        {
            FishAttrbute fish = other.GetComponent<FishAttrbute>();
            Boss boss = other.GetComponent<Boss>();
            if (fish == null && boss == null) return;
            // 锁定模式下，只攻击锁定目标，其他鱼穿透
            if (LockSkill.Instance != null && LockSkill.Instance.IsLockModeActive)
            {
                Component locked = LockSkill.Instance.LockedTarget;
                if (locked == null) return;
                if (fish != null && fish != locked) return;
                if (boss != null && boss != locked) return;

            }
            // 正常处理命中
            isHitting = true;
            col.enabled = false;
            rb.velocity = Vector2.zero;
            // 播放两个特效在子弹当前位置
            SpawnHitEffects(transform.position);

            StartCoroutine(DelayedDeactivate(0.1f));
        }
    }
    void SpawnHitEffects(Vector3 position)
    {
        if (hitEffectA)
        {
            var effectA = Instantiate(hitEffectA, position, Quaternion.identity);
            Destroy(effectA, hitEffectLife);
        }
        if (hitEffectB)
        {
            var effectB = Instantiate(hitEffectB, position, Quaternion.identity);
            CircleCollider2D netCol = effectB.GetComponentInChildren<CircleCollider2D>();
            if (netCol != null && FishnetManager.Instance != null)
            {
                netCol.radius = FishnetManager.Instance.GetCurrentRadius();
            }
            Destroy(effectB, hitEffectLife);
        }
    }

    IEnumerator DelayedDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        Deactivate();
    }

    void Deactivate()
    {
        isHitting = false;
        col.enabled = true;
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.ReturnBullet(gameObject);
        else
            Destroy(gameObject);
    }
}