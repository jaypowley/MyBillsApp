using System.Linq;
using MyBills.Data.Contexts;
using MyBills.Domain.Entities;
using MyBills.Domain.Interfaces;

namespace MyBills.Data.Repositories
{
    public class UserBillRecurrenceScheduleRepository : IUserBillRecurrenceScheduleRepository
    {
        /// <summary>
        /// Get the recurrence schedule
        /// </summary>
        /// <param name="recTypeId">The recurrence type id</param>
        /// <param name="recModel">The recurrence model</param>
        /// <returns></returns>
        public RecurrenceSchedule GetRecSchedule(int recTypeId, IRecurrenceModel recModel)
        {
            var recModelFormat = recModel.Format;

            RecurrenceSchedule recurrenceSchedule;
            using (var ctx = new MyBillsContext())
            {
                recurrenceSchedule = (from ubrs in ctx.UserBillRecurrenceSchedule
                    where ubrs.RecurrenceTypeId == recTypeId && ubrs.Schedule == recModelFormat
                    select ubrs).FirstOrDefault();
            }

            return recurrenceSchedule ?? (new RecurrenceSchedule
            {
                RecurrenceType = new RecurrenceType{ Id = 0, Name = recModel.Name, Type = recModel.Name},
                RecurrenceTypeId = recTypeId,
                Schedule = recModel.Format,
            });
        }

        /// <summary>
        /// Creates a new recurrence schedule
        /// </summary>
        /// <param name="recurrenceTypeId">The recurrence type id</param>
        /// <param name="schedule">The recurrence schedule</param>
        /// <returns></returns>
        public RecurrenceSchedule CreateNewRecurrenceSchedule(int recurrenceTypeId, string schedule)
        {
            using var ctx = new MyBillsContext();
            var recurrenceType = ctx.RecurrenceType.FirstOrDefault(x => x.Id == recurrenceTypeId);

            var userBillRecurrenceSchedule = new RecurrenceSchedule
            {
                RecurrenceType = recurrenceType,
                RecurrenceTypeId = recurrenceTypeId,
                Schedule = schedule
            };

            ctx.UserBillRecurrenceSchedule.Add(userBillRecurrenceSchedule);

            ctx.SaveChanges();

            return userBillRecurrenceSchedule;
        }
    }
}
