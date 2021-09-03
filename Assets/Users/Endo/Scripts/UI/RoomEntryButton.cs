using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum RoomStatus
{
    Empty,
    Matching,
    InBattle
}

[RequireComponent(typeof(Button))]
public class RoomEntryButton : MonoBehaviour
{
    [SerializeField]
    private Text titleText;

    [SerializeField]
    private GameObject statusTextObj;

    [SerializeField]
    private Text playerCountText;

    [SerializeField]
    private Button selfButton;

    private int  _selfIndex;
    private Text _statusText;

    private void Awake()
    {
        selfButton.onClick.AddListener(OnClick);
        _selfIndex  = transform.GetSiblingIndex();
        _statusText = SystemProperty.StatusText;
    }

    private void OnClick()
    {
        RoomData roomData = RoomSelectionController.Instance.GetRoomData(this);

        if (roomData == null) return;

        _statusText.text = "参加中…";
        RoomSelectionProperty.StatusBgImage.SetActive(true);

        var req = new JoinRoomRequest
        {
            Client   = new Client(),
            RoomUuid = roomData.RoomUuid
        };

        NetworkManager.Emit(req);
    }

    /// <summary>
    /// ボタンの情報を設定する
    /// </summary>
    /// <param name="playerCount">ルーム内のプレイヤー数</param>
    /// <param name="isInBattle">対戦中か</param>
    public void UpdateContent(int playerCount, bool isInBattle)
    {
        var titleBuilder = new StringBuilder();
        titleBuilder.Append("ルーム");
        titleBuilder.Append(_selfIndex);
        titleText.text = titleBuilder.ToString();

        statusTextObj.SetActive(isInBattle);

        playerCountText.text = playerCount.ToString();
    }
}
