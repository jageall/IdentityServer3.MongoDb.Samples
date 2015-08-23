using IdentityServer.Host.Config;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.MongoDb;
using Owin;

namespace IdentityServer.Host
{
    internal class Startup
    {
        private readonly StoreSettings _settings;

        public Startup(StoreSettings settings)
        {
            _settings = settings;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            var factory = new IdentityServer3.MongoDb.ServiceFactory(
                new Registration<IUserService>(new InMemoryUserService(Users.Get())),
                _settings
                );
            var options = new IdentityServerOptions
            {
                SiteName = "IdentityServer3 (self host)",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
            };

            appBuilder.UseIdentityServer(options);
        }
    }
}