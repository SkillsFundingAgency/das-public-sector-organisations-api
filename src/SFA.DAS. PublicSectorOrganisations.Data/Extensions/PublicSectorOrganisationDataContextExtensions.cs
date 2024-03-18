using Microsoft.EntityFrameworkCore;
using SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data.Extensions;

public static class PublicSectorOrganisationDataContextExtensions
{
    public static async Task ExecuteInATransaction(this PublicSectorOrganisationDataContext dbContext, Func<Task> action)
    {
        if (dbContext == null)
        {
            throw new ArgumentNullException(nameof(dbContext));
        }
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!IsInMemoryDatabase(dbContext))
        {
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await action.Invoke();
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new WithinTransactionException($"Transaction is being rolled back", ex);
                }
            }
        }
        else
        {
            await action.Invoke();
        }
    }

    //public static async Task ResetActiveToFalseForSource(this PublicSectorOrganisationDataContext dbContext, DataSource source)
    //{
    //    if (IsInMemoryDatabase(dbContext))
    //    {
    //        //Note: This is done here as the InMemory and SQLLite Dbs do not process the new functionality 'ExecuteUpdateAsync' correctly
    //        foreach (var entity in dbContext.PublicSectorOrganisationEntities.Where(x => x.Source == source))
    //        {
    //            entity.Active = false;
    //        }
    //    }
    //    else
    //    {
    //        await dbContext.PublicSectorOrganisationEntities.Where(x => x.Source == source)
    //            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Active, false));
    //    }
    //}
    private static bool IsInMemoryDatabase(DbContext dbContext)
    {
        return dbContext.Database.ProviderName.Equals("Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase);
    }
}

