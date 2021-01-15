using System;
using System.Threading;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms.Internals;
using ServiceStack;
using ServiceStack.Configuration;
using Squirrel;

namespace ReactChat.AppWinForms
{
    public class NativeHost
    {
        private readonly FormMain formMain;

        public NativeHost(FormMain formMain)
        {
            this.formMain = formMain;
            //Enable Chrome Dev Tools when debugging WinForms
#if DEBUG
            formMain.ChromiumBrowser.KeyboardHandler = new KeyboardHandler();
#endif
        }

        public string Platform
        {
            get { return "winforms"; }
        }

        public void ShowAbout()
        {
            MessageBox.Show(@"ServiceStack with CefSharp + ReactJS", @"ReactChat.AppWinForms", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ToggleFormBorder()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.FormBorderStyle = formMain.FormBorderStyle == FormBorderStyle.None
                    ? FormBorderStyle.Sizable
                    : FormBorderStyle.None;
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        public void Shrink()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
            });
        }

        public void Grow()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
            });
        }

        struct Position
        {
            public Position(int top, int height, int pause)
            {
                Top = top;
                Height = height;
                Pause = pause;
            }

            public int Top;
            public int Height;
            public int Pause;
        }

        public void Dance()
        {
            var height = Screen.PrimaryScreen.WorkingArea.Height;
            var positions = new[]
            {
                new Position(0, height, 500),

                new Position(height / 16, height / 16 * 15, 500),
                new Position(height / 16 * 2, height / 16 * 14, 500),
                new Position(height / 16 * 3, height / 16 * 13, 500),
                new Position(height / 16 * 4, height / 16 * 12, 500),
                new Position(height / 16 * 6, height / 16 * 10, 500),
                new Position(height / 2, height / 2, 500),

                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2 - 200, 250),
                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2, 200),

                new Position(height / 2, height / 2 - 100, 250),
                new Position(height / 2, height / 2 - 200, 250),
                new Position(height / 2, height / 2 - 100, 250),

                new Position(height / 2, height / 2, 500),
                new Position(height / 16 * 6, height / 16 * 10, 500),
                new Position(height / 16 * 4, height / 16 * 12, 500),
                new Position(height / 16 * 3, height / 16 * 13, 500),
                new Position(height / 16 * 2, height / 16 * 14, 500),
                new Position(height / 16, height / 16 * 15, 500),

                new Position(0, height, 0),
            };

            2.Times(i =>
            {
                foreach (var position in positions)
                {
                    formMain.InvokeOnUiThreadIfRequired(() =>
                    {
                        formMain.Top = position.Top;
                        formMain.Height = position.Height;
                    });
                    Thread.Sleep(position.Pause);
                }
            });
        }

        public void Quit()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Hide();
                HostContext.Resolve<IServerEvents>().NotifyChannel("home", "cmd.announce", "Quick follow me, lets get out of here..");
                Thread.Sleep(1000);
                HostContext.Resolve<IServerEvents>().NotifyChannel("home", "windows.quit", "");
            });

            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Close();
            });
        }

        public void Ready()
        {
            formMain.InvokeOnUiThreadIfRequired(() =>
            {
                formMain.Controls.Remove(formMain.SplashPanel);
            });
        }

        public void CheckForUpdates()
        {
            var appSettings = new AppSettings();
            var checkForUpdates = appSettings.Get<bool>("EnableAutoUpdate");
            if (!checkForUpdates)
                return;

            var releaseFolderUrl = appSettings.GetString("UpdateManagerUrl");
            try
            {
                var updatesAvailableTask = AppUpdater.CheckForUpdates(releaseFolderUrl);
                updatesAvailableTask.ContinueWith(isAvailable =>
                {
                    isAvailable.Wait(TimeSpan.FromMinutes(1));
                    bool updatesAvailable = isAvailable.Result;
                    //Only check once one launch then release UpdateManager.
                    if (!updatesAvailable)
                    {
                        AppUpdater.Dispose();
                        return;
                    }
                    if (formMain == null)
                    {
                        return;
                    }
                    // Notify web client updates are available.
                    formMain.InvokeOnUiThreadIfRequired(() =>
                    {
                        formMain.ChromiumBrowser.GetMainFrame().ExecuteJavaScriptAsync("window.updateAvailable();");
                    });
                });
            }
            catch (Exception e)
            {
                // Error reaching update server
            }
        }

        public void PerformUpdate()
        {
            AppUpdater.ApplyUpdates(new AppSettings().GetString("UpdateManagerUrl")).ContinueWith(t =>
            {
                formMain.InvokeOnUiThreadIfRequired(() =>
                {
                    formMain.Close();
                    UpdateManager.RestartApp();
                });
            });
        }
    }

#if DEBUG
    public class KeyboardHandler : CefSharp.IKeyboardHandler
    {
        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode,
            CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (windowsKeyCode == (int)Keys.F12)
            {
                Program.Form.ChromiumBrowser.ShowDevTools();
            }
            return false;
        }

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode,
            CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }
    }
#endif
}
