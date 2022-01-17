namespace MyPlushBuddy.Api.Services
{
    public interface IUserInfoService
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

    }
}