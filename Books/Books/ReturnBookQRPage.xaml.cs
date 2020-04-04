using Books.Requests;
using Books.Responses;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Books
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReturnBookQRPage : PopupPage
    {
        public ReturnBookQRPage()
        {
            InitializeComponent();
            BindingContext = new ReturnBookQRPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // Method for animation child in PopupPage
        // Invoced after custom animation end
        protected override Task OnAppearingAnimationEnd()
        {
            return Content.FadeTo(1);
        }

        // Method for animation child in PopupPage
        // Invoked before custom animation begin
        protected override Task OnDisappearingAnimationBegin()
        {
            return Content.FadeTo(1);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent hide popup
            //return base.OnBackButtonPressed();
            return true;
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return default value - CloseWhenBackgroundIsClicked
            Task.Run(async () => await RequestsHelper.MakePostRequest<GuidResponse>("borrow/deleteReturn/", new AcceptBookRequest { BookOfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value }));
            return base.OnBackgroundClicked();
        }
    }

    class ReturnBookQRPageViewModel : INotifyPropertyChanged
    {

        public ReturnBookQRPageViewModel()
        {
            CloseCommand = new Command(Close);
        }

        public ICommand CloseCommand { get; }

        async void Close(object item)
        {
            await RequestsHelper.MakePostRequest<GuidResponse>("borrow/deleteReturn/", new AcceptBookRequest { BookOfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value });
            await PopupNavigation.PopAsync();
        }

        string qrValue = GlobalVars.CurrentRequest.BookOffer.Id.ToString();
        public string QrValue
        {
            get { return qrValue; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}