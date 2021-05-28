//動作テスト用のスクリプト

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public void OnClick()
    {
        RoundManager.RoundMove = false;
    }
}