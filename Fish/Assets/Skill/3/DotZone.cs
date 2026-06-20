using System.Collections;
using UnityEngine;

public class DotZone : MonoBehaviour
{
    public void Init(float duration, float radius)
    {
        StartCoroutine(DamageOverTime(duration, radius));
    }

    IEnumerator DamageOverTime(float totalDuration, float radius)
    {
        float timer = 0f;
        while (timer < totalDuration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Fish"))
                {
                    FishAttrbute fish = hit.GetComponent<FishAttrbute>();
                    if (fish != null && !fish.isDead)
                        fish.HandleBulletHit();
                }
            }
            yield return new WaitForSeconds(0.5f);
            timer += 0.5f;
        }
        Destroy(gameObject);
    }
}