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

using System;
using System.IO;
using System.Reflection;

using NUnit.Runner.Services;
using NUnit.Runner.View;
using NUnit.Runner.ViewModel;
using Xamarin.Forms;

namespace NUnit.Runner
{
    /// <summary>
    /// The NUnit Xamarin test runner
    /// </summary>
    public partial class App : Application
    {
        private readonly SummaryViewModel _model;

        /// <summary>
        /// Constructs a new app adding the current assembly to be tested.
        /// <param name="output">Stream where the output is redirected. Default value is Console.Out.</param>
        /// </summary>
        public App(TextWriter output = null)
        {
            InitializeComponent();

            RealConsole.Init(output ?? Console.Out);

            // OnPlatform only reports WinPhone for WinPhone Silverlight, so swap
            // out the background color in code instead
            if (Device.RuntimePlatform == Device.UWP)
            {
                Resources["defaultBackground"] = Resources["windowsBackground"];
            }

            _model = new SummaryViewModel();
            MainPage = new NavigationPage(new SummaryView(_model));
            AddTestAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds an assembly to be tested.
        /// </summary>
        /// <param name="testAssembly">The test assembly.</param>
        public void AddTestAssembly(Assembly testAssembly)
        {
            _model.AddTest(testAssembly);
        }

        /// <summary>
        /// User options for the test suite.
        /// </summary>
        public TestOptions Options
        {
            get { return _model.Options; }
            set { _model.Options = value; }
        }
    }
}
