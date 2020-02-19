using System.Collections.Generic;
using System.Threading.Tasks;
using MyBills.Domain.Entities;

namespace MyBills.Domain.Interfaces
{
    public interface IRecurrenceTypeRepository
    {
        Task<List<RecurrenceType>> GetRecurrenceTypesAsync();
    }
}
