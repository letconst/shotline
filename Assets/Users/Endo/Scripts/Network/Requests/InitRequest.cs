public class InitRequest : RequestBase
{
    public string Uuid;

    public InitRequest()
    {
        SetType(EventType.Init);
    }
}
