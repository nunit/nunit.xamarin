using System;
using System.Collections.ObjectModel;
using NUnit.Framework.Interfaces;
using NUnit.Runner.ViewModel;
using Xamarin.Forms;

namespace NUnit.Runner
{
	class RunningViewModel : BaseViewModel, ITestListener
	{
		/// <summary>
		/// A list of tests that did not pass
		/// </summary>
		public ObservableCollection<ResultViewModel> Results { get; private set; } = new ObservableCollection<ResultViewModel>();
		public ResultViewModel CurrentTest { get; private set; } 

		public void TestFinished(ITestResult result)
		{
			if (!result.HasChildren) Results.Insert(0, new ResultViewModel(result));
			CurrentTest = null;
		}

		public void TestOutput(TestOutput output)
		{
			// not supported for running test cases
		}

		public void TestStarted(ITest test)
		{
			if (!test.IsSuite)
			{
				CurrentTest = new ResultViewModel(test);
				OnPropertyChanged(nameof(CurrentTest));
			}

		}
	}
}
