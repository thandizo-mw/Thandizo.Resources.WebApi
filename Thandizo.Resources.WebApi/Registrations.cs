using Microsoft.Extensions.DependencyInjection;
using Thandizo.FacilityResources.BLL.Services;

namespace Thandizo.Resources.WebApi
{
    public static class Registrations
    {
        /// <summary>
        /// Registers domain services to the specified
        /// service descriptor
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IResourceAllocationService, ResourceAllocationService>();
            services.AddScoped<IHealthFacilityResourceService, HealthFacilityResourceService>();
            return services.AddScoped<IResourceService, ResourceService>();
        }
    }
}
