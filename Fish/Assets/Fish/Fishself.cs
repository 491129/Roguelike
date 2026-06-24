using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishself : MonoBehaviour
{
    private FishAttrbute attr;
    public static bool IsFrozen;
    private Vector2 moveDirection = Vector2.right;   // 默认向右
    void Start()
    {
        attr = GetComponent<FishAttrbute>();
    }
    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }
    void Update()
    {
        if (IsFrozen) return;
        if (attr != null && attr.isDead) return;
        float speed = attr.CurrentSpeed;
        //transform.Translate(Vector3.right * speed * Time.deltaTime);
        transform.Translate(moveDirection * speed * Time.deltaTime);
        //销毁
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.15f || viewPos.x > 1.15f || viewPos.y < -0.15f || viewPos.y > 1.15f)
        {
            fish.OnFishDied();   // 通知生成器
            // 销毁或回收到对象池
            Destroy(gameObject);  // 或调用你的 Recycle 方法
        }
    }
}
