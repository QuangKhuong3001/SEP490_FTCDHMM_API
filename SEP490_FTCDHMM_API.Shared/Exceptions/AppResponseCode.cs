namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public record AppResponseCode(int Status, string Code, string Message)
    {
        public static readonly AppResponseCode INVALID_ROLE = new(400, "INVALID_ROLE", "Invalid role");
        public static readonly AppResponseCode INVALID_FILE = new(400, "INVALID_FILE", "Invalid file");
        public static readonly AppResponseCode INVALID_ACTION = new(400, "INVALID_ACTION", "Invalid action");
        public static readonly AppResponseCode EMAIL_NOT_CONFIRMED = new(400, "EMAIL_NOT_CONFIRMED", "Email not confirmed");
        public static readonly AppResponseCode ACCOUNT_LOCKED = new(400, "ACCOUNT_LOCKED", "Account locked");
        public static readonly AppResponseCode INVALID_ACCOUNT_INFORMATION = new(400, "INVALID_ACCOUNT_INFORMATION", "Invalid account information");
        public static readonly AppResponseCode INVALID_USER_STATUS = new(400, "INVALID_USER_STATUS ", "Invalid user status");
        public static readonly AppResponseCode OTP_INVALID = new(400, "OTP_INVALID", "OTP is invalid or expired");
        public static readonly AppResponseCode PASSWORD_CANNOT_BE_SAME_AS_OLD = new(400, "PASSWORD_CANNOT_BE_SAME_AS_OLD", "Password cannot be same as old");
        public static readonly AppResponseCode UNAUTHORIZED = new(401, "UNAUTHORIZED", "Unauthorized");
        public static readonly AppResponseCode SECURITY_TOKEN_EXCEPTION = new(401, "SECURITY_TOKEN_EXCEPTION", "Security Token Exception");
        public static readonly AppResponseCode ACCESS_DENIED = new(403, "ACCESS_DENIED", "Access Denied");
        public static readonly AppResponseCode FORBIDDEN = new(403, "FORBIDDEN", "Forbidden");
        public static readonly AppResponseCode NOT_FOUND = new(404, "NOT_FOUND", "Not Found");
        public static readonly AppResponseCode EMAIL_ALREADY_EXISTS = new(409, "EMAIL_ALREADY_EXISTS", "Email already exists");
        public static readonly AppResponseCode SERVICE_NOT_AVAILABLE = new(421, "SERVICE_NOT_AVAILABLE", "Service Not Available");
        public static readonly AppResponseCode UNKNOW_ERROR = new(500, "UNKNOW_ERROR", "Unknow error");
    }
}
