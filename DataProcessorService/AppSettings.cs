namespace DataProcessorService;

public class AppSettings
{
    public float UpdateInterval { get; set; }
}

public class ReceiverSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string QueueName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
}