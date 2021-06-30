using UnityEngine;
using UnityEngine.UI;

public class NumQuantity : SingletonMonoBehaviour<NumQuantity>
{
    private static float FA = 0;
    private static Image im;

    private void Start()
    {
        FA = 0;
        im = GetComponent<Image>();
    }

    public static void CulNum(float Max = 1)
    {
        ItemManager.currentNum++;

        FA = (ItemManager.currentNum / Max) * 100;

        FA = Mathf.Floor(FA);

        FA = FA / 100;

        if (ItemManager.currentNum == Max)
        {
            FA = 0;
        }

        im.fillAmount = FA;
    }

}