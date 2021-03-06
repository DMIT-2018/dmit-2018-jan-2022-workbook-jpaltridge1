using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional Namespaces
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AppSecurity.DAL;
using AppSecurity.BLL;
#endregion

namespace AppSecurity
{
    public static class SecurityExtentions
    {
        public static void AppSecurityBackendDependancies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {

            //register the DbContent class in Chinook with the service collection

            services.AddDbContext<AppSecurityDbContext>(options);

            //add any services that you create in class library
            //using .AddTransient<T>(..)

            services.AddTransient<SecurityService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<AppSecurityDbContext>();

                //create an instance of the service and return the instance

                return new SecurityService(context);
            });
           
        }
    }
}
