using UnityEngine;
using UnityEngine.UI;

public class BulletManager : SingletonMonoBehaviour<BulletManager>
{
    [SerializeField]
    private Button shotBtn;

    public Button ShotBtn => shotBtn;

    protected virtual void Start()
    {

    }

}
