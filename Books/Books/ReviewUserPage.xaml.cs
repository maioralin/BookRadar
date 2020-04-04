using Books.Requests;
using Books.Responses;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Books
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewUserPage : PopupPage
    {
        public ReviewUserPage()
        {
            InitializeComponent();
            BindingContext = new ReviewUserPageViewModel();
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

        private bool clicked = false;
        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!clicked)
                {
                    clicked = true;
                    string errorMessage = string.Empty;
                    if (string.IsNullOrEmpty(labelResultAspect.Text))
                    {
                        errorMessage += "Please select a 'Book aspect' rating\n";
                    }
                    if (string.IsNullOrEmpty(labelResultTime.Text))
                    {
                        errorMessage += "Please select a 'Return time' rating\n";
                    }
                    if (errorMessage == string.Empty)
                    {
                        int bookAspect = int.Parse(labelResultAspect.Text);
                        int returnTime = int.Parse(labelResultTime.Text);
                        string comment = entryReview.Text;
                        AddUserReviewRequest request = new AddUserReviewRequest
                        {
                            BookAspect = bookAspect,
                            ReturnTime = returnTime,
                            Comment = comment,
                            RequestId = GlobalVars.CurrentRequest.Id,
                            UserId = GlobalVars.CurrentRequest.RequesterId,
                            ReviewerId = GlobalVars.CurrentRequest.OwnerId
                        };
                        var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("reviews/addUserReview/", request);
                        if (resp != null && resp.ErrorCode == 0)
                        {
                            await App.Current.MainPage.DisplayAlert("Review added", $"You have succesfully added a review!", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", errorMessage, "OK");
                    }
                }
            }
            catch { }
            finally
            {
                clicked = false;
            }
        }
    }

    class ReviewUserPageViewModel : INotifyPropertyChanged
    {

        public ReviewUserPageViewModel()
        {
        }

        string bookName = GlobalVars.CurrentRequest.Title;
        public string BookName
        {
            get { return bookName; }
        }

        string name = GlobalVars.CurrentRequest.RequesterName;
        public string Name
        {
            get { return name; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}