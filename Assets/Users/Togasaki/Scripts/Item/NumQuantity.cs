using UnityEngine;
using UnityEngine.UI;

public class NumQuantity : SingletonMonoBehaviour<NumQuantity>
{
    [SerializeField]
    private Image im;

    private static float FA = 0;

    private void Start()
    {
        FA = 0;
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

        Instance.im.fillAmount = FA;

    }

}
