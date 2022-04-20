using NUnit.Runner.Messages;
using NUnit.Runner.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NUnit.Runner.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestExplorerView : ContentPage
    {
        TestExplorerViewModel _model;

        internal TestExplorerView(TestExplorerViewModel model)
        {
            _model = model;
            _model.Navigation = Navigation;
            BindingContext = _model;
            InitializeComponent();

            MessagingCenter.Subscribe<ErrorMessage>(this, ErrorMessage.Name, error =>
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", error.Message, "OK"));
            });
        }

        /// <summary>
        /// Called when the view is appearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _model.LoadTestsAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}