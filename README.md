# React Chat Apps

A port of 
[React Chat](https://github.com/ServiceStackApps/Chat-React) 
built with the new 
**[React Desktop Apps](https://github.com/ServiceStackApps/ReactDesktopApps)**
VS.NET template which is used to package the ASP.NET React Chat Web App into a native 
**Winforms App** on Windows, a **Cocoa App** on OSX and a cross-platform **Console App** runnable on Win/OSX/Linux.
The primary ASP.NET Web App remains unaffected and includes build task for easy deployments 
using MS Web Deploy which is used to deploy the [reactchatapps.servicestack.net](http://reactchatapps.servicestack.net/) Live Demo.

The template takes advantage of [CefSharp](https://github.com/cefsharp/CefSharp) on Windows and 
a Safari WebView on OSX to allow targeting of modern WebKit Browsers whilst integrating 
within a native application to enable a rich Desktop App User Experience from a single portable .exe. 

[![React Desktop Apps](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/gap/react-desktop-splash.png)](https://github.com/ServiceStackApps/ReactDesktopApps)

The [React Desktop Apps](https://github.com/ServiceStackApps/ReactDesktopApps) 
template is setup ready to deploy to multiple target platforms, just by running a grunt task after creating our solution, we have 3 working applications from Visual Studio including:

- **Web** - Ready to deploy.
- **Console** - Single portable, cross platform executable that utilises the user's default browser.
- **Windows** - Native Windows application using an embedded browser.

Additionally, an **OSX** project using Xamarin.Mac is generated preconfigured and ready to run! 
Web resources and services are shared between the Xamarin.Mac and Visual Studio solutions 
maximizing code reuse and having the ability to hook into native functionality in OSX using **Xamarin.Mac**.

![WinForms application with loading splash screen](https://github.com/ServiceStack/Assets/raw/master/img/livedemos/react-desktop-apps/redis-chat-app.gif)

### Controlling multiple Windows with Server Events

React Chat uses Server Events for real-time communication with JavaScript, which is able to control 
multiple window clients naturally just by having each Windows Application subscribe to the same 
remote `/event-stream` url. You can do in React Chat just by opening multiple windows, as all subesquent 
Windows Apps opened listen to the self-hosting listener of the first one that was opened. 

The `/windows.dance` chat message provides a nice demonstration of this in action :)

#### [YouTube Live Demo](https://youtu.be/-9kVqdPbqOM)

[![](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/livedemos/react-desktop-apps/dancing-windows.png)](https://youtu.be/-9kVqdPbqOM)

## Downloads

#### Windows Winforms App

[ReactChat-winforms.exe](https://github.com/ServiceStackApps/ReactChatApps/raw/master/dist/ReactChat-winforms.exe) (23.6 MB)

#### OSX Cocoa App

[ReactChat.AppMac.mono.app.zip](https://github.com/ServiceStackApps/ReactChatApps/raw/master/dist/ReactChat.AppMac.mono.app.zip) (16.9 MB) 

Without Mono Embedded (requires Mono): 

[ReactChat.AppMac.app.zip](https://github.com/ServiceStackApps/ReactChatApps/raw/master/dist/ReactChat.AppMac.app.zip) (4.51 MB)

#### Console App (Windows/OSX/Linux)

[ReactChat-console.exe](https://github.com/ServiceStackApps/ReactChatApps/raw/master/dist/ReactChat-console.exe) (5.33 MB) or [DefaultApp-console.zip](https://github.com/ServiceStackApps/ReactChatApps/raw/master/dist/ReactChatApps-console.zip) (1.93MB)

## Project Structure
Just like other templates in ServiceStackVS, the **React Desktop Apps** template provides the same recommended structure as well as 3 additional other projects for producing the Console and WinForms applications.

![](https://github.com/ServiceStack/Assets/raw/master/img/servicestackvs/react-desktop-apps-proj-structure.png)

- **ReactChat** - Web applicaton which contains all our resources and files used while developing.
- **ReactChat.AppConsole*** - Console application, launches default browser on users application
- **ReactChat.AppWinForms*** - WinForms application using CefSharp and Chromium Embedded Framework to output our web application in a native application.
- **ReactChat.Resources*** - Embedded resources that are used by our AppWinForms and AppConsole application and target of `01-bundle-all` Grunt task. This project has references to all minified client resources (CSS, JavaScript, images, etc) and includes each of them as an *Embedded Resource*.
- **ReactChat.ServiceInterface** - Contains ServiceStack services.
- **ReactChat.ServiceModel** - Contains request/response classes.
- **ReactChat.Tests** - Contains NUnit tests. 


#### ReactChat Project
This project contains all our development resources, JS/JSX, CSS, images, html/cshtml, etc. This project also has all the required Grunt/Gulp tasks used for deploying the 3 application outputs. Taking advantage of Visual Studio 2015's Task Runner Explorer, we can look at the `Alias` tasks to get an idea of how we can build and deploy our console, winforms and web application.

![](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/gap/react-desktop-tasks.png)

- **default** - grunt task builds and packages both the console and winforms projects by running `02-package-console` and `03-package-winforms`.
- [**01-bundle-all**](#01-bundle-all) - bundles all the application resources into the `Resources` project and into `wwwroot` to stage the web application for deployment
- [**02-package-console**](#02-package-console) - bundles and packages the console application and produces the result in `wwwroot_build\apps` directory.
- [**03-package-winforms**](#03-package-winforms) - bundles and packages the winforms application and produces the result in `wwwroot_build\apps` directory.
- [**04-deploy-webapp**](#04-deploy-webapp) - bundles, packages and deploys the web application using the `wwwroot_build\publish\config.json` file settings and webdeploy to your existing IIS server.

This project also has includes ILMerge and 7zip tools to help package the console and winforms application ready for release. The `wwwroot_build` folder contains the following structure.

The `/wwwroot_build` folder contains the necessary files required for deployments including:

```
/wwwroot_build
  /apps                       # output directory of console and winforms applications
  /deploy                     # copies all files in folder to /wwwroot
    appsettings.txt           # production appsettings to override dev defaults
  /publish                    
    config.json               # deployment config for WebDeploy deployments
  /tools                      # deployment tools for console and winforms applications
    7za.exe                   # 7zip console for packaging
    7zsd_All.sfx	          # 7zip Self Extract module used for bundling winforms app to self executing zip
    ILMerge.exe			      # ILMerge to merge console app output into single binary
  00-install-dependencies     # runs NPM install and bower install, used when getting started after cloning application
  config-winforms.txt         # 7zip SFX config for self executing zip
  package-deploy-console.bat  # runs ILMerge to package the console application
  package-deploy-winforms     # stagings winforms app and packages using 7zip SFX and config-winforms.txt
```

The minimum steps to deploy an app is to fill in `config.json` with the remote IIS WebSite settings as well as a UserName and Password of a User that has permission to remote deploy an app:

```json
{
    "iisApp": "AppName",
    "serverAddress": "deploy-server.example.com",
    "userName": "{WebDeployUserName}",
    "password" : "{WebDeployPassword}"
}
```

#### ReactChat.AppConsole
This project is for producing a SelfHost ServiceStack application that utilizes the user's default browser. Combined with the Grunt/Gulp and ILMerge, we can produce a cross-platform, single executable that has embedded resources used by our application.

This project uses the bundled resources from the web application that are bundled using the Grunt/Gulp tasks. These resources are embedded in the `ReactChat.Resources` and the AppHost needs to be configured to look for these embedded resources.

`SharedEmbeddedResources` is a class in the `ReactChat.Resources` project so we can easily refer to it's assembly with `typeof(SharedEmbeddedResources).Assembly`. 

For our other resources, we need to set the `EmbeddedResourceBaseTypes` to both our current project and the `ReactChat.Resources` using the `SharedEmbeddedResources` type.

```
SetConfig(new HostConfig
{
    EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(SharedEmbeddedResources) }
});
```

>We need to specify base types instead of assemblies so their namespaces are preserved once they're ILMerged into a single .exe

#### ReactChat.AppWinForms
This project utilizes the CefSharp project for embedding a high performing Chromium browser in a WinForms application. This project, also uses the bundled resources from the web application via the `ReactChat.Resources` project as well being a `AppSelfHostBase` based application, we need to set the same config as our `ReactChat.AppConsole` application in the AppHost.

``` csharp
SetConfig(new HostConfig
{
    EmbeddedResourceBaseTypes = { typeof(AppHost), typeof(SharedEmbeddedResources) }
});
```

To embed the Chromium web browser, we reference the `CefSharp.WinForms` project and instantiate a `ChromiumWebBrowser` specifying the applications URL, in this case `http://localhost:1337/`. When using `CefSharp.WinForms` reference, `ChromiumWebBrowser` is a WinForms control that is added to our Form. We also bind `FormClosing`, `FormClosed` and `Load` WinForms events to give the application more of a native feel.

```csharp
public FormMain()
{
    InitializeComponent();
    VerticalScroll.Visible = false;
    ChromiumBrowser = new ChromiumWebBrowser(Program.HostUrl)
    {
        Dock = DockStyle.Fill
    };
    Controls.Add(ChromiumBrowser);

    Load += (sender, args) =>
    {
        Left = Top = 0;
        Width = Screen.PrimaryScreen.WorkingArea.Width;
        Height = Screen.PrimaryScreen.WorkingArea.Height;
    };

    FormClosing += (sender, args) =>
    {
        //Make closing feel more responsive.
        Visible = false;
    };

    FormClosed += (sender, args) =>
    {
        Cef.Shutdown();
    };

    ChromiumBrowser.RegisterJsObject("nativeHost", new NativeHost(this));
}
```

CefSharp also enabled integration between JavaScript and native calls via exposing JavaScript objects that are registered .NET classes. In ReactChat and the React Desktop Apps ServiceStackVS template, we wire a default `nativeHost` object to show how this can be leveraged. One to simply show a message box when "About" is clicked and the other to close the application. The .NET classes are POCOs that have matching function names with the JavaScript object registered. The default setting is to camel case the JS object following the common naming conventions when using JS.

```csharp

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
            formMain.Left = formMain.Top = 0;
            formMain.Width = Screen.PrimaryScreen.WorkingArea.Width;
            formMain.Height = Screen.PrimaryScreen.WorkingArea.Height;
        });
    }
...
```

The `NativeHost` class is exposed by CefSharp as a JavaScript object with functions and properties. The `NativeHost` object is common on all platforms, but the implementation is different to get access to native functionality. For platforms other than `AppWinForms`, we need to provide the `nativeHost` JavaScript object via the embedded resource `platform.js`.

#### ReactChat.Resources
This project has references to the output files from the `01-bundle-all` Grunt task. If any additional images or minified JS/CSS files are added to your project, they must be referenced by this project to be included as an embedded resource for use in both AppConsole and AppWinForms projects. The structure of the project follows what is deployed in the `wwwroot` project.

```
/wwwroot
  /css
    app.min.css
  /img              #  all application images
  /js
    app.jsx.js
  /lib
    /css            # 3rd party css, eg bootstrap
    /fonts          # 3rd party fonts
    /js             # 3rd party minified JS
      lib.min.js
  default.cshtml/default.html
```

All files have a `Build Action` of `Embedded Resource` so they are ready to be used from AppConsole and AppWinForms.

![](https://github.com/ServiceStack/Assets/raw/master/img/servicestackvs/react-desktop-apps-embedded-resource.png)

#### ReactChat.AppMac
The React Desktop Apps template also generates a **Xamarin.Mac** project and solution ready to run resuing the shared `ServiceInterface`, `ServiceModel` and `Resources` project. As all the shared web resources are embedded in the `Resources` dll, the `01-bundle-all` stages the output of this project at the solution level under a `lib` folder. 

![](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/livedemos/react-desktop-apps/react-chat-sln-folders.png)

If you are working across platform and want to update the `AppMac` project with the latest `Resources`, a common workflow might be to use Git and commit the `lib` folder to source control after changes and updating the local repository on an OSX machine to build the `AppMac` project.

Debugging CSS or JavaScript issues in the `AppMac` project can be done by using the native webview 

# Platform specific JavaScript and CSS

As a way to override styles and behaviour, by convention we load `platform.js` and `platform.css`. In the React Desktop Apps template these are use to provide or override platform specific hooks/behaviour, eg OSX about dialog.

``` javascript
/* mac */
document.documentElement.className += ' mac';
window.nativeHost = {
    quit: function () {
        $.post('/nativehost/quit');
    },
    showAbout: function () {
    	$.post('/nativehost/showAbout');
    },
    ready: function () {
        //
    },
    platform: 'mac'
};
```

`platform.js` and `platform.css` are included in each native platform project as **embedded resource** so they can be hosted in the outputed executable or in the case of shared resources, in the `Resources` project.

>For AppWinForms, CefSharp is being used to provide the `nativeHost` JavaScript object before page is rendered and the native hooks will be used instead.

# Grunt Tasks
Grunt and Gulp are used in the ReactChat project to automate our bundling, packaging and deployment of the applications. These tasks are declared as small, single responsibility Grunt tasks and then orchastrated using Alias tasks to be able to run these simply either from Visual Studio using the Task Runner Explorer or from the command line.

### Including/Copying additional files
To provide an easy way to copy files to `wwwroot` and `Resources` project, eg images, fonts etc, a `COPY_FILES` array is declared at the top of the `gruntfile.js` where additional assets can be added. Each object added has a `src`,`dest`, `host` and `afterReplace`.

- `src`* - Can be a single file or wildcard notation supported by `gulp.src`. 
- `dest` - Destination folder relative to hosts. Must end with a `/`.
- `host` - `web` for `wwwroot` directory in main web project or `native` for `Resources` project.
- `afterReplace` - find replace object array expecting `from` and `to` properties. Uses `gulpReplace`.

``` JavaScript
var WEB = 'web';
var NATIVE = 'native';

var COPY_FILES = [
    { src: './bin/**/*', dest: 'bin/', host: WEB },
    { src: './img/**/*', dest: 'img/' },
    { src: './App_Data/**/*', dest: 'App_Data/', host: WEB },
    { src: './Global.asax', host: WEB },
    { src: './bower_components/bootstrap/dist/fonts/*.*', dest: 'lib/fonts/' },
    { src: './platform.js', dest: 'js/', host: WEB },
    { src: './wwwroot_build/deploy/*.*', host: WEB },
    {
        src: './web.config',
        host: [WEB],
        afterReplace: [{
            from: '<compilation debug="true" targetFramework="4.5"',
            to: '<compilation targetFramework="4.5"'
        }]
    }
];
```

## 01-bundle-all
Just like the AngularJS and React App template, we stage our application ready for release and avoid any build steps at development time to improve the simplicity and speed of the development workflow. This alias task is made up of small, simple tasks that use Gulp to process resources and perform tasks like minification, JSX transformation, copying/deleting of resources, etc.

The bundling searches for assets in any `**/*.html` file and follows build comments to minify and replace references. This enables simple use of debug JS files whilst still having control how our resources minify. The ReactChat project uses Razor so this step has been updated to bundle just `default.cshtml`.

```html
<!-- build:js lib/js/lib.min.js -->
<script src="bower_components/jquery/dist/jquery.js"></script>
<script src="bower_components/react/react.js"></script>
<script src="bower_components/reflux/dist/reflux.js"></script>
<!-- endbuild -->

<!-- build:js js/app.jsx.js -->
<script type="text/javascript" src="js/components/Actions.js"></script>
<script type="text/jsx" src="js/components/User.jsx">
</script>
<script type="text/jsx" src="js/components/Header.jsx">
</script>
<script type="text/jsx" src="js/components/Sidebar.jsx">
</script>
<script type="text/jsx" src="js/components/ChatLog.jsx">
</script>
<script type="text/jsx" src="js/components/Footer.jsx">
</script>
<script type="text/jsx" src="js/components/ChatApp.jsx">
</script>
<!-- endbuild -->
```

When creating new JS files for your application, they should be added in the `build:js js/app.jsx.js` comments shown above. `build:remove` is used to remove the use of the runtime JSX transformer that we use for our React components, but is not longer needed ([and recommended not to be used in a production environment](https://facebook.github.io/react/docs/tooling-integration.html)) in our deployed application.


## 02-package-console
This task also performs `01-build-all` as well restoring NuGet packages and building the **AppConsole** project. Once the project resources are ready, it calls the `package-deploy-console.bat` batch file which, using **ILMerge**, produces the stand alone exe of the console application and copies it to `apps` output directory.

``` bat
SET STAGING=staging-console

...

MD %STAGING%

SET TOOLS=.\tools
SET OUTPUTNAME=ReactChat.Console.exe
SET ILMERGE=%TOOLS%\ILMerge.exe
SET RELEASE=..\..\ReactChat.AppConsole\bin\x86\Release
SET INPUT=%RELEASE%\ReactChat.AppConsole.exe
SET INPUT=%INPUT% %RELEASE%\ReactChat.Resources.dll
SET INPUT=%INPUT% %RELEASE%\ReactChat.ServiceInterface.dll
SET INPUT=%INPUT% %RELEASE%\ReactChat.ServiceModel.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Text.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Client.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Common.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Interfaces.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Server.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.OrmLite.dll
SET INPUT=%INPUT% %RELEASE%\ServiceStack.Redis.dll

%ILMERGE% /target:exe /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:%STAGING%\%OUTPUTNAME% /ndebug %INPUT% 

IF NOT EXIST apps (
MD apps
)

COPY /Y .\%STAGING%\%OUTPUTNAME% .\apps\
```

## 03-package-winforms
This task also performs `01-build-all` as well restoring NuGet packages and building the **AppWinForms** project. Once the project resources are ready, it calls `package-deploy-winforms.bat` which uses 7zip SFX to zip and compresses the CefSharp.WinForms ReactChat.AppWinForms application in a self executing zip package.

``` batch
SET STAGING=staging-winforms

...

MD %STAGING%

SET TOOLS=.\tools
SET STAGINGZIP=ReactChat-winforms.7z
SET OUTPUTNAME=ReactChat-winforms.exe
SET RELEASE=..\..\ReactChat.AppWinForms\bin\x86\Release
COPY %RELEASE%\ReactChat.AppWinForms.exe .\%STAGING%
COPY %RELEASE%\ReactChat.AppWinForms.exe.config .\%STAGING%
COPY %RELEASE%\CefSharp.BrowserSubprocess.exe .\%STAGING%
ROBOCOPY "%RELEASE%" ".\%STAGING%" *.dll *.pak *.dat /E

...

cd tools && 7za a ..\%STAGINGZIP% ..\%STAGING%\* && cd..
COPY /b .\tools\7zsd_All.sfx + config-winforms.txt + %STAGINGZIP% .\apps\%OUTPUTNAME%

```
By default, all the files required for the Chromium Embedded Framework are included in the template script.

```
COPY %RELEASE%\ReactChat.AppWinForms.exe .\staging-winforms
COPY %RELEASE%\ReactChat.AppWinForms.exe.config .\staging-winforms
ROBOCOPY "%RELEASE%" ".\staging-winforms" *.dll *.pak *.dat /E
```

Once all the required files in are staged in the `staging-winforms`, this directory's contents gets zipped into a `.7z` compressed file, then packaged into a self executing zip using the `config-winforms.txt` file. 

``` txt
;!@Install@!UTF-8!
ExecuteFile="ReactChat.AppWinForms.exe"
GUIMode="2"
;!@InstallEnd@!
```
Configuration options for 7z SFX can be found in the [7z SFX documentation](http://7zsfx.info/en/configinfo.html).

The ReactChatApp solution is using a modified version of the `7zsd_All.sfx` file which generates the self executable with the custom ServiceStack `.ico` file. More information on how to change this to a custom icon can be found on the [7zsfx.info](http://7zsfx.info/en/icon.html) site.

## 04-deploy-webapp

This Grunt task uses the same conventions as those found in the AngularJS and ReactApp template in ServiceStackVS. WebDeploy is used to deploy the application from the staged `wwwroot` folder to an existing IIS application. Config for the deployment, eg the IIS Server address, application name, username and password is located in the `/wwwroot_build/publish/config.js`. 

    {
        "iisApp": "YourAppName",
        "serverAddress": "deploy-server.example.com",
        "userName": "{WebDeployUserName}",
        "password" : "{WebDeployPassword}"
    }

If you are using **Github's default Visual Studio ignore, this file will not be included in source control** due to the default rule of `publish/` to be ignored. You should check your Git Repository `.gitignore` rules before committing any potentially sensitive information into public source control.

This task shows a quick way of updating your development server quickly after making changes to your application. For more information on use web-deploy using either Grunt or just Visual Studio publish, see '[WebDeploy with AWS](https://github.com/ServiceStack/ServiceStack/wiki/WebDeploy-with-AWS#deploy-using-grunt)'.

# Design-time only resources

Gulp also supports design-time vs run-time dependencies with the `build:remove` task which can be used to remove any unnecessary dependencies not required in production like react's `JSXTransformer.js`:

```html
<!-- build:remove -->
<script src="bower_components/react/JSXTransformer.js"></script>
<!-- endbuild -->
```

React's `JSXTransformer.js` is what enables the optimal experience of letting you directly reference `.jsx` files in HTML as if they were normal `.js` files by transpiling and executing `.jsx` files directly in the browser at runtime - avoiding the need for any manual pre-compilation steps and retaining the fast `F5` reload cycle that we've come to expect from editing `.js` files. 

```html
<!-- build:js js/app.jsx.js -->
<script type="text/jsx" src="js/components/Actions.js"></script>
<script type="text/jsx" src="js/components/User.jsx"></script>
<script type="text/jsx" src="js/components/Header.jsx"></script>
<script type="text/jsx" src="js/components/Sidebar.jsx"></script>
<script type="text/jsx" src="js/components/ChatLog.jsx"></script>
<script type="text/jsx" src="js/components/Footer.jsx"></script>
<script type="text/jsx" src="js/components/ChatApp.jsx"></script>
<!-- endbuild -->
```

Then when the client app is packaged, all `.jsx` files are compiled and minified into a single `/js/app.jsx.js` with the reference to `JSXTransformer.js` also stripped from the optimized HTML page as there's no longer any need to transpile and execute `.jsx` files at runtime.

**For more info on working with React, see the [Chat-React project](https://github.com/ServiceStackApps/Chat-React#introducing-reactjs) documentation.** 
