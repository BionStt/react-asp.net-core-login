namespace Service_Repository_Layer.Enums
{
    public enum ResponseCode
    {
        Error = 0, // Globally 0 will be indicating an error occured. See the exception message in the logs.
        Success = 1,
        Warning = 10,
        NotFoundInContext = 3,
        MissingInformation = 4,
        AlreadyExistsInContext = 5,
        CantBeDeleted = 6,
        UsernameNotInContext = 2,
        EmptyNullParameters = 7,
        WrongCredentials = 8,
        EmailNotConfirmed = 9,
        UserCannotLogin=10,
        UserDeleted=11
    }
}
