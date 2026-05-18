using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [Header("打击反馈")]
    [SerializeField] private GameObject hitEffect;       // 可选的粒子特效子物体
    [SerializeField] private float hitAnimationTime = 0.3f; // 动画时长
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isHitting;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        isHitting = false;
        col.enabled = true;
        rb.velocity = transform.up * speed;
        if (hitEffect) hitEffect.SetActive(false);
        CancelInvoke();
        Invoke(nameof(Deactivate), lifeTime);
    }

    void OnDisable()
    {
        rb.velocity = Vector2.zero;
        CancelInvoke();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHitting) return;

        if (other.CompareTag("Fish"))
        {
            isHitting = true;
            col.enabled = false;
            rb.velocity = Vector2.zero;

            Fish fish = other.GetComponent<Fish>();
            if (fish != null)
                fish.TakeDamage(1);

            // 播放打击特效...
            StartCoroutine(HitAndReturn());
        }
    }

    System.Collections.IEnumerator HitAndReturn()
    {
        yield return new WaitForSeconds(hitAnimationTime);
        Deactivate();
    }

    void Deactivate()
    {
        // 重置特效
        if (hitEffect) hitEffect.SetActive(false);
        ObjectPool.Instance.ReturnBullet(gameObject);
    }
}