namespace Service_Repository_Layer.Enums
{
    public enum AuthenticationResults
    {
        Error = 0,
        Success = 1,
        UsernameNotInContext = 2,
        EmptyNullParameters = 3,
        WrongCredentials = 4,
        EmailNotConfirmed = 5
    }
}
