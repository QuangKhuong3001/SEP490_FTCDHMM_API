namespace SEP490_FTCDHMM_API.Domain.Services
{
    public static class UsernameHelper
    {
        public static string ExtractUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return string.Empty;

            int index = userName.LastIndexOf('@');

            if (index < 0)
                return userName;

            return userName.Substring(0, index);
        }

        public static string IncrementUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return "1";
            int index = userName.Length - 1;
            while (index >= 0 && char.IsDigit(userName[index]))
            {
                index--;
            }
            string prefix = userName.Substring(0, index + 1);
            string numberPart = userName.Substring(index + 1);
            if (string.IsNullOrEmpty(numberPart))
            {
                return prefix + "1";
            }
            else
            {
                if (int.TryParse(numberPart, out int number))
                {
                    number++;
                    return prefix + number.ToString();
                }
                else
                {
                    return userName + "1";
                }
            }
        }
    }
}
