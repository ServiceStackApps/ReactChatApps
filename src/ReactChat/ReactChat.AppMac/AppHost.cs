using System;
using System.Net;
using System.Reflection;
using System.Linq;
using Funq;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Auth;
using ServiceStack.Redis;
using AppKit;
using ReactChat.ServiceInterface;
using ReactChat.Resources;

namespace ReactChat.AppMac
{
	public class AppHost : AppSelfHostBase
	{
		/// <summary>
		/// Default constructor.
		/// Base constructor requires a name and assembly to locate web service classes. 
		/// </summary>
		public AppHost ()
			: base ("ReactChat.AppMac", typeof(ServerEventsServices).Assembly) {}

		/// <summary>
		/// Application specific configuration
		/// This method should initialize any IoC resources utilized by your web service classes.
		/// </summary>
		/// <param name="container"></param>
		public override void Configure (Container container)
		{
			JsConfig.EmitCamelCaseNames = true;

			Plugins.Add(new ServerEventsFeature());

			InitializeAppSettings ();

			SetConfig (new HostConfig {
				DebugMode = AppSettings.Get("DebugMode", false),
				DefaultContentType = MimeTypes.Json,
				EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(SharedEmbeddedResources) },
			});

			Routes.Add<NativeHostAction>("/nativehost/{Action}");
			ServiceController.RegisterService(typeof(NativeHostService));

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

		private void InitializeAppSettings()
		{
			var allKeys = AppSettings.GetAllKeys();
			if(!allKeys.Contains("oauth.RedirectUrl"))
				AppSettings.Set("oauth.RedirectUrl", Program.HostUrl);
			if(!allKeys.Contains("oauth.CallbackUrl"))
				AppSettings.Set("oauth.CallbackUrl", Program.HostUrl + "auth/{0}");
			if(!allKeys.Contains("oauth.twitter.ConsumerKey"))
				AppSettings.Set("oauth.twitter.ConsumerKey", "6APZQFxeVVLobXT2wRZArerg0");
			if (!allKeys.Contains("oauth.twitter.ConsumerSecret"))
				AppSettings.Set("oauth.twitter.ConsumerSecret", "bKwpp31AS90MUBw1s1w0pIIdYdVEdPLa1VvobUr7IXR762hdUn");
			//if (!allKeys.Contains("RedisHost"))
			//    AppSettings.Set("RedisHost", "localhost:6379");
		}
	}

	public class NativeHostService : Service
	{
		public void Any(NativeHostAction request)
		{
			if (string.IsNullOrEmpty(request.Action)) 
				throw HttpError.NotFound ("Function Not Found");

			var nativeHost = typeof(NativeHost).CreateInstance<NativeHost>();
			var methodName = request.Action.Substring(0, 1).ToUpper() + request.Action.Substring(1);
			var methodInfo = typeof(NativeHost).GetMethod(methodName);
			if (methodInfo == null)
				throw new HttpError(HttpStatusCode.NotFound,"Function Not Found");

			methodInfo.Invoke(nativeHost, null);
		}
	}

	public class NativeHostAction : IReturnVoid
	{
		public string Action { get; set; }
	}

	public class NativeHost
	{
		public void ShowAbout()
		{
			//Invoke native about menu item programmatically.
			Program.MainMenu.InvokeOnMainThread (() => {
				foreach (var item in Program.MainMenu.ItemArray()) {
					if (item.Title == "ReactChat") {
						item.Submenu.PerformActionForItem(0);
						return;
					}
				}
			});
		}

		public void Quit()
		{
			Program.MainMenu.InvokeOnMainThread (() => {
				NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication);
			});
		}
	}
}

