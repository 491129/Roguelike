using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [Header("打击反馈")]
    [SerializeField] private GameObject hitEffect;       // 特效子物体
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
        //transform.Translate(transform.up * speed * Time.unscaledDeltaTime, Space.World);
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

        if (other.CompareTag("Fish") || other.CompareTag("BOSS"))
        {
            isHitting = true;
            col.enabled = false;
            rb.velocity = Vector2.zero;

            StartCoroutine(DelayedDeactivate(0.3f));  // 0.3秒后消失
        }

    }
    IEnumerator DelayedDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        Deactivate();
    }

    void Deactivate()
    {
        if (hitEffect) hitEffect.SetActive(false);
        isHitting = false;                  // 重置状态供下次使用
        col.enabled = true;

        // 使用对象池回收
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.ReturnBullet(gameObject);
        else
            Destroy(gameObject);            // 后备：无对象池时销毁
    }
}