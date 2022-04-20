// ***********************************************************************
// Copyright (c) 2015 NUnit Project
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

using NUnit.Runner.Helpers;
using NUnit.Runner.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NUnit.Runner.ViewModel
{
    class TestExplorerViewModel : BaseViewModel
    {
        readonly TestPackage _testPackage;
        readonly SummaryViewModel _summaryViewModel;
        bool _busy;

        public TestExplorerViewModel(TestPackage testPackage, SummaryViewModel summaryViewModel)
        {
            _testPackage = testPackage;
            _summaryViewModel = summaryViewModel;
            TestList = new ObservableCollection<GroupedTestsViewModel>();

            RunTestCommand = new Command(
                async o =>
                {
                    if(o is TestDescriptionViewModel testVm) 
                    {
                        Busy = true;
                        var results = await _testPackage.ExecuteTests(new[] { testVm.FullName });
                        await _summaryViewModel.DisplayResults(results);
                        await Navigation.PopAsync();
                    }                    
                });
        }

        public async Task LoadTestsAsync()
        {
            var groupedTests = (await _testPackage.EnumerateTestsAsync()).GroupBy(t => t.Test.ClassName);
            Device.BeginInvokeOnMainThread(() =>
            {
                Busy = true;

                TestList.Clear();

                foreach (var testClass in groupedTests)
                {
                    TestList.Add(new GroupedTestsViewModel(testClass.Key, testClass.Select(t => new TestDescriptionViewModel(t.Test, t.Assembly))));
                }

                Busy = false;
            });          
        } 

        public ObservableCollection<GroupedTestsViewModel> TestList { get; }

        public ICommand RunTestCommand { set; get; }

        /// <summary>
        /// True if tests are being loaded or executed
        /// </summary>
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (value.Equals(_busy)) return;
                _busy = value;
                OnPropertyChanged();
            }
        }


    }
}
