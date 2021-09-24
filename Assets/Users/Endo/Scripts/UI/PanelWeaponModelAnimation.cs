using System.Collections.Generic;
using UnityEngine;

public class PanelWeaponModelAnimation : MonoBehaviour, IManagedMethod
{
    public enum RotateDirection
    {
        Left,
        Right
    }

    [SerializeField, Header("回転方向")]
    private RotateDirection rotateDirection;

    [SerializeField, Header("回転速度"), Range(0, 300)]
    private float rotateSpeed = 100;

    private List<Transform> _modelTrfs;

    public void ManagedStart()
    {
        _modelTrfs = WeaponManager.weaponModels;
    }

    public void ManagedUpdate()
    {
        RotateAnimation();
    }

    /// <summary>
    /// すべてのパネル上の武器モデルを回転させる
    /// </summary>
    private void RotateAnimation()
    {
        // 回転速度および方向を設定
        float speed = rotateSpeed * Time.deltaTime;
        speed *= rotateDirection == RotateDirection.Left ? 1 : -1;

        foreach (Transform modelTrf in _modelTrfs)
        {
            modelTrf.Rotate(new Vector3(0, speed, 0));
        }
    }
}
