namespace SEP490_FTCDHMM_API.Domain.Services
{
    /// <summary>
    /// Tính toán tuổi người dùng từ ngày sinh.
    /// </summary>
    public static class AgeCalculator
    {
        public static int Calculate(DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age < 0 ? 0 : age;
        }
    }
}
