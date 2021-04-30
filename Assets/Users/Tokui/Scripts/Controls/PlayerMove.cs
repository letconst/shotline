using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public FloatingJoystick inputMove; //左画面JoyStick
    float moveSpeed = 10.0f; //移動する速度
    float rotateSpeed = 1.0f;  //回転する速度

    void Update()
    {
        //左スティックでの縦移動
        this.transform.position += this.transform.forward * inputMove.Vertical * moveSpeed * Time.deltaTime;
        //左スティックでの横移動
        this.transform.position += this.transform.right * inputMove.Horizontal * moveSpeed * Time.deltaTime;
    }
}
