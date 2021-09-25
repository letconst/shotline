using Cinemachine;
using UnityEngine;

public class MainGameInitializer : MonoBehaviour, IManagedMethod
{
    [SerializeField, Header("生成する対戦相手プレハブ")]
    private GameObject rivalPrefab;

    [SerializeField, Header("初期位置用Virtual CameraのTransform")]
    private Transform vcam1Trf;

    [SerializeField, Header("プレイヤー追従用Virtual CameraのTransform")]
    private Transform vcam2Trf;

    [SerializeField, Header("プレイヤー追従用Virtual Camera")]
    private CinemachineVirtualCamera vcam2;

    private GameObject _playerObject;

    private CinemachineTransposer _vcam2Transposer;

    private Transform _playerTrf;
    private Transform _rivalTrf;

    public void ManagedStart()
    {
        _vcam2Transposer = vcam2.GetCinemachineComponent<CinemachineTransposer>();
        _playerObject    = GameObject.FindGameObjectWithTag("Player");
        _playerObject.SetActive(false);

        HostOrGuestInit();

        _playerObject.SetActive(true);
        SystemUIManager.SetInputBlockerVisibility(false);

        ApplyWeaponData();
    }

    public void ManagedUpdate()
    {
    }

    /// <summary>
    /// 1P2Pの設定を行う
    /// </summary>
    private void HostOrGuestInit()
    {
        // 1P
        if (NetworkManager.IsOwner)
        {
            // プレイヤー追従用カメラを反転
            vcam2Trf.RotateAround(_playerObject.transform.position, Vector3.up, 180);

            // 射線ゲージも逆になるため反転
            MainGameProperty.Instance.LineGaugeObject.transform.Rotate(Vector3.forward, 180);

            //1P用射線ゲージを表示
            MainGameProperty.Instance.LineGaugeObject.SetActive(true);

            // 2Pの方向矢印を表示
            MainGameProperty.RivalDirection2P.SetActive(true);

            // 初期位置設定
            _playerObject.transform.position = MainGameProperty.Instance.startPos1P.position;

            // 相手オブジェクト生成
            GameObject rivalObject =
                Instantiate(rivalPrefab, MainGameProperty.Instance.startPos2P.position, Quaternion.identity);

            MainGameController.rivalTrf = rivalObject.transform;

            _playerTrf = _playerObject.transform.Find("player_1");
            _rivalTrf  = rivalObject.transform.Find("player_2");

            // 表示オブジェクト選択
            _playerTrf.parent.Find("player_2").gameObject.SetActive(false);
            _rivalTrf.parent.Find("player_1").gameObject.SetActive(false);

            MainGameController.linePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL1");
            MainGameController.bulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
            MainGameController.rivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
        }
        // 2P
        else
        {
            // 初期位置用カメラおよび追従用カメラのオフセット値を反転
            Camera.main.transform.RotateAround(_playerObject.transform.position, Vector3.up, 180);
            vcam1Trf.RotateAround(_playerObject.transform.position, Vector3.up, 180);
            _vcam2Transposer.m_FollowOffset.z *= -1;

            //2P用射線ゲージを表示
            MainGameProperty.Instance.LineGaugeObject2.SetActive(true);

            // 1Pの方向矢印を表示
            MainGameProperty.RivalDirection1P.SetActive(true);

            // 初期位置設定
            _playerObject.transform.position = MainGameProperty.Instance.startPos2P.position;

            // 相手オブジェクト生成
            GameObject rivalObject =
                Instantiate(rivalPrefab, MainGameProperty.Instance.startPos1P.position, Quaternion.identity);

            MainGameController.rivalTrf = rivalObject.transform;

            _playerTrf = _playerObject.transform.Find("player_2");
            _rivalTrf  = rivalObject.transform.Find("player_1");

            // 表示オブジェクト選択
            _playerTrf.parent.Find("player_1").gameObject.SetActive(false);
            _rivalTrf.parent.Find("player_2").gameObject.SetActive(false);

            MainGameController.linePrefab        = Resources.Load<GameObject>("Prefabs/Line_PL2");
            MainGameController.bulletPrefab      = Resources.Load<GameObject>("Prefabs/Bullet_PL2");
            MainGameController.rivalBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet_PL1");
        }
    }

    /// <summary>
    /// 選択されている武器データに応じて動作切り替えを行う
    /// </summary>
    private void ApplyWeaponData()
    {
        WeaponDatas selectedWeapon = WeaponManager.SelectWeapon;

        if (selectedWeapon == null) return;

        bool isLinearDraw = selectedWeapon.ShotType == ShotType.Linear;

        // 各種パラメーター設定
        Projectile.OriginSpeed                    = selectedWeapon.BulletSpeed / 10;
        LinearDraw._linearDrawOn                  = isLinearDraw;
        LinearDraw._isLinearDraw                  = isLinearDraw;
        LineGaugeController.Instance.MaxLinePower = selectedWeapon.GaugeMax;
        LineGaugeController.Instance.HealingGauge = selectedWeapon.GaugeRecovery / 10000;

        var playerProp = _playerTrf.GetComponent<PlayerProperty>();

        // 選択武器モデルを表示
        if (isLinearDraw)
        {
            playerProp.DrawLineGun.SetActive(true);
        }
        else
        {
            playerProp.LinearLineGun.SetActive(true);
        }
    }
}
