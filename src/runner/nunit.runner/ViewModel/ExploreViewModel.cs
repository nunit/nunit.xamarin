// ***********************************************************************
// Copyright (c) 2017 Charlie Poole
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NUnit.Framework.Interfaces;
using NUnit.Runner.Helpers;
using NUnit.Runner.Services;
using NUnit.Runner.View;
using Xamarin.Forms;

namespace NUnit.Runner.ViewModel
{
    internal class ExploreViewModel : BaseViewModel
    {
        private readonly TestPackage _package;

        private bool _running;

        public IEnumerable<TestViewModel> Tests { get; }

        public string Title { get; }

        public string RunText { get; }

        public bool Running
        {
            get 
            { 
                return _running; 
            }
            set
            {
                Set(ref _running, value);
            }
        }

        public ICommand RunTestsCommand { set; get; }
        public Command<TestViewModel> TestDetailsCommand { get; set; }
        public Command<TestViewModel> RunTestCommand { get; set; }
        
        public ExploreViewModel(IEnumerable<TestViewModel> tests, string title, TestPackage package)
        {
            Tests = tests.OrderBy(t => t.Name).ToArray();
            Title = title;

            _package = package;

            RunTestsCommand = new Command(_ => ExecuteTestsAync(), _ => !Running);
            TestDetailsCommand = new Command<TestViewModel>(vm => NavigateToTestDetails(vm), _ => !Running);
            RunTestCommand = new Command<TestViewModel>(vm => RunTest(vm), _ => !Running);

            var totalTests = tests.Sum(t => t.Test.TestCaseCount);
            RunText = $"Run {totalTests} Tests";
        }

        private async Task ExecuteTestsAync()
        {
            Running = true;
            var results = await _package.ExecuteTests(Tests.Select(t => t.Test));

            foreach (var result in results.TestResults.Flatten())
            {
                var testVM = Tests.FirstOrDefault(t => t.Test.FullName == result.Test.FullName);
                if (testVM != null)
                {
                    testVM.Result = result;
                }
            }

            Running = false;
        }

        private async Task RunTest(TestViewModel vm)
        {
            Running = true;

            var run = await _package.ExecuteTests(new[] { vm.Test }, force: !vm.Test.IsSuite);
            vm.Result = run.TestResults.Flatten().FirstOrDefault(t => t.Test.FullName == vm.Test.FullName);

            Running = false;
        }

        private async Task NavigateToTestDetails(TestViewModel vm)
        {
            if (vm.Test.HasChildren)
            {
                await NavigateToChildren(vm);
                return;
            }

            if (vm.Result == null)
            {
                await RunTest(vm);
            }

            await Navigation.PushAsync(new TestView(new TestDetailsViewModel(vm.Result)));
        }

        public async Task SelectTest(TestViewModel vm)
        {
            var test = vm.Test;
            if (test.HasChildren)
            {
                await NavigateToChildren(vm);
            }
            else
            {
                await RunTest(vm);
            }
        }

        private async Task NavigateToChildren(TestViewModel vm)
        {
            IEnumerable<TestViewModel> tests;
            if (vm.Result != null)
            {
                var results = vm.Result.Children
                                .Select(r => new TestViewModel(r));

                var pendingTests = vm.Test.Tests
                                     .Where(t => vm.Result.Children.All(c => c.Test.FullName != t.FullName))
                                     .Select(t => new TestViewModel(t));
                tests = results.Concat(pendingTests);
            }
            else
            {
                tests = vm.Test.Tests.Select(t => new TestViewModel(t));
            }

            await Navigation.PushAsync(new ExploreView(new ExploreViewModel(tests, vm.Test.Name, _package)));
        }
    }
}
