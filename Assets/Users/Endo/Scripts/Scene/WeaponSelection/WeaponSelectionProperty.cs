using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionProperty : SingletonMonoBehaviour<WeaponSelectionProperty>
{
    [SerializeField]
    private Button exitButton;

    public static Button ExitButton => Instance.exitButton;

    [SerializeField]
    private Button confirmButton;

    public static Button ConfirmButton => Instance.confirmButton;

    [SerializeField]
    private TimerScript timer;

    public static TimerScript Timer => Instance.timer;

    [SerializeField]
    private ScrollSnapSelector snapSelector;

    public static ScrollSnapSelector SnapSelector => Instance.snapSelector;

    [SerializeField]
    private Text roomIdText;

    public static Text RoomIdText => Instance.roomIdText;
}
