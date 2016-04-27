using System.Net;
using Funq;
using ReactChat.Resources;
using ReactChat.ServiceInterface;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace ReactChat.AppWinForms
{
    public class AppHost : AppSelfHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("ReactChat.AppWinForms", typeof(ServerEventsServices).Assembly)
        {

        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //Plugins.Add(new CorsFeature());

            JsConfig.EmitCamelCaseNames = true;

            Plugins.Add(new ServerEventsFeature());

            SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get("DebugMode", false),
                DefaultContentType = MimeTypes.Json,
                AddRedirectParamsToQueryString = true,
                EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(SharedEmbeddedResources) }
            });

            CustomErrorHttpHandlers.Remove(HttpStatusCode.Forbidden);

            //Register all Authentication methods you want to enable for this web app.            
            Plugins.Add(new AuthFeature(
                () => new AuthUserSession(),
                new IAuthProvider[] {
                    new TwitterAuthProvider(AppSettings)   //Sign-in with Twitter
                }));

            container.RegisterAutoWiredAs<MemoryChatHistory, IChatHistory>();

            var redisHost = AppSettings.GetString("RedisHost");
            if (redisHost != null)
            {
                container.Register<IRedisClientsManager>(new RedisManagerPool(redisHost));

                container.Register<IServerEvents>(c =>
                    new RedisServerEvents(c.Resolve<IRedisClientsManager>()));
                container.Resolve<IServerEvents>().Start();
            }
        }
    }
}
