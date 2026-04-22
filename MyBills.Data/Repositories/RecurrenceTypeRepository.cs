using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBills.Data.Contexts;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;

namespace MyBills.Data.Repositories
{

    public class RecurrenceTypeRepository: IRecurrenceTypeRepository
    {
        private readonly MyBillsContext _context;

        public RecurrenceTypeRepository(MyBillsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the bill recurrence types
        /// </summary>
        /// <returns></returns>
        public async Task<List<RecurrenceType>> GetRecurrenceTypesAsync()
        {
            List<RecurrenceType> recurrenceTypeList = await _context.RecurrenceType.OrderBy(x => x.Id).ToListAsync();

            return recurrenceTypeList;
        }
    }
}
