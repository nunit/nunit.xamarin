using System.IO;
using System.Threading.Tasks;
using Foundation;
using NUnit.Runner.Services;
using NUnit.Runner.Tests;
using UIKit;

namespace NUnit.Xamarin.Tests.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // This will load all tests within the current project
            var nunit = new Runner.App();

            // If you want to add tests in another assembly
            nunit.AddTestAssembly(typeof(AsyncTests).Assembly);

            // Available options for testing
            nunit.Options = new TestOptions
            {
                // If True, the tests will run automatically when the app starts
                // otherwise you must run them manually.
                AutoRun = true,

                // Creates a NUnit Xml result file on the host file system using PCLStorage library.
                CreateXmlResultFile = true,

                // Choose a different path for the xml result file (ios file share / library directory)
                ResultFilePath = Path.Combine(NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path, "Results.xml"),

                LogToOutput = true,

                OnCompletedCallback = () =>
                {
                    // var selector = new ObjCRuntime.Selector("terminateWithSuccess");
                    // UIApplication.SharedApplication.PerformSelector(selector, UIKit.UIApplication.SharedApplication, 0);

                    return Task.FromResult(true);
                }
            };

            LoadApplication(nunit);

            return base.FinishedLaunching(app, options);
        }
    }
}
