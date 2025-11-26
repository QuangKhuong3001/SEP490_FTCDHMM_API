namespace SEP490_FTCDHMM_API.Domain.Interfaces
{
    public interface INutrientIdProvider
    {
        Guid ProteinId { get; }
        Guid FatId { get; }
        Guid CarbohydrateId { get; }
    }
}
