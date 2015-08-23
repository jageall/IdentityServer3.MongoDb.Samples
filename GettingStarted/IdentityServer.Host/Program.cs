using System;
using System.Threading.Tasks;
using IdentityServer.Host.Config;
using IdentityServer3.Admin.MongoDb;
using IdentityServer3.MongoDb;
using Microsoft.Owin.Hosting;
using Serilog;

namespace IdentityServer.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = StoreSettings.DefaultSettings();
            /* Equivilant to:
            var settings = new StoreSettings
            {
                ConnectionString = "mongodb://localhost",
                Database = "identityserver",
                ClientCollection = "clients",
                ScopeCollection = "scopes",
                ConsentCollection = "consents",
                AuthorizationCodeCollection = "authorizationCodes",
                RefreshTokenCollection = "refreshtokens",
                TokenHandleCollection = "tokenhandles"
            };
            */


            SetupDatabase(settings).Wait();

            Console.Title = "IdentityServer3 SelfHost";

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            const string url = "https://localhost:44333/core";
            var startup = new Startup(settings);
            using (WebApp.Start(url, startup.Configuration))
            {
                Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                Console.ReadLine();
            }
        

    }
        
        static async Task SetupDatabase(StoreSettings settings)
        {
            //This setup script should really be run as a job during deployment and is
            //only here to illustrate how the database can be setup from code
            var adminService = AdminServiceFactory.Create(settings);
            await adminService.CreateDatabase();
            foreach (var client in Clients.Get())
            {
                await adminService.Save(client);
            }

            foreach (var scope in Scopes.Get())
            {
                await adminService.Save(scope);
            }
        }
    }
}
