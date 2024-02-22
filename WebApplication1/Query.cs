using HotChocolate.Resolvers;

namespace WebApplication1;

public sealed record Query
{
    public Calendar Calendar(IResolverContext resolverContext)
        => new Calendar();
}

public sealed class Calendar
{
    public async Task<Encoding[]> Encodings()
    {
        await Task.Delay(Random.Shared.Next(10, 100));
        return Get().ToArray(); 
        
        static IEnumerable<Encoding> Get()
        {
            yield return new Encoding { CompanyId = 1 };
            yield return new Encoding { CompanyId = 1 };
            yield return new Encoding { CompanyId = 1 };
        }
    }
    
    public async Task<TimeRegistration[]> TimeRegistrations()
    {
        await Task.Delay(Random.Shared.Next(250, 500));
        return Get().ToArray(); 
        
        static IEnumerable<TimeRegistration> Get()
        {
            yield return new TimeRegistration { CompanyId = 2 };
            yield return new TimeRegistration { CompanyId = 1 };
            yield return new TimeRegistration { CompanyId = null };
        }
    }
}

public abstract record CalendarItem
{
    private static int _invocationCount = 0;
    
    public int? CompanyId { get; init; }
    
    public async Task<string?> CompanyName(IResolverContext resolverContext)
    {
        if (!CompanyId.HasValue) return null;
        
        var dataLoader = resolverContext.BatchDataLoader<int, string>(async (companyIds, cancellationToken) =>
        {
            var invocationCount = Interlocked.Increment(ref _invocationCount);
            await Task.Delay(Random.Shared.Next(1, 5), cancellationToken);
            return companyIds.ToDictionary(x => x, x => x switch
            {
                1 => $"Company A - InvocationCount: {invocationCount}",
                2 => $"Company B - InvocationCount: {invocationCount}",
                _ => $"Unknown - InvocationCount: {invocationCount}",
            });
        },
        "CompanyNameByCompanyIdDataLoader");

        return await dataLoader.LoadAsync(CompanyId.Value);
    }
}

public sealed record Encoding : CalendarItem
{
}

public sealed record TimeRegistration : CalendarItem
{
}
