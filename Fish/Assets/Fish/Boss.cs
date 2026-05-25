using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string bossType;
    public float speed = 5f;
    public int health = 10;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (collision.tag == "Bullet")
        {
            health -= 2;
            
        }

    }
    public void Init(Vector2 dir)
    {
        moveDirection = dir.normalized;
        rb.velocity = moveDirection * speed;

        // 根据方向翻转图片（假设默认朝右）
        if (sr != null)
            sr.flipX = dir.x < 0;
    }

    void Update()
    {
        // 保持恒定速度（防止物理干扰）
        rb.velocity = moveDirection * speed;

        // 可选：超出屏幕后自动回收
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.2f || viewPos.x > 1.2f || viewPos.y < -0.2f || viewPos.y > 1.2f)
        {
            // Boss 离开屏幕后可以在这里通知 Manager 回收，或直接销毁
            Destroy(gameObject);
        }
        if(health<=0)
        {
            rb.velocity = Vector2.zero;       // 停止物理运动（如果用了刚体）
            col.enabled = false;              // 关闭碰撞器，防止重复击中
            // 启动延迟销毁协程
            StartCoroutine(DieAfterDelay(0.5f));
        }
    }
    IEnumerator DieAfterDelay(float delay)
    {
        // 可以在这里播放死亡动画、闪红等
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}