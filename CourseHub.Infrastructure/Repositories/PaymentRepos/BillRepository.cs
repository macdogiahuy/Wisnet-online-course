using CourseHub.Core.Entities.PaymentDomain;
using CourseHub.Core.Interfaces.Repositories.PaymentRepos;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories.PaymentRepos;

public class BillRepository : BaseRepository<Bill>, IBillRepository
{
    public BillRepository(DbContext context) : base(context)
    {
    }
}
