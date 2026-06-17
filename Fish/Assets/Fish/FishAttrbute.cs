using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


    public class FishAttrbute : MonoBehaviour
    {
        public static FishAttrbute Instance { get; private set; }
        public enum FishType
        {
            Gold,
            GF00,
            CF,
            GB,
            GD,
            GF,
            MT
        }
        public FishType fishType;
        public int fishNumber = 0;
        public float fishSpeed = 0;
        public int goldNum = 0;
        public static float getgoldMore = 1f;

        private Rigidbody2D rb;
        private Collider2D col;

        [Header("动画与特效")]
        [SerializeField] private GameObject deathEffect;    // 死亡动画/粒子预制体
        [SerializeField] private float deathEffectDuration = 0.5f;

        [Header("金币动画")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private Vector2 targetPos = new Vector2(-8f, -4f);
        [SerializeField] private float moveDuration = 0.5f;

        [Header("逃脱设置")]
        //public static float escapeChance = 0.4f;
        public float escapeSpeedMultiplier = 2f;
        public float escapeDuration = 3f;
        public float CurrentSpeed { get; set; }   // 当前实际速度，逃脱时会变化
                                                  //private float baseSpeed;                  // 原始速度
        public static float CatchRateMultiplier = 1f;

        private SpriteRenderer sr;
        // private bool escaping = false; // 是否正在逃脱
        public bool isDead { get; private set; }
        [Header("鱼种编号 (1~12)")]
        public int fishID = 1;   // 在预制体 Inspector 中设置 1~12

        // 移除原来的静态 escapeChance（若还在用，可保留但不再依赖它）

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            CurrentSpeed = fishSpeed;
        }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            HandleBulletHit();
        }
        else if (collision.tag == "Wall")
            Destroy(gameObject);
    }
    public void HandleBulletHit()
        {
            if (isDead) return;   // 防止重复触发

            // 1. 获取当前炮台等级 (0~4)
            int cannonLevel = ALLCannon.currentLevel;   // 静态变量
                                                        // 2. 查表得到该鱼的基础捕捉率（万分比）
            float baseCatchRate = FishDataConfig.CatchRates[fishID - 1, cannonLevel] / 10000f;
        // 3. 应用全局捕鱼加成（商人海盗、挑剔海盗等）
        //float catchMultiplier = TotemManager.Instance != null ? TotemManager.Instance.CatchRateMultiplier : 1f;
        //float catchMultiplier = 1f;
            float finalCatchRate = Mathf.Clamp01(baseCatchRate * CatchRateMultiplier);

            Debug.Log(baseCatchRate+"1" +finalCatchRate+"2"+ CatchRateMultiplier);

        // 4. 判断是否捕捉成功
        if (Random.value < finalCatchRate)
            {
                // 捕获成功 -> 死亡逻辑
                isDead = true;
                col.enabled = false;
                StartCoroutine(DieAfterDelay(0.2f));

                if (deathEffect != null)
                {
                    GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
                    Destroy(effect, deathEffectDuration);
                }
                SpawnCoinAndFly();
                Coin100();

                int coin = Mathf.RoundToInt(goldNum * getgoldMore);
                // 技能加成金币（原有 switch 保持不变）
                switch (fishType)
                {
                    case FishType.Gold:
                        if (YFSkill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(YFSkill.fishCoin * getgoldMore));
                        }
                        else
                            GameManager.AddCoin(coin);
                        break;
                    case FishType.GF:
                        if (GFSkill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(GFSkill.fishCoin * getgoldMore));
                            Debug.Log("222" + GFSkill.fishCoin);
                        }
                        else
                            GameManager.AddCoin(coin);
                        Debug.Log("222" + coin);
                        break;
                    case FishType.CF:
                        if (CFSkill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(CFSkill.fishCoin * getgoldMore));
                        }
                        else
                            GameManager.AddCoin(coin);
                        break;
                    case FishType.GB:
                        if (GBSkill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(GBSkill.fishCoin * getgoldMore));
                        }
                        else
                            GameManager.AddCoin(coin);
                        break;
                    case FishType.GD:
                        if (GDSkill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(GDSkill.fishCoin * getgoldMore));
                        }
                        else
                            GameManager.AddCoin(coin);
                        break;
                    case FishType.GF00:
                        if (GF00Skill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(GF00Skill.fishCoin * getgoldMore));
                        }
                        else
                            GameManager.AddCoin(coin);
                        break;
                    case FishType.MT:
                        if (MTSkill.isUsed)
                        {
                            GameManager.AddCoin(Mathf.RoundToInt(MTSkill.fishCoin * getgoldMore));
                        }
                        else
                            GameManager.AddCoin(coin);
                        break;
                    default:
                        Debug.Log("N" + goldNum);
                        GameManager.AddCoin(coin);
                        break;
                        // return goldNum;
                }
            }
            else
            {
                // 逃脱
                StartCoroutine(EscapeRoutine());
            Debug.Log("fish222");
            }
        }

        IEnumerator EscapeRoutine()
        {
            //escaping = true;
            // 关闭碰撞器，避免重复击中
            //col.enabled = false;

            // 加速：速度变为原来的 escapeSpeedMultiplier 倍
            CurrentSpeed = fishSpeed * escapeSpeedMultiplier;   // 加速

            // 可以播放一个受惊特效（闪烁、变色）
            //if (sr != null)
            //    sr.color = Color.yellow;  // 变黄表示受惊

            yield return new WaitForSeconds(escapeDuration);

            // 恢复正常速度
            CurrentSpeed = fishSpeed;

            // 恢复颜色
            if (sr != null)
                sr.color = Color.white;

            // 恢复碰撞器
            col.enabled = true;
            //escaping = false;
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
        public void Coin100()
        {
            if (TotemManager.Instance.get100)
            {
                GameManager.AddCoin(100);
            }
        }
    }

