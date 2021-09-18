using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [SerializeField]
    Image image1;

    [SerializeField]
    Text TimerText;

    public float waitTime = 60.0f;

    public float NowTime = 0;

    public bool TimeStop;

    void Start()
    {
        TimeStop = false;

        NowTime = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount();

        TimeColorChange();
    }

    private void TimeCount()
    {
        if (0 < NowTime)
        {
            image1.fillAmount -= 1.0f / waitTime * Time.deltaTime;

            NowTime -= Time.deltaTime;

            TimerText.text = NowTime.ToString("f1");
        }
        else if (NowTime < 0)
        {
            TimeStop = true;
        }
    }

    private void TimeColorChange()
    {
        // Fill Amountによってゲージの色を変える
        if (image1.fillAmount > 0.5f)
        {
            image1.color = Color.green;
        }
        else if (image1.fillAmount > 0.25f)
        {
            image1.color = new Color(1f, 0.67f, 0f);
        }
        else
        {
            image1.color = Color.red;
        }
    }
}