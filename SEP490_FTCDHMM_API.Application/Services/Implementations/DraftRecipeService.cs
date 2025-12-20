using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class DraftRecipeService : IDraftRecipeService
    {
        private readonly IMapper _mapper;
        private readonly IDraftRecipeRepository _draftRecipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly IS3ImageService _imageService;
        private readonly IImageRepository _imageRepository;
        private readonly IIngredientRepository _ingredientRepository;
        public DraftRecipeService(IMapper mapper,
            IDraftRecipeRepository draftRecipeRepository,
            IUserRepository userRepository,
            ILabelRepository labelRepository,
            IS3ImageService imageService,
            IImageRepository imageRepository,
            IIngredientRepository ingredientRepository)
        {
            _mapper = mapper;
            _draftRecipeRepository = draftRecipeRepository;
            _userRepository = userRepository;
            _labelRepository = labelRepository;
            _imageRepository = imageRepository;
            _imageService = imageService;
            _ingredientRepository = ingredientRepository;
        }

        public async Task CreateDraftAsync(Guid userId, DraftRecipeRequest request)
        {

            var stepOrders = request.CookingSteps.Select(x => x.StepOrder).ToList();
            if (stepOrders.Count != stepOrders.Distinct().Count())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự bước nấu ăn bị trùng.");

            foreach (var step in request.CookingSteps)
            {
                var imgOrders = step.Images.Select(i => i.ImageOrder).ToList();
                if (imgOrders.Count != imgOrders.Distinct().Count())
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự ảnh trùng trong cùng bước.");
            }

            if (request.Ingredients.Count > 0)
            {
                var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
                var ingredientExists = await _ingredientRepository.IdsExistAsync(ingredientIds);

                if (!(ingredientExists))
                    throw new AppException(AppResponseCode.NOT_FOUND, "Nguyên liệu không tồn tại");

                if (request.Ingredients.Select(i => i.IngredientId).HasDuplicate())
                    throw new AppException(AppResponseCode.DUPLICATE, "Nguyên liệu bị trùng.");
            }

            if (request.LabelIds.Count > 0)
            {
                var labelExists = await _labelRepository.IdsExistAsync(request.LabelIds);
                if (!(labelExists))
                    throw new AppException(AppResponseCode.NOT_FOUND, "Nhãn dán không tồn tại");

                if (request.LabelIds.HasDuplicate())
                    throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nhãn dán bị trùng.");
            }

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            var draft = new DraftRecipe
            {
                AuthorId = userId,
                Name = request.Name,
                Description = request.Description,
                CookTime = request.CookTime,
                Ration = request.Ration ?? 0,
                Difficulty = DifficultyValue.From(request.Difficulty),
                UpdatedAtUtc = DateTime.UtcNow,
                Labels = labels.ToList()
            };

            if (request.Image != null)
            {
                var uploaded = await _imageService.UploadImageAsync(request.Image, StorageFolder.DRAFTS);
                draft.Image = uploaded;
            }

            draft.DraftRecipeIngredients = request.Ingredients.Select(i => new DraftRecipeIngredient
            {
                IngredientId = i.IngredientId,
                QuantityGram = i.QuantityGram
            }).ToList();

            foreach (var uid in request.TaggedUserIds.Distinct())
            {
                if (uid == userId)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể tự tag chính mình.");

                var exists = await _userRepository.ExistsAsync(u => u.Id == uid);
                if (!exists)
                    throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

                draft.DraftRecipeUserTags.Add(new DraftRecipeUserTag
                {
                    TaggedUserId = uid
                });
            }

            foreach (var s in request.CookingSteps.OrderBy(x => x.StepOrder))
            {
                var step = new DraftCookingStep
                {
                    StepOrder = s.StepOrder,
                    Instruction = s.Instruction?.Trim(),
                };

                if (s.Images != null)
                {
                    step.DraftCookingStepImages = new List<DraftCookingStepImage>();

                    foreach (var img in s.Images)
                    {
                        if (img.Image != null)
                        {
                            var uploaded = await _imageService.UploadImageAsync(img.Image, StorageFolder.DRAFT_COOKING_STEPS);
                            step.DraftCookingStepImages.Add(new DraftCookingStepImage
                            {
                                ImageOrder = img.ImageOrder,
                                Image = uploaded
                            });
                        }
                    }
                }

                draft.DraftCookingSteps.Add(step);
            }

            await _draftRecipeRepository.AddAsync(draft);
        }

        public async Task UpdateDraftAsync(Guid userId, Guid draftId, DraftRecipeRequest request)
        {

            var stepOrders = request.CookingSteps.Select(x => x.StepOrder).ToList();
            if (stepOrders.Count != stepOrders.Distinct().Count())
                throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự bước nấu ăn bị trùng.");

            foreach (var step in request.CookingSteps)
            {
                var imgOrders = step.Images.Select(i => i.ImageOrder).ToList();
                if (imgOrders.Count != imgOrders.Distinct().Count())
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự ảnh trùng trong cùng bước.");
            }

            if (request.Ingredients.Count > 0)
            {
                var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
                var ingredientExists = await _ingredientRepository.IdsExistAsync(ingredientIds);

                if (!(ingredientExists))
                    throw new AppException(AppResponseCode.NOT_FOUND, "Nguyên liệu không tồn tại");

                if (request.Ingredients.Select(i => i.IngredientId).HasDuplicate())
                    throw new AppException(AppResponseCode.DUPLICATE, "Nguyên liệu bị trùng.");
            }

            if (request.LabelIds.Count > 0)
            {
                var labelExists = await _labelRepository.IdsExistAsync(request.LabelIds);
                if (!(labelExists))
                    throw new AppException(AppResponseCode.NOT_FOUND, "Nhãn dán không tồn tại");

                if (request.LabelIds.HasDuplicate())
                    throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nhãn dán bị trùng.");
            }

            var draft = await _draftRecipeRepository.GetByIdAsync(draftId);
            if (draft == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bản nháp không tồn tại");

            if (draft.AuthorId != userId)
                throw new AppException(AppResponseCode.ACCESS_DENIED, "Không có quyền chỉnh sửa bản nháp này.");

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            await _imageRepository.MarkDeletedAsync(draft.ImageId);
            await _imageRepository.MarkDeletedStepsImageFromDraftAsync(draft);
            await _draftRecipeRepository.DeleteAsync(draft);

            draft = new DraftRecipe
            {
                AuthorId = userId,
                Name = request.Name,
                Description = request.Description,
                CookTime = request.CookTime,
                Ration = request.Ration ?? 0,
                Difficulty = DifficultyValue.From(request.Difficulty),
                UpdatedAtUtc = DateTime.UtcNow,
                Labels = labels.ToList()
            };

            if (request.Image != null)
            {
                var uploaded = await _imageService.UploadImageAsync(request.Image, StorageFolder.DRAFTS);
                draft.Image = uploaded;
            }
            else if (request.ExistingMainImageUrl != null)
            {
                var existing = await _imageService.MirrorExternalImageAsync(StorageFolder.DRAFTS, request.ExistingMainImageUrl);
                draft.Image = existing;
            }

            draft.DraftRecipeIngredients = request.Ingredients.Select(i => new DraftRecipeIngredient
            {
                IngredientId = i.IngredientId,
                QuantityGram = i.QuantityGram
            }).ToList();

            foreach (var uid in request.TaggedUserIds.Distinct())
            {
                if (uid == userId)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể tự tag chính mình.");

                var exists = await _userRepository.ExistsAsync(u => u.Id == uid);
                if (!exists)
                    throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

                draft.DraftRecipeUserTags.Add(new DraftRecipeUserTag
                {
                    TaggedUserId = uid
                });
            }

            foreach (var s in request.CookingSteps.OrderBy(x => x.StepOrder))
            {
                var step = new DraftCookingStep
                {
                    StepOrder = s.StepOrder,
                    Instruction = s.Instruction?.Trim(),
                };

                if (s.Images != null)
                {
                    step.DraftCookingStepImages = new List<DraftCookingStepImage>();

                    foreach (var img in s.Images)
                    {
                        if (img.Image != null)
                        {
                            var uploaded = await _imageService.UploadImageAsync(img.Image, StorageFolder.DRAFT_COOKING_STEPS);
                            step.DraftCookingStepImages.Add(new DraftCookingStepImage
                            {
                                ImageOrder = img.ImageOrder,
                                Image = uploaded
                            });
                        }
                        else if (img.ExistingImageUrl != null)
                        {
                            var existing = await _imageService.MirrorExternalImageAsync(StorageFolder.DRAFT_COOKING_STEPS, img.ExistingImageUrl);
                            step.DraftCookingStepImages.Add(new DraftCookingStepImage
                            {
                                ImageOrder = img.ImageOrder,
                                Image = existing
                            });
                        }
                    }
                }

                draft.DraftCookingSteps.Add(step);
            }

            await _draftRecipeRepository.AddAsync(draft);
        }
        public async Task<IEnumerable<DraftRecipeResponse>> GetDraftsAsync(Guid userId)
        {
            var draft = await _draftRecipeRepository.GetAllAsync(
                    predicate: u => u.AuthorId == userId,
                    include: i => i.Include(d => d.Image)
                );
            var result = _mapper.Map<IEnumerable<DraftRecipeResponse>>(draft);
            return result;
        }

        public async Task<DraftDetailsResponse> GetDraftByIdAsync(Guid userId, Guid draftId)
        {
            var draft = await _draftRecipeRepository.GetByIdAsync(
                id: draftId,
                include: i => i.Include(d => d.DraftRecipeIngredients)
                                    .ThenInclude(dr => dr.Ingredient)
                                .Include(d => d.Labels)
                                .Include(d => d.DraftCookingSteps)
                                    .ThenInclude(ds => ds.DraftCookingStepImages)
                                        .ThenInclude(dsi => dsi.Image)
                                .Include(d => d.Image)
                                .Include(d => d.DraftRecipeUserTags)
                                    .ThenInclude(dut => dut.TaggedUser)
                                        .ThenInclude(u => u.Avatar)
                                );

            if (draft == null || draft.AuthorId != userId)
                throw new AppException(AppResponseCode.NOT_FOUND, "Bản nháp không tồn tại");

            var result = _mapper.Map<DraftDetailsResponse>(draft);
            return result;
        }
    }
}
