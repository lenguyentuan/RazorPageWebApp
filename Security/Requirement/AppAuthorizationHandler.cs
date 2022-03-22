using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using App.Models;

namespace App.Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;


        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();

            foreach (var requirement in requirements)
            {
                if (requirement is GenzRequirement)
                {
                    if (IsGenZ(context.User, (GenzRequirement)requirement)) context.Succeed(requirement);
                }
                if (requirement is ArticleUpdateRequirement)
                {
                    bool canupdate = CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)requirement);
                    if (canupdate) context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object resource, ArticleUpdateRequirement requirement)
        {
            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin update article");
                return true;
            }
            var article = resource as Article;
            var dateCreated = article.created;
            var dateCanUpdate = new DateTime(requirement.Year, requirement.Month, requirement.Date);
            if (dateCreated <= dateCanUpdate)
            {
                _logger.LogInformation("date update was expired");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenzRequirement requirement)
        {
            var AppUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(AppUserTask);
            var appUser = AppUserTask.Result;

            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} did not have DOB");
                return false;
            }

            int year = appUser.BirthDate.Value.Year;



            var success = (year >= requirement.FromYear && year <= requirement.ToYear);
            if (success)
            {
                _logger.LogInformation($"{appUser.UserName} meet GenZRequirement");
            }
            else
            {
                _logger.LogInformation($"{appUser.UserName} can not meet GenZRequirement");
            }
            return success;
        }
    }
}