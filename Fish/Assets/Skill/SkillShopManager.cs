using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillShopManager : MonoBehaviour
{
   
    static public bool YFskill=false;
    static public bool TTskill = false;
    static public bool DJskill = false;
    //市场券
    public GameObject Sure;//确认界面
    public Sprite[] skillSprites;
    public Image ShopImage;
    public int bSkillNum;//总技能数
    private int bSkill;//技能
    public Text biname;
    public Text bitext;
    public Text bctext;
    //补给券
    public GameObject SureTT;//确认界面
    public Sprite[] sskillSprites;
    public Image ShopImages;
    public int sSkillNum;//总技能数
    private int sSkill;//技能
    public Text siname;
    public Text sitext;
    public Text sctext;
    //装置制造
    public GameObject SureD;//确认界面
    public Sprite[] DskillSprites;
    public Image ShopImageD;
    public int DSkillNum;//总技能数
    private int DSkill;//技能
    public Text Diname;
    public Text Ditext;
    public Text Dctext;
    public Sprite skillSpriteDJ;
    private void Start()
    {

    }
    private void Update()
    {
        bSkill= UnityEngine.Random.Range(0,bSkillNum);
        ShopImage.sprite = skillSprites[bSkill];
        if (bSkill==0 )
        {
            biname.text = "YF";
            bitext.text = "这是一个介绍（YF   ";
            bctext.text = YFSkill.Skillcoin.ToString();

        }
       if (YFskill)
       {
            GetComponent<YFSkill>().enabled = true;
       }

       //tuteng
        sSkill = UnityEngine.Random.Range(0, sSkillNum);
        ShopImages.sprite = sskillSprites[sSkill];
        if (sSkill == 0)
        {
            siname.text = "图腾+1";
            sitext.text = "这是一个介绍（TT   ";
            sctext.text = Onemore.Skillcoin.ToString();

        }
        if (TTskill)
        {
            GetComponent<Onemore>().enabled = true;
        }

        //装置
        DSkill = UnityEngine.Random.Range(0, DSkillNum);
        ShopImageD.sprite = DskillSprites[DSkill];
        if (DSkill == 0)
        {
            Diname.text = "冻结";
            Ditext.text = "这是一个介绍（D   ";
            Dctext.text = CoolSkill.Skillcoin.ToString();

        }
        //if (DJskill)
        //{
        //   // GetComponent<CoolSkill>().enabled = true;

        //}
    }
    public void MakeSureYF()
    {
        Sure.SetActive(true);
    }
    public void MakeSureTT()
    {
        SureTT.SetActive(true);
    }
    public void MakeSureD()
    {
        SureD.SetActive(true);
    }
    public void SureYesYF()
    {
        GameManager.CostCoin(YFSkill.Skillcoin);
        YFskill = true;
        Sure.SetActive(false);
    }
    public void SureYesTT()
    {
        GameManager.CostCoin(Onemore.Skillcoin);
        TTskill = true;
        SureTT.SetActive(false);
    }
    //public void SureYesD()
    //{
    //    GameManager.CostCoin(CoolSkill.Skillcoin);
    //    DJskill = true;
    //    SureD.SetActive(false);
    //}
    public void SureYesDJ()
    {
        Debug.Log("111111");
        if (!SkillButtonManager.Instance.CanPurchase)
        {
            Debug.Log("技能已满");
            return;
        }
        GameManager.CostCoin(CoolSkill.Skillcoin);
        DJskill = true;
        SkillButtonManager.Instance.ActivateNextSkill(skillSpriteDJ, () => { Debug.Log("Button"); GetComponent<CoolSkill>().enabled = true; });
        SureD.SetActive(false);
    }
 
}