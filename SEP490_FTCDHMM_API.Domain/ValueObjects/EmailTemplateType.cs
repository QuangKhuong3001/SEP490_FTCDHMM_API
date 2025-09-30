using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record EmailTemplateType(string Value)
    {
        public static readonly EmailTemplateType VerifyAccountEmail = new("VerifyAccountEmail");
        public static readonly EmailTemplateType ForgotPassword = new("ForgotPassword");
        public static readonly EmailTemplateType ModeratorCreated = new("ModeratorCreated");

        public override string ToString() => Value;

        public static EmailTemplateType From(string value) =>
            value switch
            {
                "VerifyAccountEmail" => VerifyAccountEmail,
                "ForgotPassword" => ForgotPassword,
                "ModeratorCreated" => ModeratorCreated,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
    }
}
