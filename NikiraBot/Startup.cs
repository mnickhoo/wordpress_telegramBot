using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(wordpressBot.Startup))]
namespace wordpressBot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
