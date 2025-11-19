namespace SEP490_FTCDHMM_API.Application.Common.Helpers
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
    }
}
