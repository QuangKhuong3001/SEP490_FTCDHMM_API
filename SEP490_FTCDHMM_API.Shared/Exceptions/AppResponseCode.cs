namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public record AppResponseCode(int StatusCode, string Code)
    {
        public static readonly AppResponseCode MISSING_ADMIN_ACCOUNT_CONFIG = new(101, "MISSING_ADMIN_ACCOUNT_CONFIG");
        public static readonly AppResponseCode INVALID_FILE = new(400, "INVALID_FILE");
        public static readonly AppResponseCode INVALID_ACTION = new(401, "INVALID_ACTION");
        public static readonly AppResponseCode EMAIL_NOT_CONFIRMED = new(402, "EMAIL_NOT_CONFIRMED");
        public static readonly AppResponseCode ACCOUNT_LOCKED = new(403, "ACCOUNT_LOCKED");
        public static readonly AppResponseCode INVALID_ACCOUNT_INFORMATION = new(404, "INVALID_ACCOUNT_INFORMATION");
        public static readonly AppResponseCode INVALID_USER_STATUS = new(405, "INVALID_USER_STATUS ");
        public static readonly AppResponseCode OTP_INVALID = new(406, "OTP_INVALID");
        public static readonly AppResponseCode INVALID_ROLE = new(407, "INVALID_ROLE");
        public static readonly AppResponseCode PASSWORD_CANNOT_BE_SAME_AS_OLD = new(408, "PASSWORD_CANNOT_BE_SAME_AS_OLD");
        public static readonly AppResponseCode UNAUTHORIZED = new(409, "UNAUTHORIZED");
        public static readonly AppResponseCode SECURITY_TOKEN_EXCEPTION = new(410, "SECURITY_TOKEN_EXCEPTION");
        public static readonly AppResponseCode ACCESS_DENIED = new(411, "ACCESS_DENIED");
        public static readonly AppResponseCode FORBIDDEN = new(412, "FORBIDDEN");
        public static readonly AppResponseCode NOT_FOUND = new(413, "NOT_FOUND");
        public static readonly AppResponseCode EMAIL_ALREADY_EXISTS = new(415, "EMAIL_ALREADY_EXISTS");
        public static readonly AppResponseCode ROLE_ALREADY_EXISTS = new(418, "ROLE_ALREADY_EXISTS");
        public static readonly AppResponseCode SERVICE_NOT_AVAILABLE = new(416, "SERVICE_NOT_AVAILABLE");
        public static readonly AppResponseCode NO_PERMISSION = new(417, "NO_PERMISSION");
        public static readonly AppResponseCode UNKNOWN_ERROR = new(500, "UNKNOWN_ERROR");
    }
}
