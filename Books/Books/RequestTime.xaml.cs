using Books.OtherClasses;
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
    public partial class RequestTime : PopupPage
    {
        public RequestTime()
        {
            InitializeComponent();
            BindingContext = new RequestTimePageViewModel();
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
            return base.OnBackgroundClicked();
        }
    }

    class RequestTimePageViewModel : INotifyPropertyChanged
    {
        public RequestTimePageViewModel()
        {
            DateSelectedCommand = new Command(DateSelected);
        }

        DateTime proposedDate = GlobalVars.CurrentRequest.BookOffer.ProposedReturnDate.Value;
        public DateTime ProposedDate
        {
            get { return proposedDate; }
            set { proposedDate = value; OnPropertyChanged(); }
        }

        public ICommand DateSelectedCommand { get; }

        private bool dateSelectedClicked = false;
        async void DateSelected(object item)
        {
            try
            {
                if (!dateSelectedClicked)
                {
                    dateSelectedClicked = true;
                    ExtendBookRequest request = new ExtendBookRequest
                    {
                        OfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value,
                        ProposedDate = ProposedDate
                    };
                    var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/requestTime/", request);
                    if (resp != null && resp.ErrorCode == 0)
                    {
                        GlobalVars.CurrentRequest.BookOffer.ExtendedDate = ProposedDate;
                        string message = $"{GlobalVars.CurrentRequest.RequesterName} has requested extra time to return the book";
                        string target = GlobalVars.CurrentRequest.OwnerId.ToString();
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("action", "messages");
                        param.Add("requestId", GlobalVars.CurrentRequest.Id.ToString());
                        NotificationRequest notificationRequest = new NotificationRequest
                        {
                            Title = "Social Books",
                            Message = message,
                            Target = target,
                            Params = param
                        };
                        await RequestsHelper.MakePostRequest<DefaultResponse>("notificationSender/send", notificationRequest);
                        GlobalVars.ChatClient.RequestTime(GlobalVars.CurrentRequest.Id, GlobalVars.CurrentRequest.OwnerId, ProposedDate);
                        await PopupNavigation.PopAsync();
                    }
                    else
                    {
                        //error message
                    }
                }
            }
            catch { }
            finally
            {
                dateSelectedClicked = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}