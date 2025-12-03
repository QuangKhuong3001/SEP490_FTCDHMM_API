using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutrientUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionDomains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nutrients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VietnameseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMacroNutrient = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutrients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nutrients_NutrientUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "NutrientUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionDomainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionActions_PermissionDomains_PermissionDomainId",
                        column: x => x.PermissionDomainId,
                        principalTable: "PermissionDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionActionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_PermissionActions_PermissionActionId",
                        column: x => x.PermissionActionId,
                        principalTable: "PermissionActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "CommentMentions",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MentionedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentMentions", x => new { x.CommentId, x.MentionedUserId });
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CookingStepImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CookingStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CookingStepImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CookingSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CookingSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomHealthGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomHealthGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthGoalTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomHealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'ABSOLUTE'"),
                    MinValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MinEnergyPct = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MaxEnergyPct = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthGoalTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthGoalTargets_CustomHealthGoals_CustomHealthGoalId",
                        column: x => x.CustomHealthGoalId,
                        principalTable: "CustomHealthGoals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HealthGoalTargets_HealthGoals_HealthGoalId",
                        column: x => x.HealthGoalId,
                        principalTable: "HealthGoals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DraftCookingStepImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DraftCookingStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftCookingStepImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DraftCookingSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftCookingSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DraftRecipeIngredients",
                columns: table => new
                {
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityGram = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipeIngredients", x => new { x.DraftRecipeId, x.IngredientId });
                });

            migrationBuilder.CreateTable(
                name: "DraftRecipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CookTime = table.Column<int>(type: "int", nullable: false),
                    Ration = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_DraftRecipes_DraftRecipeId",
                        column: x => x.DraftRecipeId,
                        principalTable: "DraftRecipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DraftRecipeUserTags",
                columns: table => new
                {
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaggedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipeUserTags", x => new { x.DraftRecipeId, x.TaggedUserId });
                    table.ForeignKey(
                        name: "FK_DraftRecipeUserTags_DraftRecipes_DraftRecipeId",
                        column: x => x.DraftRecipeId,
                        principalTable: "DraftRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailOtps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SentToId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOtps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UploadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Calories = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'MALE'"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ActivityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'MODERATE'"),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LockReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Images_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientCategoryLink",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientCategoryLink", x => new { x.CategoriesId, x.IngredientsId });
                    table.ForeignKey(
                        name: "FK_IngredientCategoryLink_IngredientCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "IngredientCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientCategoryLink_Ingredients_IngredientsId",
                        column: x => x.IngredientsId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientNutrients",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    MedianValue = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientNutrients", x => new { x.IngredientId, x.NutrientId });
                    table.ForeignKey(
                        name: "FK_IngredientNutrients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientNutrients_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Difficulty = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'MEDIUM'"),
                    CookTime = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ration = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Calories = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'POSTED'"),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AvgRating = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recipes_Recipes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recipes_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReviewedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    FollowerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FolloweeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => new { x.FollowerId, x.FolloweeId });
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_FolloweeId",
                        column: x => x.FolloweeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserHealthGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomHealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'CUSTOM'"),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHealthGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHealthGoals_CustomHealthGoals_CustomHealthGoalId",
                        column: x => x.CustomHealthGoalId,
                        principalTable: "CustomHealthGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHealthGoals_HealthGoals_HealthGoalId",
                        column: x => x.HealthGoalId,
                        principalTable: "HealthGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHealthGoals_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserHealthGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserHealthMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    HeightCm = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BMI = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BodyFatPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MuscleMassKg = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ActivityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'MODERATE'"),
                    BMR = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    TDEE = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHealthMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHealthMetrics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserIngredientRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IngredientCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'ALLERGY'"),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExpiredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIngredientRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserIngredientRestrictions_IngredientCategories_IngredientCategoryId",
                        column: x => x.IngredientCategoryId,
                        principalTable: "IngredientCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserIngredientRestrictions_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserIngredientRestrictions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityGram = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => new { x.RecipeId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeLabels",
                columns: table => new
                {
                    LabelsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeLabels", x => new { x.LabelsId, x.RecipesId });
                    table.ForeignKey(
                        name: "FK_RecipeLabels_Labels_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeLabels_Recipes_RecipesId",
                        column: x => x.RecipesId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeNutritionAggregates",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AmountPerServing = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ComputedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeNutritionAggregates", x => new { x.RecipeId, x.NutrientId });
                    table.ForeignKey(
                        name: "FK_RecipeNutritionAggregates_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeNutritionAggregates_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeUserTags",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaggedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeUserTags", x => new { x.RecipeId, x.TaggedUserId });
                    table.ForeignKey(
                        name: "FK_RecipeUserTags_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeUserTags_Users_TaggedUserId",
                        column: x => x.TaggedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRecipeViews",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecipeViews", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_UserRecipeViews_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRecipeViews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSaveRecipes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSaveRecipes", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_UserSaveRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSaveRecipes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "ContentType", "CreatedAtUTC", "IsDeleted", "Key", "UploadedById" },
                values: new object[] { new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"), "image/png", new DateTime(2025, 12, 3, 19, 56, 32, 742, DateTimeKind.Utc).AddTicks(707), false, "images/default/no-image.png", null });

            migrationBuilder.InsertData(
                table: "IngredientCategories",
                columns: new[] { "Id", "Name", "isDeleted" },
                values: new object[,]
                {
                    { new Guid("0b391ac2-8440-b318-afc4-045c61aee15c"), "Gia vị", false },
                    { new Guid("0ef727db-5be6-f820-ec21-5d1d34876db5"), "Đồ uống", false },
                    { new Guid("2510563b-4a1a-36f8-3eee-0081aeb1b15c"), "Đồ hộp / chế biến sẵn", false },
                    { new Guid("36e6cb97-3dc3-c518-e22c-4d2804e5a65d"), "Hải sản", false },
                    { new Guid("3741c8e7-ce2b-c477-4e45-169cec441664"), "Rau củ quả", false },
                    { new Guid("447e8fa6-250f-0c6c-190e-d7ec87e91745"), "Nguyên liệu khác", false },
                    { new Guid("5b8fd31b-bca6-bd0f-4bd4-1008a83f4385"), "Trứng", false },
                    { new Guid("7814e36f-6b23-5d6b-f0b7-bc34f75fa315"), "Ngũ cốc", false },
                    { new Guid("bcfbe809-1ee1-771d-e271-0f959bfd67f6"), "Đồ ngọt", false },
                    { new Guid("db5072d7-9bc0-6d4a-8d33-3b18239c40f6"), "Dầu mỡ", false },
                    { new Guid("e7f53468-c971-6d4d-7e56-1e50702495fd"), "Thịt", false }
                });

            migrationBuilder.InsertData(
                table: "Labels",
                columns: new[] { "Id", "ColorCode", "DraftRecipeId", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("133554ee-b8bf-0518-a055-4097baea7b64"), "#2196F3", null, false, "Giàu đạm" },
                    { new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"), "#8BC34A", null, false, "Thuần chay" },
                    { new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"), "#FF9800", null, false, "Không gluten" },
                    { new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"), "#CDDC39", null, false, "Chay" },
                    { new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"), "#FFC107", null, false, "Món nhanh" },
                    { new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"), "#00BCD4", null, false, "Ít béo" },
                    { new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"), "#795548", null, false, "Keto" },
                    { new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"), "#9C27B0", null, false, "Ít tinh bột" },
                    { new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"), "#4CAF50", null, false, "Lành mạnh" },
                    { new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"), "#FF5722", null, false, "Phù hợp cho người tiểu đường" }
                });

            migrationBuilder.InsertData(
                table: "NutrientUnits",
                columns: new[] { "Id", "Description", "Name", "Symbol" },
                values: new object[,]
                {
                    { new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đơn vị dùng cho các đại dưỡng chất như protein, chất béo, tinh bột.", "Gram (Gam)", "g" },
                    { new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Đơn vị dùng cho khoáng chất và vitamin thông thường.", "Milligram (Miligam)", "mg" },
                    { new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Đơn vị năng lượng thường gọi là calo.", "Kilocalorie (Kcal)", "kcal" },
                    { new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Dùng cho hoạt tính của vitamin A, D, E, K.", "International Unit (Đơn vị quốc tế)", "IU" },
                    { new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Đơn vị dùng cho vitamin vi lượng như B12, K, Folate...", "Microgram (Micromet)", "µg" }
                });

            migrationBuilder.InsertData(
                table: "PermissionDomains",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2a4f7344-77d7-f5fb-727a-ce3c3f9a79e4"), "Nhóm nguyên liệu" },
                    { new Guid("3137cc33-0a46-b311-6ff2-bf91a8142f47"), "Quản lí người dùng" },
                    { new Guid("4c689fb8-b1ca-c181-9aea-58a449314d35"), "Nguyên liệu" },
                    { new Guid("7218eae3-8905-04d8-d6a4-670fe18b3f5d"), "Nhãn món ăn" },
                    { new Guid("9200695c-188f-9006-34d4-68681681c7be"), "Mục tiêu sức khỏe" },
                    { new Guid("b8cfdcb4-4201-9269-b3af-6203c23e4ace"), "Công thức" },
                    { new Guid("c237902e-2228-e29a-be9a-08ad51dc9310"), "Báo cáo" },
                    { new Guid("e32d29b4-2541-8830-3dde-355cf77e4a6d"), "Đánh giá" },
                    { new Guid("f6d91264-54e5-24ab-9773-3cbd65b32a91"), "Bình luận" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsActive", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), null, true, "Admin", "ADMIN" },
                    { new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"), null, true, "Customer", "CUSTOMER" },
                    { new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"), null, true, "Moderator", "MODERATOR" }
                });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[,]
                {
                    { new Guid("3a9a556f-7285-4572-28aa-67447560ece8"), "Giúp tạo máu và duy trì hệ thần kinh.", "Vitamin B12", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Vitamin B12" },
                    { new Guid("40d7e2f9-a5da-064c-fe4d-28febe860039"), "Chống oxy hóa, bảo vệ tế bào.", "Vitamin E", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin E" },
                    { new Guid("422833be-5c43-e625-7a6a-6a74c32794a6"), "Chuyển hóa năng lượng, hỗ trợ thần kinh.", "Vitamin B1 (Thiamin)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B1" },
                    { new Guid("4345a4c7-9cd2-6519-5892-9dcc40bb9ecc"), "Tăng cường miễn dịch, chống oxy hóa.", "Vitamin C", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin C" },
                    { new Guid("4e465394-2d14-2a0a-7a00-5db0bc9e4597"), "Giúp tổng hợp hồng cầu, duy trì trao đổi chất.", "Vitamin B6", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B6" }
                });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsMacroNutrient", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("4e7a667e-4012-d80e-9276-1cd44d4e7fbd"), "Giúp xây dựng cơ bắp và tế bào.", true, "Protein", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất đạm" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[,]
                {
                    { new Guid("55fb9dc4-6bf0-06b4-e2c5-ca786f557d38"), "Chống oxy hóa, tăng cường miễn dịch.", "Selenium", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Selen" },
                    { new Guid("5e08cf55-2b60-6f26-ef65-305553ffb09b"), "Giúp sáng mắt và tăng sức đề kháng.", "Vitamin A", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin A" },
                    { new Guid("67dc4e3a-958b-9f2b-ba70-d7e6690b8f2d"), "Duy trì cân bằng nước và nhịp tim.", "Potassium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Kali" }
                });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsMacroNutrient", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("73cd094d-61aa-61ce-d021-9ffa9b9ebbad"), "Tổng lượng chất béo trong thực phẩm.", true, "Fat", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tổng chất béo" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[,]
                {
                    { new Guid("7dd02ec7-0bde-e9d2-4f7b-99e3184f139e"), "Hỗ trợ miễn dịch, da, tóc và móng.", "Zinc", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Kẽm" },
                    { new Guid("7df9ddde-bcce-958a-2a38-85778c6cfb7b"), "Giúp đông máu và duy trì xương khỏe mạnh.", "Vitamin K", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Vitamin K" },
                    { new Guid("88264feb-65c1-6808-c47c-44e3ebe1f725"), "Giúp hình thành xương và răng.", "Phosphorus", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Phốt pho" },
                    { new Guid("968aface-8106-d49e-09dc-761ca6080887"), "Thành phần của huyết sắc tố (hemoglobin).", "Iron", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Sắt" },
                    { new Guid("ac960903-ad9f-dfae-e0b3-35628565a3cb"), "Tốt cho da, mắt và hệ thần kinh.", "Vitamin B2 (Riboflavin)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B2" },
                    { new Guid("ba6906e3-9e16-e3df-06c5-f3b628919649"), "Giúp điều hòa nước và áp suất máu.", "Sodium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Natri" },
                    { new Guid("bc5e858f-8aaa-e3f1-c7ae-bf691e5fa88e"), "Giúp chuyển hóa năng lượng, bảo vệ tim mạch.", "Vitamin B3 (Niacin)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B3" },
                    { new Guid("c52f37b6-b8ba-c587-72d7-d3f5dc8044d6"), "Cần cho xương và não.", "Manganese", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Mangan" },
                    { new Guid("c8cd2a0b-6458-d98b-0ebf-0243cf575556"), "Giúp hấp thu canxi, tốt cho xương.", "Vitamin D", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin D" },
                    { new Guid("dbe42ec4-51b1-f98d-66fe-6fed6bdcad0a"), "Tổng lượng đường tự nhiên và thêm vào.", "Sugars", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường" },
                    { new Guid("e19cd21d-2c38-e38f-7c55-f643fd65daf9"), "Hỗ trợ tiêu hóa và giảm cholesterol.", "Dietary Fiber", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất xơ" },
                    { new Guid("ed0c64a9-afc7-216a-a83e-8aebc743e462"), "Cần thiết cho xương và răng chắc khỏe.", "Calcium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Canxi" },
                    { new Guid("f2e0b30a-40ad-f850-5251-36fd00dc462e"), "Cholesterol trong thực phẩm.", "Cholesterol", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Cholesterol" },
                    { new Guid("f3c5dea5-8442-1e88-a8bb-d71679c86ede"), "Quan trọng cho cơ và thần kinh.", "Magnesium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Magie" },
                    { new Guid("fa0f09a4-fbbd-3da5-76b0-748a0d87ce21"), "Tham gia hình thành tế bào máu và enzyme.", "Copper", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Đồng" }
                });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsMacroNutrient", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("feca7dbc-1254-74f3-c7e0-ff7b786515d0"), "Nguồn năng lượng chính của cơ thể.", true, "Carbohydrate", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tinh bột" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("ff39565c-1b2f-7db1-4f47-7b9ca86221f6"), "Quan trọng cho phụ nữ mang thai và tế bào mới.", "Folate (Folic Acid)", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Axit folic" });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("08bf779c-d72c-7fb2-da89-8f762f9655d7"), "Xóa", new Guid("3137cc33-0a46-b311-6ff2-bf91a8142f47") },
                    { new Guid("1f80d74b-47ce-57da-cd73-409be61bcfca"), "Xem với quyền", new Guid("4c689fb8-b1ca-c181-9aea-58a449314d35") },
                    { new Guid("36d3785c-ba45-f596-42e1-ff3503eefe6c"), "Tạo", new Guid("7218eae3-8905-04d8-d6a4-670fe18b3f5d") },
                    { new Guid("41397e25-2514-1a63-4462-62337b78d6d9"), "Xem với quyền", new Guid("b8cfdcb4-4201-9269-b3af-6203c23e4ace") },
                    { new Guid("415c998c-7281-d587-8d83-04182ec1d8c9"), "Xem", new Guid("3137cc33-0a46-b311-6ff2-bf91a8142f47") },
                    { new Guid("56874e49-85f7-3648-c6ba-e00f17bed289"), "Xem", new Guid("c237902e-2228-e29a-be9a-08ad51dc9310") },
                    { new Guid("59fc98f7-cec7-98ea-5cb8-42569e858a6b"), "Tạo", new Guid("2a4f7344-77d7-f5fb-727a-ce3c3f9a79e4") },
                    { new Guid("5d12a711-9033-ccab-4b12-c2f929daef24"), "Xóa", new Guid("b8cfdcb4-4201-9269-b3af-6203c23e4ace") },
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), "Cập nhật", new Guid("e32d29b4-2541-8830-3dde-355cf77e4a6d") },
                    { new Guid("726e7a7b-6cfd-7eb5-67b2-d369a3057350"), "Xóa", new Guid("2a4f7344-77d7-f5fb-727a-ce3c3f9a79e4") },
                    { new Guid("7b1f6e8d-4ab3-dfab-b15d-aa2914f86f26"), "Cập nhật", new Guid("f6d91264-54e5-24ab-9773-3cbd65b32a91") },
                    { new Guid("7f606467-5dd3-40ea-251d-46b068bba8ca"), "Xóa", new Guid("f6d91264-54e5-24ab-9773-3cbd65b32a91") },
                    { new Guid("87dd48ca-fdba-5090-060c-ab51927e1574"), "Xóa", new Guid("e32d29b4-2541-8830-3dde-355cf77e4a6d") },
                    { new Guid("8d8d29e6-f5bb-67c6-da83-9d8c90d983dd"), "Cập nhật", new Guid("3137cc33-0a46-b311-6ff2-bf91a8142f47") },
                    { new Guid("93eb2cca-2057-4dca-e548-9a6feada7f2f"), "Xóa", new Guid("7218eae3-8905-04d8-d6a4-670fe18b3f5d") },
                    { new Guid("9484aac2-6d5d-e8f6-c640-e3835910320c"), "Tạo", new Guid("f6d91264-54e5-24ab-9773-3cbd65b32a91") },
                    { new Guid("9b30e915-25cd-e11b-7f82-1e4872fcf382"), "Tạo", new Guid("4c689fb8-b1ca-c181-9aea-58a449314d35") },
                    { new Guid("aa94461c-6ae5-7f85-317c-b637c3191578"), "Tạo", new Guid("9200695c-188f-9006-34d4-68681681c7be") },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), "Tạo", new Guid("e32d29b4-2541-8830-3dde-355cf77e4a6d") },
                    { new Guid("b1f091d5-0d3d-764b-d063-02f60f1521eb"), "Từ chối", new Guid("c237902e-2228-e29a-be9a-08ad51dc9310") },
                    { new Guid("b7c20f26-bf21-88e4-c21e-a5276782f4fb"), "Xác nhận", new Guid("b8cfdcb4-4201-9269-b3af-6203c23e4ace") },
                    { new Guid("c0ae0175-fa89-d728-e5d3-91cacc621b35"), "Xóa", new Guid("4c689fb8-b1ca-c181-9aea-58a449314d35") },
                    { new Guid("cafa9b9d-117e-a8bb-7257-0c690172ae77"), "Cập nhật", new Guid("7218eae3-8905-04d8-d6a4-670fe18b3f5d") },
                    { new Guid("d7fcb1a8-2e8b-30af-2d68-61632720cd1d"), "Cập nhật", new Guid("9200695c-188f-9006-34d4-68681681c7be") },
                    { new Guid("f03ce2a1-72c8-02c7-fe69-8fe4018049a2"), "Xác nhận", new Guid("c237902e-2228-e29a-be9a-08ad51dc9310") },
                    { new Guid("f2e7330b-d852-a9e2-cf52-445cdb8a4db6"), "Tạo", new Guid("3137cc33-0a46-b311-6ff2-bf91a8142f47") },
                    { new Guid("f67d6419-aaf4-fe2e-77e1-c347ead43ed4"), "Xóa", new Guid("9200695c-188f-9006-34d4-68681681c7be") },
                    { new Guid("fc9c2ea1-13e9-0f88-8ad1-0f479d4f4407"), "Khóa", new Guid("b8cfdcb4-4201-9269-b3af-6203c23e4ace") },
                    { new Guid("fddc030c-67f3-57d0-c4b4-73223bf0395b"), "Cập nhật", new Guid("4c689fb8-b1ca-c181-9aea-58a449314d35") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("08bf779c-d72c-7fb2-da89-8f762f9655d7"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("1f80d74b-47ce-57da-cd73-409be61bcfca"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("36d3785c-ba45-f596-42e1-ff3503eefe6c"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("41397e25-2514-1a63-4462-62337b78d6d9"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("415c998c-7281-d587-8d83-04182ec1d8c9"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("56874e49-85f7-3648-c6ba-e00f17bed289"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("59fc98f7-cec7-98ea-5cb8-42569e858a6b"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("5d12a711-9033-ccab-4b12-c2f929daef24"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("726e7a7b-6cfd-7eb5-67b2-d369a3057350"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("7b1f6e8d-4ab3-dfab-b15d-aa2914f86f26"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("7f606467-5dd3-40ea-251d-46b068bba8ca"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("87dd48ca-fdba-5090-060c-ab51927e1574"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("8d8d29e6-f5bb-67c6-da83-9d8c90d983dd"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("93eb2cca-2057-4dca-e548-9a6feada7f2f"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("9484aac2-6d5d-e8f6-c640-e3835910320c"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("9b30e915-25cd-e11b-7f82-1e4872fcf382"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("aa94461c-6ae5-7f85-317c-b637c3191578"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("b1f091d5-0d3d-764b-d063-02f60f1521eb"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("b7c20f26-bf21-88e4-c21e-a5276782f4fb"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("c0ae0175-fa89-d728-e5d3-91cacc621b35"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("cafa9b9d-117e-a8bb-7257-0c690172ae77"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("d7fcb1a8-2e8b-30af-2d68-61632720cd1d"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("f03ce2a1-72c8-02c7-fe69-8fe4018049a2"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("f2e7330b-d852-a9e2-cf52-445cdb8a4db6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("f67d6419-aaf4-fe2e-77e1-c347ead43ed4"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("fc9c2ea1-13e9-0f88-8ad1-0f479d4f4407"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("fddc030c-67f3-57d0-c4b4-73223bf0395b"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("08bf779c-d72c-7fb2-da89-8f762f9655d7"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("1f80d74b-47ce-57da-cd73-409be61bcfca"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("36d3785c-ba45-f596-42e1-ff3503eefe6c"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("41397e25-2514-1a63-4462-62337b78d6d9"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("415c998c-7281-d587-8d83-04182ec1d8c9"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("56874e49-85f7-3648-c6ba-e00f17bed289"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("59fc98f7-cec7-98ea-5cb8-42569e858a6b"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("5d12a711-9033-ccab-4b12-c2f929daef24"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("726e7a7b-6cfd-7eb5-67b2-d369a3057350"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("7b1f6e8d-4ab3-dfab-b15d-aa2914f86f26"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("7f606467-5dd3-40ea-251d-46b068bba8ca"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("87dd48ca-fdba-5090-060c-ab51927e1574"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("8d8d29e6-f5bb-67c6-da83-9d8c90d983dd"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("93eb2cca-2057-4dca-e548-9a6feada7f2f"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("9484aac2-6d5d-e8f6-c640-e3835910320c"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("9b30e915-25cd-e11b-7f82-1e4872fcf382"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("aa94461c-6ae5-7f85-317c-b637c3191578"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("b1f091d5-0d3d-764b-d063-02f60f1521eb"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("b7c20f26-bf21-88e4-c21e-a5276782f4fb"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("c0ae0175-fa89-d728-e5d3-91cacc621b35"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("cafa9b9d-117e-a8bb-7257-0c690172ae77"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("d7fcb1a8-2e8b-30af-2d68-61632720cd1d"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("f03ce2a1-72c8-02c7-fe69-8fe4018049a2"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("f2e7330b-d852-a9e2-cf52-445cdb8a4db6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("f67d6419-aaf4-fe2e-77e1-c347ead43ed4"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("fc9c2ea1-13e9-0f88-8ad1-0f479d4f4407"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("fddc030c-67f3-57d0-c4b4-73223bf0395b"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("08bf779c-d72c-7fb2-da89-8f762f9655d7"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("1f80d74b-47ce-57da-cd73-409be61bcfca"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("36d3785c-ba45-f596-42e1-ff3503eefe6c"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("41397e25-2514-1a63-4462-62337b78d6d9"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("415c998c-7281-d587-8d83-04182ec1d8c9"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("56874e49-85f7-3648-c6ba-e00f17bed289"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("59fc98f7-cec7-98ea-5cb8-42569e858a6b"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("5d12a711-9033-ccab-4b12-c2f929daef24"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("726e7a7b-6cfd-7eb5-67b2-d369a3057350"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("7b1f6e8d-4ab3-dfab-b15d-aa2914f86f26"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("7f606467-5dd3-40ea-251d-46b068bba8ca"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("87dd48ca-fdba-5090-060c-ab51927e1574"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("8d8d29e6-f5bb-67c6-da83-9d8c90d983dd"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("93eb2cca-2057-4dca-e548-9a6feada7f2f"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("9484aac2-6d5d-e8f6-c640-e3835910320c"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("9b30e915-25cd-e11b-7f82-1e4872fcf382"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("aa94461c-6ae5-7f85-317c-b637c3191578"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("b1f091d5-0d3d-764b-d063-02f60f1521eb"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("b7c20f26-bf21-88e4-c21e-a5276782f4fb"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("c0ae0175-fa89-d728-e5d3-91cacc621b35"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("cafa9b9d-117e-a8bb-7257-0c690172ae77"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("d7fcb1a8-2e8b-30af-2d68-61632720cd1d"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("f03ce2a1-72c8-02c7-fe69-8fe4018049a2"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("f2e7330b-d852-a9e2-cf52-445cdb8a4db6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("f67d6419-aaf4-fe2e-77e1-c347ead43ed4"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("fc9c2ea1-13e9-0f88-8ad1-0f479d4f4407"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("fddc030c-67f3-57d0-c4b4-73223bf0395b"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_MentionedUserId",
                table: "CommentMentions",
                column: "MentionedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RecipeId",
                table: "Comments",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CookingStepImages_CookingStepId",
                table: "CookingStepImages",
                column: "CookingStepId");

            migrationBuilder.CreateIndex(
                name: "IX_CookingStepImages_ImageId",
                table: "CookingStepImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_RecipeId",
                table: "CookingSteps",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomHealthGoals_UserId",
                table: "CustomHealthGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftCookingStepImages_DraftCookingStepId",
                table: "DraftCookingStepImages",
                column: "DraftCookingStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftCookingStepImages_ImageId",
                table: "DraftCookingStepImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftCookingSteps_DraftRecipeId",
                table: "DraftCookingSteps",
                column: "DraftRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipeIngredients_IngredientId",
                table: "DraftRecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipes_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipes_ImageId",
                table: "DraftRecipes",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipeUserTags_TaggedUserId",
                table: "DraftRecipeUserTags",
                column: "TaggedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOtps_SentToId",
                table: "EmailOtps",
                column: "SentToId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_CustomHealthGoalId",
                table: "HealthGoalTargets",
                column: "CustomHealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_HealthGoalId",
                table: "HealthGoalTargets",
                column: "HealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_UploadedById",
                table: "Images",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_Name",
                table: "IngredientCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLink_IngredientsId",
                table: "IngredientCategoryLink",
                column: "IngredientsId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientNutrients_IngredientId",
                table: "IngredientNutrients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientNutrients_NutrientId",
                table: "IngredientNutrients",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ImageId",
                table: "Ingredients",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Name",
                table: "Ingredients",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DraftRecipeId",
                table: "Labels",
                column: "DraftRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Nutrients_UnitId",
                table: "Nutrients",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionActions_PermissionDomainId",
                table: "PermissionActions",
                column: "PermissionDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_AppUserId",
                table: "Ratings",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RecipeId",
                table: "Ratings",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId_RecipeId",
                table: "Ratings",
                columns: new[] { "UserId", "RecipeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLabels_RecipesId",
                table: "RecipeLabels",
                column: "RecipesId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeNutritionAggregates_NutrientId",
                table: "RecipeNutritionAggregates",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ImageId",
                table: "Recipes",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ParentId",
                table: "Recipes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUserTags_TaggedUserId",
                table: "RecipeUserTags",
                column: "TaggedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReporterId",
                table: "Reports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionActionId",
                table: "RolePermissions",
                column: "PermissionActionId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_FolloweeId",
                table: "UserFollows",
                column: "FolloweeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_AppUserId",
                table: "UserHealthGoals",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_CustomHealthGoalId",
                table: "UserHealthGoals",
                column: "CustomHealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_HealthGoalId",
                table: "UserHealthGoals",
                column: "HealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_UserId",
                table: "UserHealthGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthMetrics_UserId",
                table: "UserHealthMetrics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_IngredientCategoryId",
                table: "UserIngredientRestrictions",
                column: "IngredientCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_IngredientId",
                table: "UserIngredientRestrictions",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_UserId",
                table: "UserIngredientRestrictions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipeViews_RecipeId",
                table: "UserRecipeViews",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarId",
                table: "Users",
                column: "AvatarId",
                unique: true,
                filter: "[AvatarId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveRecipes_RecipeId",
                table: "UserSaveRecipes",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentMentions_Comments_CommentId",
                table: "CommentMentions",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentMentions_Users_MentionedUserId",
                table: "CommentMentions",
                column: "MentionedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Recipes_RecipeId",
                table: "Comments",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CookingStepImages_CookingSteps_CookingStepId",
                table: "CookingStepImages",
                column: "CookingStepId",
                principalTable: "CookingSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CookingStepImages_Images_ImageId",
                table: "CookingStepImages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CookingSteps_Recipes_RecipeId",
                table: "CookingSteps",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomHealthGoals_Users_UserId",
                table: "CustomHealthGoals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DraftCookingStepImages_DraftCookingSteps_DraftCookingStepId",
                table: "DraftCookingStepImages",
                column: "DraftCookingStepId",
                principalTable: "DraftCookingSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DraftCookingStepImages_Images_ImageId",
                table: "DraftCookingStepImages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DraftCookingSteps_DraftRecipes_DraftRecipeId",
                table: "DraftCookingSteps",
                column: "DraftRecipeId",
                principalTable: "DraftRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipeIngredients_DraftRecipes_DraftRecipeId",
                table: "DraftRecipeIngredients",
                column: "DraftRecipeId",
                principalTable: "DraftRecipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipeIngredients_Ingredients_IngredientId",
                table: "DraftRecipeIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipes_Images_ImageId",
                table: "DraftRecipes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipes_Users_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipeUserTags_Users_TaggedUserId",
                table: "DraftRecipeUserTags",
                column: "TaggedUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOtps_Users_SentToId",
                table: "EmailOtps",
                column: "SentToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_UploadedById",
                table: "Images",
                column: "UploadedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_UploadedById",
                table: "Images");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CommentMentions");

            migrationBuilder.DropTable(
                name: "CookingStepImages");

            migrationBuilder.DropTable(
                name: "DraftCookingStepImages");

            migrationBuilder.DropTable(
                name: "DraftRecipeIngredients");

            migrationBuilder.DropTable(
                name: "DraftRecipeUserTags");

            migrationBuilder.DropTable(
                name: "EmailOtps");

            migrationBuilder.DropTable(
                name: "HealthGoalTargets");

            migrationBuilder.DropTable(
                name: "IngredientCategoryLink");

            migrationBuilder.DropTable(
                name: "IngredientNutrients");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "RecipeLabels");

            migrationBuilder.DropTable(
                name: "RecipeNutritionAggregates");

            migrationBuilder.DropTable(
                name: "RecipeUserTags");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserFollows");

            migrationBuilder.DropTable(
                name: "UserHealthGoals");

            migrationBuilder.DropTable(
                name: "UserHealthMetrics");

            migrationBuilder.DropTable(
                name: "UserIngredientRestrictions");

            migrationBuilder.DropTable(
                name: "UserRecipeViews");

            migrationBuilder.DropTable(
                name: "UserSaveRecipes");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CookingSteps");

            migrationBuilder.DropTable(
                name: "DraftCookingSteps");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Nutrients");

            migrationBuilder.DropTable(
                name: "PermissionActions");

            migrationBuilder.DropTable(
                name: "CustomHealthGoals");

            migrationBuilder.DropTable(
                name: "HealthGoals");

            migrationBuilder.DropTable(
                name: "IngredientCategories");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "DraftRecipes");

            migrationBuilder.DropTable(
                name: "NutrientUnits");

            migrationBuilder.DropTable(
                name: "PermissionDomains");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
