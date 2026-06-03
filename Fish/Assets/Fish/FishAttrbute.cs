using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FishAttrbute : MonoBehaviour
{
    public enum FishType
    {
        Normal,   // ÆÕÍ¨Óã
        Gold,     // YF 
        Shark,    // SF 
        HHH,
        YYY
    }

    public FishType fishType;
    public int fishNumber = 0;
    public int fishSpeed = 0;
    public int goldNum = 0;

    private Rigidbody2D rb;
    private Collider2D col;

    [Header("½ð±Ò¶¯»­")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Vector2 targetPos = new Vector2(-8f, -4f); 
    [SerializeField] private float moveDuration = 0.5f;  

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
              
            col.enabled = false;  
            StartCoroutine(DieAfterDelay(0.2f));

            SpawnCoinAndFly();
           
            switch (fishType)
            {
                case FishType.Gold:
                    if (YFSkill.isUsed)
                    {
                        Debug.Log("2" + YFSkill.fishCoin);
                        GameManager.AddCoin(YFSkill.fishCoin);
                    }
                    else
                        Debug.Log("1"+goldNum);
                    GameManager.AddCoin(goldNum);
                    break;
                case FishType.Shark:
                    Debug.Log(goldNum);
                    GameManager.AddCoin(goldNum);
                    break;
                   // return SkillShopManager.SFskill ? specialCoin : goldNum;
                default:
                    Debug.Log("N"+goldNum);
                    GameManager.AddCoin(goldNum);
                    break;
                   // return goldNum;
            }

            //if (SkillShopManager.YFskill)
            //{
            //    GameManager.AddCoin(YFSkill.fishCoin);
            //}
            //else
            //    GameManager.AddCoin(goldNum);
        }
       
    }
    IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    private void SpawnCoinAndFly()
    {
        if (coinPrefab == null)
        {
            return;
        }

        GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Vector3 originalPos = coin.transform.position;

        Sequence seq = DOTween.Sequence();

        seq.Append(coin.transform.DOMoveY(originalPos.y + 0.5f, 0.2f).SetEase(Ease.OutQuad));
        seq.Append(coin.transform.DOMoveY(originalPos.y, 0.2f).SetEase(Ease.InQuad));
        seq.AppendInterval(0.1f);
        seq.Append(coin.transform.DOMove(targetPos, moveDuration).SetEase(Ease.InBack));
        seq.OnComplete(() => Destroy(coin));
    }
}
