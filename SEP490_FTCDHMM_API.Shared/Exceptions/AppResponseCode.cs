namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public record AppResponseCode(int StatusCode, string Message)
    {
        public static readonly AppResponseCode MISSING_ADMIN_ACCOUNT_CONFIG = new(101, "Thiếu cấu hình tài khoản quản trị viên");
        public static readonly AppResponseCode INVALID_FILE = new(400, "Tệp tin không hợp lệ");
        public static readonly AppResponseCode INVALID_ACTION = new(401, "Hành động không hợp lệ");
        public static readonly AppResponseCode EMAIL_NOT_CONFIRMED = new(402, "Email chưa được xác thực");
        public static readonly AppResponseCode ACCOUNT_LOCKED = new(403, "Tài khoản đã bị khóa");
        public static readonly AppResponseCode INVALID_ACCOUNT_INFORMATION = new(404, "Tài khoản không hợp lệ hoặc không tồn tại");
        public static readonly AppResponseCode DUPLICATE = new(405, "Dữ liệu bị trùng lặp");
        public static readonly AppResponseCode OTP_INVALID = new(406, "Mã OTP không hợp lệ hoặc đã hết hạn");
        public static readonly AppResponseCode INVALID_ROLE = new(407, "Vai trò không hợp lệ");
        public static readonly AppResponseCode PASSWORD_CANNOT_BE_SAME_AS_OLD = new(408, "Mật khẩu mới không được trùng với mật khẩu cũ");
        public static readonly AppResponseCode UNAUTHORIZED = new(409, "Chưa được xác thực");
        public static readonly AppResponseCode SECURITY_TOKEN_EXCEPTION = new(410, "Lỗi xác thực token bảo mật");
        public static readonly AppResponseCode ACCESS_DENIED = new(411, "Không có quyền truy cập");
        public static readonly AppResponseCode FORBIDDEN = new(412, "Bạn không có quyền thực hiện yều cầu này");
        public static readonly AppResponseCode NOT_FOUND = new(413, "Không tìm thấy dữ liệu yêu cầu");
        public static readonly AppResponseCode EXISTS = new(415, "Dữ liệu đã tồn tại trong hệ thống");
        public static readonly AppResponseCode SERVICE_NOT_AVAILABLE = new(416, "Dịch vụ tạm thời không khả dụng");
        public static readonly AppResponseCode MISSING_REQUIRED_NUTRIENTS = new(419, "Thiếu các chất dinh dưỡng bắt buộc");
        public static readonly AppResponseCode MISSING_GENDER = new(421, "Thiếu thông tin giới tính");
        public static readonly AppResponseCode UNKNOWN_ERROR = new(500, "Đã xảy ra lỗi không xác định");
    }
}
