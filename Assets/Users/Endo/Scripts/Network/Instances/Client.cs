[System.Serializable]
public class Client
{
    public string uuid;

    public Client()
    {
        uuid = SelfPlayerData.PlayerUuid;
    }
}
