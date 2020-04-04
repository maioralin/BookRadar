using Books.OtherClasses;
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
    public partial class RateBookPage : PopupPage
    {
        public RateBookPage()
        {
            InitializeComponent();
            BindingContext = new RateBookPageViewModel();
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
        private async void Submit_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!clicked)
                {
                    clicked = true;
                    if (!string.IsNullOrEmpty(labelResult.Text))
                    {
                        int rating = int.Parse(labelResult.Text);
                        string comment = entryReview.Text;
                        AddBookReviewRequest request = new AddBookReviewRequest
                        {
                            Comment = comment,
                            Rating = rating,
                            ReviewerId = GlobalVars.UserId,
                            ISBN = GlobalVars.VisitedBook.ISBN
                        };
                        var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("reviews/addBookReview/", request);
                        if (resp != null && resp.ErrorCode == 0)
                        {
                            await App.Current.MainPage.DisplayAlert("Review added", $"You have succesfully added a new review!", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Please select a rating", "OK");
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

    class RateBookPageViewModel : INotifyPropertyChanged
    {

        public RateBookPageViewModel()
        {
        }

        string bookName = GlobalVars.VisitedBook.Title;
        public string BookName
        {
            get { return bookName; }
            set { bookName = value; OnPropertyChanged(); }
        }

        string authors = GlobalVars.VisitedBook.Authors;
        public string Authors
        {
            get { return authors; }
            set { authors = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}