using UnityEngine;

public class RoomSelectionProperty : SingletonMonoBehaviour<RoomSelectionProperty>
{
    [SerializeField]
    private GameObject statusBgImage;

    public static GameObject StatusBgImage => Instance.statusBgImage;
}
