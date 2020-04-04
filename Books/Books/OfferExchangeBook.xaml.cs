using Books.OtherClasses;
using Books.Requests;
using Books.Responses;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class OfferExchangeBook : PopupPage
    {
        public OfferExchangeBook()
        {
            InitializeComponent();
            BindingContext = new OfferExchangeBookPageViewModel();
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
            GlobalVars.Master.Navigation.PopPopupAsync();
            return base.OnBackgroundClicked();
        }
    }

    class OfferExchangeBookPageViewModel : INotifyPropertyChanged
    {

        public OfferExchangeBookPageViewModel()
        {
            GlobalVars.ChatClient.Start().ContinueWith(task => {

                if (task.IsFaulted)
                {
                    ;
                }
                else
                {
                    GlobalVars.ChatClient.Connect(GlobalVars.FacebookDetails.LastName, GlobalVars.UserId);
                }
            });
            BookExchangePageAppearingCommand = new Command(BookExchangePageAppearing);
            BookSelectedCommand = new Command(BookSelected);
        }

        ObservableCollection<Book> myBooks;
        public ObservableCollection<Book> MyBooks
        {
            get { return myBooks; }
            set { myBooks = value; OnPropertyChanged(); }
        }

        bool permanent = false;
        public bool Permanent
        {
            get { return permanent; }
            set { permanent = value; OnPropertyChanged(); }
        }

        Book selectedItem;
        public Book SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged();

                if (selectedItem == null)
                    return;

                //SelectedItem = null;
            }
        }

        async void BookSelected(object item)
        {
            if (SelectedItem != null)
            {
                BookExchangeRequest request = new BookExchangeRequest
                {
                    AcceptedExchange = false,
                    OfferedBookId = SelectedItem.Id,
                    PermanentExchange = Permanent,
                    RequestId = GlobalVars.CurrentRequest.Id
                };
                var resp = await RequestsHelper.MakePostRequest<BookRequestResponse>("borrow/bookExchange/", request);
                if (resp != null && resp.ErrorCode == 0)
                {
                    string authors = SelectedItem.Authors;
                    GlobalVars.ChatClient.OfferExchangeBook(SelectedItem.Title, authors, GlobalVars.CurrentRequest.OwnerId, GlobalVars.CurrentRequest.Id, SelectedItem.Id);
                    await PopupNavigation.PopAsync();
                }
                else
                {
                    //error message
                }
            }
        }

        public ICommand BookExchangePageAppearingCommand { get; }
        public ICommand BookSelectedCommand { get; }


        async void BookExchangePageAppearing()
        {
            var resp = await RequestsHelper.MakeGetRequest<UserBooksResponse>($"books/getBooksByUserId/?UserId={GlobalVars.UserId}&PageNumber=1&PageSize=100");
            if (resp.ErrorCode == 0)
            {
                ObservableCollection<Book> requests = new ObservableCollection<Book>(resp.Books);
                MyBooks = requests;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}