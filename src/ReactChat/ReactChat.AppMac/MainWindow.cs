
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace ReactChat.AppMac
{
	public partial class MainWindow : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void AwakeFromNib()
		{
			base.AwakeFromNib ();
			Program.MainMenu = NSApplication.SharedApplication.MainMenu;
			webView.MainFrameUrl = Program.HostUrl;
			webView.Frame = new CoreGraphics.CGRect(0,0,this.Frame.Width,this.Frame.Height);
			this.DidResize += (sender, e) =>  {
				webView.Frame = new CoreGraphics.CGRect(0,0,this.Frame.Width,this.Frame.Height);
			};

			//Hack to fix WebView rendering issue - need to resize just a little bit
			System.Threading.ThreadPool.QueueUserWorkItem (_ => {
				System.Threading.Thread.Sleep(100);
				this.InvokeOnMainThread(() => {
					this.SetFrame(new CoreGraphics.CGRect(this.Frame.Location, new CoreGraphics.CGSize(this.Frame.Size.Width+5, this.Frame.Size.Height+5)), true);
				});
			});
		}
	}
}

