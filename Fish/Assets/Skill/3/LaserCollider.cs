using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    private HashSet<FishAttrbute> hitFishes = new HashSet<FishAttrbute>();
    private HashSet<Boss> hitBosses = new HashSet<Boss>();

    public void Init(float duration)
    {
        // 可选：在持续时间结束后自动清除记录，但这里碰撞体本身会被销毁，所以不需要
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            FishAttrbute fish = other.GetComponent<FishAttrbute>();
            if (fish != null && !fish.isDead && !hitFishes.Contains(fish))
            {
                // 锁定检查：如果锁定模式激活且锁定目标存在，只允许伤害锁定目标
                if (LockSkill.Instance != null && LockSkill.Instance.IsLockModeActive && LockSkill.Instance.LockedTarget != null)
                {
                    if (fish != LockSkill.Instance.LockedTarget)
                        return;
                }

                hitFishes.Add(fish);
                fish.HandleBulletHit();   // 复用鱼的统一受伤入口
            }
        }
        else if (other.CompareTag("BOSS"))
        {
            Boss boss = other.GetComponent<Boss>();
            if (boss != null && !boss.isDead && !hitBosses.Contains(boss))
            {
                if (LockSkill.Instance != null && LockSkill.Instance.IsLockModeActive && LockSkill.Instance.LockedTarget != null)
                {
                    if (boss != LockSkill.Instance.LockedTarget)
                        return;
                }

                hitBosses.Add(boss);
                boss.HandleBulletHit();
            }
        }
    }
}