using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlaySE(SELabel.ボタン音29);
    }
}
