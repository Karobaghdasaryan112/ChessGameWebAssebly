namespace BlazorServerSideClient.Data.IdentityModels;

public class AspNetUsers
{
    public string Id{get;set;}
    public string FirstName{get;set;}
    public string LastName{get;set;}
    public string RefreshToken{get;set;}
    public string AccessToken{get;set;}
    public string RefreshTokenExpiryTime{get;set;}
    public DateTime CreatedAt{get;set;}
    public DateTime UpdatedAt{get;set;}
    public string UserName{get;set;}
    public string NormalizedUserName{get;set;}
    public bool EmailConfirmed{get;set;}
    public string PasswordHash{get;set;}
    public string SecurityStamp{get;set;}
    public string ConcurrencyStamp{get;set;}
    public string PhoneNumber{get;set;}
    public string PhoneNumberConfirmed{get;set;}
    public string TwoFactorEnabled{get;set;}
    public string LockoutEnd{get;set;}
    public string LockoutEnabled{get;set;}
    public int  AccessFailedCount{get;set;}
    public bool LoggedIn{get;set;}
    public string EmailAddress{get;set;}
    
}