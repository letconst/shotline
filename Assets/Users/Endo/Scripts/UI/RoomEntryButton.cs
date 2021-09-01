using System;
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

    private int _selfIndex;

    private void Awake()
    {
        selfButton.onClick.AddListener(OnClick);
        _selfIndex = transform.GetSiblingIndex();
    }

    private void OnClick()
    {
        RoomData roomData = RoomSelectionController.Instance.GetRoomData(this);

        if (roomData == null) return;

        var req = new JoinRoomRequest
        {
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
