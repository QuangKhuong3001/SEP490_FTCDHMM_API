using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record OtpPurpose(string Value)
    {
        public static readonly OtpPurpose VerifyAccountEmail = new("VERIFYACCOUNTEMAIL");
        public static readonly OtpPurpose ForgotPassword = new("FORGOTPASSWORD");

        public override string ToString() => Value;

        public static OtpPurpose From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "VERIFYACCOUNTEMAIL" => VerifyAccountEmail,
                "FORGOTPASSWORD" => ForgotPassword,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }

    }
}