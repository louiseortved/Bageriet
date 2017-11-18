using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bageriet.Startup))]
namespace Bageriet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
