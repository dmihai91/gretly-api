using FaunaDB.Types;

public class FaunaResultDto
{
    [FaunaField("ref")]
    public RefV Ref { get; set; }

    [FaunaField("ts")]
    public LongV Ts { get; set; }

    [FaunaField("data")]
    public ObjectV Data { get; set; }
}