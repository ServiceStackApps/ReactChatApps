using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Funq;
using ReactChat.Resources;
using ReactChat.ServiceInterface;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace ReactChat.AppConsole
{
    public class AppHost : AppSelfHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("ReactChat.AppConsole", typeof(ServerEventsServices).Assembly)
        {
            var customSettings = new FileInfo("appsettings.txt");
            AppSettings = customSettings.Exists
                ? (IAppSettings)new TextFileSettings(customSettings.FullName)
                : new AppSettings();
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

            InitializeAppSettings();

            SetConfig(new HostConfig
            {
                DebugMode = true,
                EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(SharedEmbeddedResources) }
            });

            JsConfig.EmitCamelCaseNames = true;

            Plugins.Add(new ServerEventsFeature());

            SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get("DebugMode", false),
                DefaultContentType = MimeTypes.Json,
                AddRedirectParamsToQueryString = true,
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

            // This route is added using Routes.Add and ServiceController.RegisterService due to
            // using ILMerge limiting our AppHost : base() call to one assembly.
            // If two assemblies are used, the base() call searchs the same assembly twice due to the ILMerged result.
            Routes.Add<NativeHostAction>("/nativehost/{Action}");
            ServiceController.RegisterService(typeof(NativeHostService));
        }

        private void InitializeAppSettings()
        {
            var allKeys = AppSettings.GetAllKeys();
            if (!allKeys.Contains("oauth.RedirectUrl"))
                AppSettings.Set("oauth.RedirectUrl", Program.HostUrl);
            if (!allKeys.Contains("oauth.CallbackUrl"))
                AppSettings.Set("oauth.CallbackUrl", Program.HostUrl + "auth/{0}");
            if (!allKeys.Contains("oauth.twitter.ConsumerKey"))
                AppSettings.Set("oauth.twitter.ConsumerKey", "6APZQFxeVVLobXT2wRZArerg0");
            if (!allKeys.Contains("oauth.twitter.ConsumerSecret"))
                AppSettings.Set("oauth.twitter.ConsumerSecret", "bKwpp31AS90MUBw1s1w0pIIdYdVEdPLa1VvobUr7IXR762hdUn");
            //if (!allKeys.Contains("RedisHost"))
            //    AppSettings.Set("RedisHost", "localhost:6379");
        }
    }

    public class NativeHostService : Service
    {
        public object Get(NativeHostAction request)
        {
            if (string.IsNullOrEmpty(request.Action))
            {
                throw HttpError.NotFound("Function Not Found");
            }
            Type nativeHostType = typeof(NativeHost);
            object nativeHost = nativeHostType.CreateInstance<NativeHost>();
            //Upper case first character.
            string methodName = request.Action.First().ToString().ToUpper() + String.Join("", request.Action.Skip(1));
            MethodInfo methodInfo = nativeHostType.GetMethod(methodName);
            if (methodInfo == null)
            {
                throw new HttpError(HttpStatusCode.NotFound, "Function Not Found");
            }
            methodInfo.Invoke(nativeHost, null);
            return null;
        }
    }

    public class NativeHostAction : IReturnVoid
    {
        public string Action { get; set; }
    }

    public class NativeHost
    {
        public void Quit()
        {
            Environment.Exit(0);
        }
    }
}
