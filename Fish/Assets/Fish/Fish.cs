using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
public class Fish : MonoBehaviour
{
    private FishData data;
    private int currentHealth;
    private float speed;
    private Vector2 moveDirection;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private FishSpawner spawner;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        spawner = FindObjectOfType<FishSpawner>();
    }

    /// <summary> 由生成器调用，注入数据并激活 </summary>
    public void Init(FishData fishData, Vector2 dir)
    {
        data = fishData;

        // 应用外观
        sr.sprite = data.sprite;
        if (data.animator != null)
        {
            Animator anim = GetComponent<Animator>();
            if (anim == null) anim = gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = data.animator;
        }

        // 随机属性
        speed = Random.Range(data.minSpeed, data.maxSpeed);
        currentHealth = data.maxHealth;

        // 方向
        moveDirection = dir.normalized;
        FlipSprite();
        rb.velocity = moveDirection * speed;
    }

    /// <summary> 仅在游动过程中需要改变方向时调用（如屏幕反弹） </summary>
    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        FlipSprite();
        rb.velocity = moveDirection * speed;
    }

    void FlipSprite()
    {
        // 假设鱼图片默认朝右，向左游时翻转
        if (moveDirection.x < 0)
            sr.flipX = true;
        else if (moveDirection.x > 0)
            sr.flipX = false;
    }

    void Update()
    {
        // 保持速度（防止物理干扰）
        rb.velocity = moveDirection * speed;

        // 超出屏幕边界回收
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
        {
            Recycle();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(FlashRed());
    }

    void Die()
    {
        // 给予金币
        GameManager.AddCoin(data.price);
        // 可在此播放死亡特效，然后回收
        Recycle();
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void Recycle()
    {
        if (spawner != null)
            spawner.ReturnFish(gameObject);
        else
            gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // 重置颜色，防止对象池复用残留
        sr.color = Color.white;
        rb.velocity = Vector2.zero;
    }
}