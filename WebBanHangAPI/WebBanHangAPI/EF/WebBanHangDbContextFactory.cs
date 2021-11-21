using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.EF
{
    public class WebBanHangDbContextFactory : IDesignTimeDbContextFactory<WebBanHangAPIDBContext>
    {
        public WebBanHangAPIDBContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuaration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuaration.GetConnectionString("ConnStr");

            var optionsBuilder = new DbContextOptionsBuilder<WebBanHangAPIDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new WebBanHangAPIDBContext(optionsBuilder.Options);


        }
    }
}
