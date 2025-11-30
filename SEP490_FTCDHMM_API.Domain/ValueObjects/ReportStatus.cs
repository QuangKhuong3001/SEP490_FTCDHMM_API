using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record ReportStatus(string Value)
    {
        public static readonly ReportStatus Pending = new("PENDING");
        public static readonly ReportStatus Approved = new("APPROVED");
        public static readonly ReportStatus Rejected = new("REJECTED");

        public override string ToString() => Value;

        public static ReportStatus From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new AppException(AppResponseCode.INVALID_ACTION, "Trạng thái không được để trống.");

            return value.Trim().ToUpperInvariant() switch
            {
                "PENDING" => Pending,
                "APPROVED" => Approved,
                "REJECTED" => Rejected,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Trạng thái report không hợp lệ.")
            };
        }


    }
}
