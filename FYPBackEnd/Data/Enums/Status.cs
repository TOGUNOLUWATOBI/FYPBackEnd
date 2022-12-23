namespace FYPBackEnd.Data.Enums
{
    public enum TransactonStatus
    {
        Successful,
        UnSuccessful,
        Pending
    }

    public enum Status
    {

        Successful,
        UnSuccessful
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Blacklisted
    }

    public enum OtpPurpose
    {
        UserVerification,
        PasswordReset
    }
}
