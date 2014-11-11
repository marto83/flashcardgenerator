using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FlashCardGenerator.Startup))]
namespace FlashCardGenerator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}