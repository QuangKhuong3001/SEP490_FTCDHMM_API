namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record PermissionValue(string Domain, string Action)
    {
        public override string ToString() => $"{Domain}:{Action}";

        public static readonly PermissionValue User_Create = new("Quản lí người dùng", "Tạo");
        public static readonly PermissionValue User_View = new("Quản lí người dùng", "Xem");
        public static readonly PermissionValue User_Update = new("Quản lí người dùng", "Cập nhật");
        public static readonly PermissionValue User_Delete = new("Quản lí người dùng", "Xóa");

        public static readonly PermissionValue Ingredient_Create = new("Nguyên liệu", "Tạo");
        public static readonly PermissionValue Ingredient_Update = new("Nguyên liệu", "Cập nhật");
        public static readonly PermissionValue Ingredient_Delete = new("Nguyên liệu", "Xóa");
        public static readonly PermissionValue Ingredient_ManagerView = new("Nguyên liệu", "Xem với quyền");

        public static readonly PermissionValue Label_Create = new("Nhãn món ăn", "Tạo");
        public static readonly PermissionValue Label_Update = new("Nhãn món ăn", "Cập nhật");
        public static readonly PermissionValue Label_Delete = new("Nhãn món ăn", "Xóa");

        public static readonly PermissionValue IngredientCategory_Create = new("Nhóm nguyên liệu", "Tạo");
        public static readonly PermissionValue IngredientCategory_Delete = new("Nhóm nguyên liệu", "Xóa");

        public static readonly PermissionValue HealthGoal_Create = new("Mục tiêu sức khỏe", "Tạo");
        public static readonly PermissionValue HealthGoal_Update = new("Mục tiêu sức khỏe", "Cập nhật");
        public static readonly PermissionValue HealthGoal_Delete = new("Mục tiêu sức khỏe", "Xóa");

        public static readonly PermissionValue Comment_Create = new("Bình luận", "Tạo");
        public static readonly PermissionValue Comment_Delete = new("Bình luận", "Xóa");
        public static readonly PermissionValue Comment_Update = new("Bình luận", "Cập nhật");

        public static readonly PermissionValue Rating_Create = new("Đánh giá", "Tạo");
        public static readonly PermissionValue Rating_Delete = new("Đánh giá", "Xóa");
        public static readonly PermissionValue Rating_Update = new("Đánh giá", "Cập nhật");

        public static readonly PermissionValue Recipe_Lock = new("Công thức", "Khóa");
        public static readonly PermissionValue Recipe_Delete = new("Công thức", "Xóa");
        public static readonly PermissionValue Recipe_Approve = new("Công thức", "Xác nhận");
        public static readonly PermissionValue Recipe_ManagementView = new("Công thức", "Xem với quyền");

        public static readonly PermissionValue Report_View = new("Báo cáo", "Xem");
        public static readonly PermissionValue Report_Approve = new("Báo cáo", "Xác nhận");
        public static readonly PermissionValue Report_Reject = new("Báo cáo", "Từ chối");
        public static IEnumerable<PermissionValue> All => new[]
        {
            User_Create,
            User_View,
            User_Update,
            User_Delete,
            Ingredient_Create,
            Ingredient_Update,
            Ingredient_Delete,
            Label_Create,
            Label_Delete,
            Label_Update,
            IngredientCategory_Create,
            IngredientCategory_Delete,
            HealthGoal_Create,
            HealthGoal_Delete,
            HealthGoal_Update,
            Comment_Create,
            Comment_Delete,
            Comment_Update,
            Rating_Create,
            Rating_Delete,
            Rating_Update,
            Recipe_Approve,
            Recipe_Delete,
            Recipe_Lock,
            Recipe_ManagementView,
            Rating_Update,
            Report_View,
            Report_Approve,
            Report_Reject,
            Ingredient_ManagerView
        };
    }
}
