namespace FileParserService;

public class AppSettings
{
    public string PathToXmlDir { get; set; }
    public float UpdateInterval { get; set; }
}

public class PublisherSettings
{
    public string HostName { get; set; }
    public string QueueName { get; set; }
}