namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public record AppResponseCode(int Status, string Message)
    {
        public static readonly AppResponseCode INVALID_ROLE = new(400, "Invalid role");
        public static readonly AppResponseCode INVALID_FILE = new(400, "Invalid file");
        public static readonly AppResponseCode INVALID_ACTION = new(400, "Invalid action");
        public static readonly AppResponseCode EMAIL_NOT_CONFIRMED = new(400, "Email not confirmed");
        public static readonly AppResponseCode ACCOUNT_LOCKED = new(400, "Account locked");
        public static readonly AppResponseCode INVALID_ACCOUNT_INFORMATION = new(400, "Invalid account information");
        public static readonly AppResponseCode OTP_INVALID = new(400, "OTP is invalid or expired");
        public static readonly AppResponseCode PASSWORD_CANNOT_BE_SAME_AS_OLD = new(400, "Password cannot be same as old");
        public static readonly AppResponseCode UNAUTHORIZED = new(401, "Unauthorized");
        public static readonly AppResponseCode SECURITY_TOKE_EXCEPTION = new(401, "Security Token Exception");
        public static readonly AppResponseCode ACCESS_DENIED = new(403, "Access Denied");
        public static readonly AppResponseCode FORBIDDEN = new(403, "Forbidden");
        public static readonly AppResponseCode NOT_FOUND = new(404, "Not Found");
        public static readonly AppResponseCode EMAIL_ALREADY_EXISTS = new(409, "Email already exists");
        public static readonly AppResponseCode SERVICE_NOT_AVAILABLE = new(421, "Service Not Available");
        public static readonly AppResponseCode UNKNOW_ERROR = new(500, "Unknow error");
    }
}
