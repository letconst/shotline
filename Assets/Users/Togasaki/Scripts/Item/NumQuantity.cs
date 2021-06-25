using UnityEngine;
using UnityEngine.UI;

public class NumQuantity : SingletonMonoBehaviour<NumQuantity>
{
    private float FA = 0;
    public static float maxNum;

    private void Start()
    {
        FA = 0;
    }

    public void CulNum()
    {
        ItemManager.currentNum++;

        FA = (ItemManager.currentNum / maxNum) * 100;

        FA = Mathf.Floor(FA);

        FA = FA / 100;

        if (ItemManager.currentNum == maxNum)
        {
            FA = 0;
        }

        GetComponent<Image>().fillAmount = FA;
    }

}