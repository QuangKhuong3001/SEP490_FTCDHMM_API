using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeImageService : IRecipeImageService
    {
        private readonly IS3ImageService _imageService;
        private readonly ICookingStepRepository _cookingStepRepository;

        public RecipeImageService(
            IS3ImageService imageService,
            ICookingStepRepository cookingStepRepository)
        {
            _imageService = imageService;
            _cookingStepRepository = cookingStepRepository;
        }

        public async Task DeleteImageAsync(Guid? imageId)
        {
            if (imageId.HasValue)
                await _imageService.DeleteImageAsync(imageId.Value);
        }

        public async Task SetRecipeImageAsync(Recipe recipe, FileUploadModel? file, string? url)
        {
            if (file != null)
            {
                var uploaded = await _imageService.UploadImageAsync(file, StorageFolder.RECIPES);
                recipe.Image = uploaded;
            }
            else if (url != null)
            {
                var mirrored = await _imageService.MirrorExternalImageAsync(StorageFolder.RECIPES, url);
                recipe.Image = mirrored;
            }
        }

        public async Task ReplaceRecipeImageAsync(Recipe recipe, FileUploadModel? file)
        {
            if (file != null)
            {
                if (recipe.ImageId.HasValue)
                {
                    await _imageService.DeleteImageAsync(recipe.ImageId.Value);
                }

                var newImage = await _imageService.UploadImageAsync(file, StorageFolder.RECIPES);

                recipe.Image = newImage;
            }
        }

        public async Task<List<CookingStep>> CreateCookingStepsAsync(IEnumerable<CookingStepRequest> steps, Recipe recipe)
        {
            var result = new List<CookingStep>();

            foreach (var step in steps.OrderBy(s => s.StepOrder))
            {
                var newStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    RecipeId = recipe.Id
                };

                var imageRequests = step.Images?.ToList() ?? new List<CookingStepImageRequest>();
                if (imageRequests.Any())
                {
                    newStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var img in imageRequests)
                    {
                        if (img.Image != null)
                        {
                            var uploaded = await _imageService.UploadImageAsync(img.Image, StorageFolder.COOKING_STEPS);

                            newStep.CookingStepImages.Add(new CookingStepImage
                            {
                                Id = Guid.NewGuid(),
                                CookingStepId = newStep.Id,
                                ImageOrder = img.ImageOrder,
                                ImageId = uploaded.Id
                            });
                        }
                        else if (img.ExistingImageUrl != null)
                        {
                            var mirrored = await _imageService.MirrorExternalImageAsync(StorageFolder.COOKING_STEPS, img.ExistingImageUrl);
                            newStep.CookingStepImages.Add(new CookingStepImage
                            {
                                Id = Guid.NewGuid(),
                                CookingStepId = newStep.Id,
                                ImageOrder = img.ImageOrder,
                                ImageId = mirrored.Id
                            });
                        }
                    }
                }

                result.Add(newStep);
            }

            return result;
        }

        public async Task<List<CookingStep>> CreateCookingStepsWithDraftImagesAsync(
            IEnumerable<CookingStepRequest> steps,
            Recipe recipe,
            Dictionary<(int stepOrder, int imageOrder), Guid> draftStepImageMap)
        {
            var result = new List<CookingStep>();

            foreach (var step in steps.OrderBy(s => s.StepOrder))
            {
                var newStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    RecipeId = recipe.Id
                };

                var imageRequests = step.Images?.ToList() ?? new List<CookingStepImageRequest>();
                if (imageRequests.Any())
                {
                    newStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var img in imageRequests)
                    {
                        Guid imageId;

                        if (img.Image != null)
                        {
                            // New image uploaded - use it
                            var uploaded = await _imageService.UploadImageAsync(img.Image, StorageFolder.COOKING_STEPS);
                            imageId = uploaded.Id;
                        }
                        else if (img.ExistingImageUrl != null)
                        {
                            // Check if we have a draft image to reuse for this step/image order
                            var key = (step.StepOrder, img.ImageOrder);
                            if (draftStepImageMap.TryGetValue(key, out var draftImageId))
                            {
                                // Reuse the draft image ID directly
                                imageId = draftImageId;
                            }
                            else
                            {
                                // Not from draft, mirror the external URL
                                var mirrored = await _imageService.MirrorExternalImageAsync(StorageFolder.COOKING_STEPS, img.ExistingImageUrl);
                                imageId = mirrored.Id;
                            }
                        }
                        else
                        {
                            continue; // No image for this slot
                        }

                        newStep.CookingStepImages.Add(new CookingStepImage
                        {
                            Id = Guid.NewGuid(),
                            CookingStepId = newStep.Id,
                            ImageOrder = img.ImageOrder,
                            ImageId = imageId
                        });
                    }
                }

                result.Add(newStep);
            }

            return result;
        }

        public async Task ReplaceCookingStepsAsync(Guid recipeId, IEnumerable<CookingStepRequest> newSteps)
        {
            var oldSteps = await _cookingStepRepository.GetAllAsync(r => r.RecipeId == recipeId,
                include: q => q
                    .Include(s => s.CookingStepImages));

            foreach (var oldStep in oldSteps)
            {
                foreach (var si in oldStep.CookingStepImages)
                {
                    await _imageService.DeleteImageAsync(si.ImageId);
                }
            }

            await _cookingStepRepository.DeleteStepsByRecipeIdAsync(recipeId);

            var newStepEntities = new List<CookingStep>();

            foreach (var step in newSteps.OrderBy(s => s.StepOrder))
            {
                var newStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    RecipeId = recipeId
                };

                var imageRequests = step.Images?.ToList() ?? new List<CookingStepImageRequest>();
                if (imageRequests.Any())
                {
                    newStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var img in imageRequests)
                    {
                        if (img.Image != null)
                        {
                            var uploaded = await _imageService.UploadImageAsync(img.Image, StorageFolder.COOKING_STEPS);

                            newStep.CookingStepImages.Add(new CookingStepImage
                            {
                                Id = Guid.NewGuid(),
                                CookingStepId = newStep.Id,
                                ImageOrder = img.ImageOrder,
                                ImageId = uploaded.Id
                            });
                        }
                        else if (img.ExistingImageUrl != null)
                        {
                            var mirrored = await _imageService.MirrorExternalImageAsync(StorageFolder.COOKING_STEPS, img.ExistingImageUrl);
                            newStep.CookingStepImages.Add(new CookingStepImage
                            {
                                Id = Guid.NewGuid(),
                                CookingStepId = newStep.Id,
                                ImageOrder = img.ImageOrder,
                                ImageId = mirrored.Id
                            });
                        }
                    }
                }

                newStepEntities.Add(newStep);
            }

            await _cookingStepRepository.AddRangeAsync(newStepEntities);
        }
    }
}
