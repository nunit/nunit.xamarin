// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using NUnit.Runner.Helpers;
using NUnit.Runner.Services;
using NUnit.Runner.View;
using Xamarin.Forms;

namespace NUnit.Runner.ViewModel
{
    internal class SummaryViewModel : BaseViewModel
    {
        private readonly TestPackage _testPackage;
        private ResultSummary _summary;
        private bool _running;

        public SummaryViewModel()
        {
            _testPackage = new TestPackage();
            RunTestsCommand = new Command(_ => ExecuteTestsAync(), _ => !Running);
            ViewFailedResultsCommand = new Command(
                _ => Navigation.PushAsync(new ResultsView(new ResultsViewModel(_summary.GetTestResults(), false))),
                _ => !HasResults);

            ExploreTestsCommand = new Command(_ => ExploreTestsAsync(), _ => !Running);
        }

        /// <summary>
        /// User options for the test suite.
        /// </summary>
        public TestOptions Options 
        {
            get
            {
                if (_testPackage.Options == null)
                {
                    _testPackage.Options = new TestOptions();
                }

                return _testPackage.Options;
            }
            set
            {
                _testPackage.Options = value;
            }
        }

        public string ExploreText => $"Explore {_testPackage.TestsCount} tests >";

        /// <summary>
        /// Called from the view when the view is appearing
        /// </summary>
        public void OnAppearing()
        {
            if (Options.AutoRun)
            {
                // Don't rerun if we navigate back
                Options.AutoRun = false;
                RunTestsCommand.Execute(null);
            }
        }

        /// <summary>
        /// The overall test results
        /// </summary>
        public ResultSummary Results
        {
            get 
            { 
                return _summary; 
            }
            set
            {
                if (Set(ref _summary, value))
                {
                    OnPropertyChanged(nameof(HasResults));
                }
            }
        }

        /// <summary>
        /// True if tests are currently running
        /// </summary>
        public bool Running
        {
            get { return _running; }
            set
            {
                Set(ref _running, value);
            }
        }

        /// <summary>
        /// True if we have test results to display
        /// </summary>
        public bool HasResults => Results != null;

        public ICommand RunTestsCommand { set; get; }
        public ICommand ViewFailedResultsCommand { set; get; }
        public ICommand ExploreTestsCommand { set; get; }

        /// <summary>
        /// Adds an assembly to be tested.
        /// </summary>
        /// <param name="testAssembly">The test assembly.</param>
        /// <returns></returns>
        internal void AddTest(Assembly testAssembly)
        {
            _testPackage.AddTestAssembly(testAssembly);
            OnPropertyChanged(nameof(ExploreText));
        }

        private async Task ExploreTestsAsync()
        {
            IEnumerable<TestViewModel> tests;
            if (_summary == null)
            {
                tests = _testPackage.LoadedTests.Select(t => new TestViewModel(t));
            }
            else
            {
                var results = _summary.GetTestResults().AsEnumerable();

                while (results.Count() == 1)
                {
                    results = results.Single().Children;
                }

                tests = results.Select(r => new TestViewModel(r));
            }

            await Navigation.PushAsync(new ExploreView(new ExploreViewModel(tests, "Tests", _testPackage)));
        }

        private async Task ExecuteTestsAync()
        {
            Running = true;
            Results = null;
            var results = await _testPackage.ExecuteTests();
            var summary = await _testPackage.ProcessResults(results);

            Device.BeginInvokeOnMainThread(() =>
            {
                Options.OnCompletedCallback?.Invoke();

                if (Options.TerminateAfterExecution)
                {
                    TerminateWithSuccess();
                    return;
                }

                Results = summary;
                Running = false;
            });
        }

        private static void TerminateWithSuccess()
        {
#if __IOS__
            var selector = new ObjCRuntime.Selector("terminateWithSuccess");
            UIKit.UIApplication.SharedApplication.PerformSelector(selector, UIKit.UIApplication.SharedApplication, 0);
#elif __DROID__
            System.Environment.Exit(0);
#elif WINDOWS_UWP
            Windows.UI.Xaml.Application.Current.Exit();
#endif
        }
    }
}
