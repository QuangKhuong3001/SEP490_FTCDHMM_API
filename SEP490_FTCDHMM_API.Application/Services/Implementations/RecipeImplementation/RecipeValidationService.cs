using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeValidationService : IRecipeValidationService
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserRepository _userRepository;

        public RecipeValidationService(
            ILabelRepository labelRepository,
            IIngredientRepository ingredientRepository,
            IUserRepository userRepository)
        {
            _labelRepository = labelRepository;
            _ingredientRepository = ingredientRepository;
            _userRepository = userRepository;
        }

        public async Task ValidateLabelsAsync(IEnumerable<Guid> labelIds)
        {
            var ids = labelIds?.ToList() ?? new List<Guid>();
            if (!ids.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Danh sách nhãn dán trống");

            if (ids.HasDuplicate())
                throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nhãn dán bị trùng lặp");

            var exist = await _labelRepository.IdsExistAsync(ids);
            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Nhãn dán không tồn tại");
        }

        public async Task ValidateIngredientsAsync(IEnumerable<Guid> ingredientIds)
        {
            var ids = ingredientIds?.ToList() ?? new List<Guid>();
            if (!ids.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Danh sách nguyên liệu trống");

            if (ids.HasDuplicate())
                throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nguyên liệu bị trùng lặp");

            var exist = await _ingredientRepository.IdsExistAsync(ids);
            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Nguyên liệu không tồn tại");
        }

        public Task ValidateCookingStepsAsync(IEnumerable<CookingStepRequest> steps)
        {
            var stepList = steps?.ToList() ?? new List<CookingStepRequest>();
            if (!stepList.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Cần ít nhất 1 bước nấu ăn");

            var stepOrders = stepList.Select(s => s.StepOrder).ToList();
            if (stepOrders.HasDuplicate())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự bước nấu bị trùng");

            foreach (var step in stepList)
            {
                if (string.IsNullOrWhiteSpace(step.Instruction))
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Mô tả bước nấu không được rỗng");

                var images = step.Images?.ToList() ?? new List<CookingStepImageRequest>();
                if (images.Any())
                {
                    var imageOrders = images.Select(i => i.ImageOrder).ToList();
                    if (imageOrders.HasDuplicate())
                        throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự ảnh trong bước nấu bị trùng");
                }
            }

            return Task.CompletedTask;
        }

        public async Task ValidateTaggedUsersAsync(Guid authorId, IEnumerable<Guid> taggedUserIds)
        {
            var ids = taggedUserIds?.Distinct().ToList() ?? new List<Guid>();
            if (!ids.Any())
                return;

            foreach (var id in ids)
            {
                if (id == authorId)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể tự gắn thẻ chính mình.");

                var exists = await _userRepository.ExistsAsync(u => u.Id == id);
                if (!exists)
                    throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION, $"Người dùng không tồn tại.");
            }
        }

        public async Task ValidateUserExistsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);
        }

        public Task ValidateRecipeOwnerAsync(Guid userId, Recipe recipe)
        {
            if (recipe.AuthorId != userId)
                throw new AppException(AppResponseCode.ACCESS_DENIED, "Bạn không có quyền chỉnh sửa công thức");

            if (recipe.Status == RecipeStatus.Deleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            return Task.CompletedTask;
        }
    }
}
