using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizer
{
    public class AggregateItemNameLookupService(List<IItemNameLookupService> gameSpecificNameLookupServices) : IItemNameLookupService
    {
        public bool TryGetItemName(SoulsItem item, [NotNullWhen(true)] out string? itemName)
        {
            foreach (var nameLookupService in gameSpecificNameLookupServices)
            {
                if (nameLookupService.TryGetItemName(item, out itemName))
                {
                    return true;
                }
            }
            itemName = null;
            return false;
        }
    }

    public class AggregateItemNameLookupServiceBuilder
    {
        private readonly List<IItemNameLookupService> services = [];

        public void WithLookupService(IItemNameLookupService service)
        {
            services.Add(service);
        }

        public AggregateItemNameLookupService Build()
        {
            return new AggregateItemNameLookupService(services);
        }
    }
}