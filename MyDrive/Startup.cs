using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyDrive.Startup))]
namespace MyDrive
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
