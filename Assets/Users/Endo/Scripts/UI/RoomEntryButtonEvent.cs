using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntryButtonEvent : MonoBehaviour
{
    [SerializeField]
    private Text titleText;

    [SerializeField]
    private Text statusText;

    [SerializeField]
    private Text playerCountText;

    public enum RoomStatus
    {
        Empty,
        Matching,
        InBattle
    }

    public void UpdateContent(int roomNumber, RoomStatus roomStatus, int playerCount)
    {
        var titleBuilder = new StringBuilder();
        titleBuilder.Append("ルーム");
        titleBuilder.Append(roomNumber.ToString());
    }
}
