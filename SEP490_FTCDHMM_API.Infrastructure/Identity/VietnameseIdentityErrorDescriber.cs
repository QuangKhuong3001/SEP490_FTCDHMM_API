namespace SEP490_FTCDHMM_API.Infrastructure.Identity
{
    using Microsoft.AspNetCore.Identity;

    public class VietnameseIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "Mật khẩu hiện tại không chính xác." };

        public override IdentityError InvalidToken()
            => new() { Code = nameof(InvalidToken), Description = "Mã xác thực không hợp lệ." };

        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"Mật khẩu phải có ít nhất {length} ký tự." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt." };

        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "Mật khẩu phải chứa ít nhất một chữ số (0-9)." };

        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "Mật khẩu phải chứa ít nhất một chữ thường (a-z)." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "Mật khẩu phải chứa ít nhất một chữ in hoa (A-Z)." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"Tên đăng nhập '{userName}' đã tồn tại." };

        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' đã tồn tại trong hệ thống." };

        public override IdentityError InvalidEmail(string? email)
            => new() { Code = nameof(InvalidEmail), Description = $"Email '{email}' không hợp lệ." };
    }

}
