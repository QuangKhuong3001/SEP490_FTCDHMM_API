using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record OtpPurpose(string Value)
    {
        public static readonly OtpPurpose ConfirmAccountEmail = new("ConfirmAccountEmail");
        public static readonly OtpPurpose ForgotPassword = new("ForgotPassword");

        public override string ToString() => Value;

        public static OtpPurpose From(string value) =>
            value switch
            {
                "ConfirmAccountEmail" => ConfirmAccountEmail,
                "ForgotPassword" => ForgotPassword,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
    }
}