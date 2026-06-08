using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class fish : MonoBehaviour
{
    public static fish _instance;
    public static fish instance
    {
        get { return _instance; }
    } 

    public GameObject[] fishPres;
   // public int fishSpeed=3;
    public Transform[] fishMakers;
    public Transform fishParent;

    public int gold;
    void Awake()
    {
         _instance = this; 
    }
    void Start()
    {
        InvokeRepeating("FishMaker", 0, 0.3f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FishMaker() 
    {
        int index = Random.Range(0, fishMakers.Length);
        int indexFishPre=Random.Range(0, fishPres.Length);
        int fishNum=fishPres[indexFishPre].GetComponent<FishAttrbute>().fishNumber;
        float angle = Random.Range(-45,45);
        StartCoroutine(MakeOneFish(indexFishPre, index,fishNum,angle));
    }
    IEnumerator MakeOneFish(int fishPre,int fishPoint,int fishNum,float angle )
    {
        for (int i = 0; i < fishNum; i++)
        {
            GameObject fish = Instantiate(fishPres[fishPre]);
            fish.transform.position = fishMakers[fishPoint].position;
            fish.transform.SetParent(fishParent, false);
            fish.GetComponent<SpriteRenderer>().sortingOrder += i;
            fish.transform.rotation = Quaternion.Euler(0, 0, fishMakers[fishPoint].eulerAngles.z + angle);
       
            float eulerZ = fish.transform.eulerAngles.z;
            float normalizedZ = eulerZ > 180 ? eulerZ - 360 : eulerZ;
            if (normalizedZ > -90 && normalizedZ < 90)
                fish.GetComponent<SpriteRenderer>().flipY = false;
            else
                fish.GetComponent<SpriteRenderer>().flipY = true;
            yield return new WaitForSeconds(0.3f);
        }
       
    }
}
