using System;
using ServiceStack.Text;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace ReactChat.AppMac
{
	public static class Program
	{
		public static string HostUrl = "http://localhost:3337/";

		public static AppHost App;
		public static NSMenu MainMenu;

		static void Main (string[] args)
		{
			try
			{
				App = new AppHost();
				App.Init().Start("http://*:3337/");
			}
			catch 
			{
				"Use existing AppHost found on {0}".Print(HostUrl);
			}

			NSApplication.Init();
			NSApplication.Main(args);
		}
	}
}

