namespace MusicStore.Entities;

public class AppSettings
{
    public Jwt Jwt { get; set; } = default!;
    public Smtp Smtp { get; set; } = default!;
    public AzureOpenAI AzureOpenAI { get; set; } = default!;
}

public class Jwt
{
    public string Key { get; set; } = string.Empty;
    public int LifetimeInSeconds { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

public class Smtp
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string FromName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}

public class AzureOpenAI
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
}