using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SubscriptionService.Data;
using SubscriptionService.Dtos;
using SubscriptionService.Models;

namespace SubscriptionService.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPlan>> GetPlansAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }

        public async Task<SubscriptionPlan> GetPlansAsync(int Id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(Id);
            if (plan == null)
                return new();
            return plan;
        }

        public async Task<bool> SubscribeAsync(SubscribeRequestDto dto)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(dto.PlanId);
            if (plan == null)
                return false;

            var newSub = new UserSubscription
            {
                UserId = dto.UserId,
                SubscriptionPlanId = plan.Id,               
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(plan.DurationInDays),
                ReferenceId=dto.ReferenceId
            };

            _context.UserSubscriptions.Add(newSub);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ActivateSubscription(string ReferenceId)
        {
            bool result = false;
            try
            {
                var subscription = await _context.UserSubscriptions.Where(us => us.ReferenceId == ReferenceId).FirstOrDefaultAsync();
                if (subscription == null) return false;
                subscription.IsActive = true;
                _context.Entry(subscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public async Task<List<SubscriptionPlan>>  GetActivePlan(string UserId)
        {
            try
            {
                var list = await _context.UserSubscriptions.Include(us=>us.SubscriptionPlan).Where(us=>us.UserId == UserId)
                    .Select(us=>us.SubscriptionPlan).ToListAsync();
                if(list==null || list.Count==0) return new();    
                return list;
            }
            catch (Exception ex)
            {
                return new();
            }
        }
    }

}