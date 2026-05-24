using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAttrbute : MonoBehaviour
{
    public int fishNumber = 0;
    public int fishSpeed = 0;
    public int goldNum = 0;

    private Rigidbody2D rb;
    private Collider2D col;
    public bool isDead { get; private set; }   
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (collision.tag == "Bullet")
        {
           
            isDead = true;
            rb.velocity = Vector2.zero;       // 停止物理运动（如果用了刚体）
            col.enabled = false;              // 关闭碰撞器，防止重复击中
            GameManager.AddCoin(goldNum);
            // 启动延迟销毁协程
            StartCoroutine(DieAfterDelay(0.2f));
        }
       
    }
    IEnumerator DieAfterDelay(float delay)
    {
        // 可以在这里播放死亡动画、闪红等
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
