namespace Jwt.Refresh.Token.Domain.DataTransferObjects;

public class Token
{
    public string Id { get; set; }
    
    public string UserId { get; set; }    
    
    public string AccessToken { get; set; }
    
    public int Status { get; set; }
    public DateTimeOffset? Expires { get; set; }
    
    public int ExpiresMilliseconds
    {
        get
        {
            if (Expires.HasValue && Expires != DateTimeOffset.MinValue)
            {
                var remaining = (int)(Expires.Value - DateTimeOffset.UtcNow).TotalMilliseconds;
                return remaining > 0 ? remaining : 0;
            }

            return 0;
        }
    }
}