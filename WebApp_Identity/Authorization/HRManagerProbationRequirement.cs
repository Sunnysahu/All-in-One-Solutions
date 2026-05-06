using Microsoft.AspNetCore.Authorization;

namespace WebApp_Identity.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }

    public class HRManagerProbingRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (context.User.HasClaim(x => x.Type == "EmplomentDate")) return Task.CompletedTask;

            if(DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value, out DateTime employementDate))
            {
                var period = DateTime.Now - employementDate;
                if (period.Days > 30 * requirement.ProbationMonths) context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
