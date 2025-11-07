using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record EmailTemplateType(string Value)
    {
        public static readonly EmailTemplateType VerifyAccountEmail = new("VERIFYACCOUNTEMAIL");
        public static readonly EmailTemplateType ForgotPassword = new("FORGOTPASSWORD");
        public static readonly EmailTemplateType ModeratorCreated = new("MODERATORCREATED");

        public override string ToString() => Value;

        public static EmailTemplateType From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "VERIFYACCOUNTEMAIL" => VerifyAccountEmail,
                "FORGOTPASSWORD" => ForgotPassword,
                "MODERATORCREATED" => ModeratorCreated,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }
    }
}
