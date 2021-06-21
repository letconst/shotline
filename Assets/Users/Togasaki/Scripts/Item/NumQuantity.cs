using UnityEngine;
using UnityEngine.UI;

public class NumQuantity : SingletonMonoBehaviour<NumQuantity>
{
    private float FA = 0;
    private float currentNum = Projectile.BBnum;
    private float maxNum = 3f;

    private void Start()
    {
        FA = 0f;
        currentNum = 0;
        maxNum = 3f;
    }

    public void CulNum()
    {

        currentNum++;


        FA = (currentNum / maxNum) * 100f;

        FA = Mathf.Floor(FA);

        FA = FA / 100f;

        if (currentNum == maxNum)
        {
            FA = 0f;
            currentNum = 0;
            maxNum = 3f;
        }    

        GetComponent<Image>().fillAmount = FA;

    }

}