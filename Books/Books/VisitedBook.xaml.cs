using Books.OtherClasses;
using Books.Requests;
using Books.Responses;
using Books.SqlClasses;
using Rg.Plugins.Popup.Extensions;
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
    public partial class VisitedBook : ContentPage
    {
        public VisitedBook()
        {
            InitializeComponent();
            BindingContext = new VisitedBookViewModel();
        }
    }

    class VisitedBookViewModel : INotifyPropertyChanged
    {
        public VisitedBookViewModel()
        {
            ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
            BookRequestCommand = new Command(BookRequest);
            RateBookCommand = new Command(RateBook);
            VisitedBookAppearingCommand = new Command(VisitedBookAppearing);
            ShowExtra = GlobalVars.VisitedBook.Owner.UserId != GlobalVars.UserId;
            ViewUserCommand = new Command(ViewUser);
            NextPageCommand = new Command(NextPage);
            PreviousPageCommand = new Command(PreviousPage);
            IsOwner = !ShowExtra;
            if(GlobalVars.VisitedBook.Donate)
            {
                Type = "Donate";
            }
            if (GlobalVars.VisitedBook.Giveaway)
            {
                Type = "Sell";
            }
            if (IsOwner)
            {
                Status = GlobalVars.VisitedBook.BorrowerId.HasValue ? $"Borrowed by {GlobalVars.VisitedBook.BorrowerName}" : "Not borrowed";
            }
        }

        public bool showAds;
        public bool ShowAds
        {
            get { return showAds; }
            set { showAds = value; OnPropertyChanged(); }
        }

        string authors = GlobalVars.VisitedBook.Authors;
        public string Authors
        {
            get { return authors; }
            set { authors = value; OnPropertyChanged(); }
        }

        string title = GlobalVars.VisitedBook.Title;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        string status = string.Empty;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }

        string owner = GlobalVars.VisitedBook.Owner.Name;
        public string Owner
        {
            get { return owner; }
            set { owner = value; OnPropertyChanged(); }
        }

        string cover = GlobalVars.VisitedBook.MediumCover;
        public string Cover
        {
            get { return cover; }
            set { cover = value; OnPropertyChanged(); }
        }

        double distance = GlobalVars.VisitedBook.Distance;
        public double Distance
        {
            get { return distance; }
            set { distance = value; OnPropertyChanged(); }
        }

        int totalReviews = 0;
        public int TotalReviews
        {
            get { return totalReviews; }
            set { totalReviews = value; OnPropertyChanged(); }
        }

        decimal average = 0;
        public decimal Average
        {
            get { return average; }
            set { average = value; OnPropertyChanged(); }
        }

        bool showReviewButton = false;
        public bool ShowReviewButton
        {
            get { return showReviewButton; }
            set { showReviewButton = value; OnPropertyChanged(); }
        }

        bool showExtra = false;
        public bool ShowExtra
        {
            get { return showExtra; }
            set { showExtra = value; OnPropertyChanged(); }
        }

        bool isOwner = false;
        public bool IsOwner
        {
            get { return isOwner; }
            set { isOwner = value; OnPropertyChanged(); }
        }

        int pageNumber = 1;
        public int PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; OnPropertyChanged(); }
        }

        int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; OnPropertyChanged(); }
        }

        bool prevButtonVisible = false;
        public bool PrevButtonVisible
        {
            get { return prevButtonVisible; }
            set { prevButtonVisible = value; OnPropertyChanged(); }
        }

        bool nextButtonVisible = false;
        public bool NextButtonVisible
        {
            get { return nextButtonVisible; }
            set { nextButtonVisible = value; OnPropertyChanged(); }
        }

        string type = "Borrow";
        public string Type
        {
            get { return type; }
            set { type = value; OnPropertyChanged(); }
        }

        string description = GlobalVars.VisitedBook.Description;
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        ObservableCollection<BookReviewSQL> bookReviews;
        public ObservableCollection<BookReviewSQL> BookReviews
        {
            get { return bookReviews; }
            set { bookReviews = value; OnPropertyChanged(); }
        }

        BookReviewSQL selectedItem;
        public BookReviewSQL SelectedItem
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

                SelectedItem = null;
            }
        }

        public ICommand BookRequestCommand { get; }
        public ICommand RateBookCommand { get; }
        public ICommand VisitedBookAppearingCommand { get; }
        public ICommand ViewUserCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        async void ViewUser()
        {
            var page = (Page)Activator.CreateInstance(typeof(UserDetailsPage), GlobalVars.VisitedBook.Owner.UserId);
            await GlobalVars.Master.Navigation.PushAsync(page);
        }

        private bool itemPressed = false;
        async void BookRequest()
        {
            try
            {
                if (!itemPressed)
                {
                    itemPressed = true;
                    RequestBookRequest request = new RequestBookRequest
                    {
                        UserId = GlobalVars.UserId,
                        OwnerId = GlobalVars.VisitedBook.Owner.UserId,
                        BookId = GlobalVars.VisitedBook.Id,
                        Wanted = true
                    };
                    var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/RequestBook/", request);
                    if (resp != null)
                    {
                        if (resp.ErrorCode == 0)
                        {
                            string message = $"{GlobalVars.FacebookDetails.LastName} has requested the book {GlobalVars.VisitedBook.Title}";
                            string target = GlobalVars.VisitedBook.Owner.UserId.ToString();
                            Dictionary<string, string> param = new Dictionary<string, string>();
                            param.Add("action", "requests");
                            NotificationRequest notificationRequest = new NotificationRequest
                            {
                                Title = "New book request",
                                Message = message,
                                Target = target,
                                Params = param
                            };
                            await RequestsHelper.MakePostRequest<DefaultResponse>("notificationSender/send", notificationRequest);
                            await App.Current.MainPage.DisplayAlert("Book requested", $"You have succesfully requested the book from {GlobalVars.VisitedBook.Owner.Name}.\nGo to 'My requests to chat with them!'", "OK");
                        }
                        else
                        {
                            if (resp.ErrorCode == 1)
                            {
                                await App.Current.MainPage.DisplayAlert("Book request limit", resp.ErrorMessage, "OK");
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                itemPressed = false;
            }
        }

        async void VisitedBookAppearing()
        {
            var resp = await RequestsHelper.MakeGetRequest<BooleanResponse>($"reviews/HasReviewAdded/?userId={GlobalVars.UserId}&isbn={GlobalVars.VisitedBook.ISBN}");
            ShowReviewButton = !resp.Result;
            var resp2 = await RequestsHelper.MakeGetRequest<GetBookReviewsResponse>($"reviews/getBookReviews/?isbn={GlobalVars.VisitedBook.ISBN}&pageNumber={PageNumber}&pageSize={PageSize}");
            if(resp2.ErrorCode == 0)
            {
                Average = resp2.Average;
                TotalReviews = resp2.Total;
                ObservableCollection<BookReviewSQL> bookReviews = new ObservableCollection<BookReviewSQL>(resp2.Reviews);
                BookReviews = bookReviews;
                if (TotalReviews > 0 && TotalReviews > PageSize)
                {
                    NextButtonVisible = true;
                    PrevButtonVisible = false;
                }
            }
        }

        private bool nextPagePressed = false;
        async void NextPage()
        {
            try
            {
                if (!nextPagePressed)
                {
                    nextPagePressed = true;
                    PageNumber += 1;
                    var resp = await RequestsHelper.MakeGetRequest<GetBookReviewsResponse>($"reviews/getBookReviews/?isbn={GlobalVars.VisitedBook.ISBN}&pageNumber={PageNumber}&pageSize={PageSize}");
                    if (resp.ErrorCode == 0)
                    {
                        ObservableCollection<BookReviewSQL> bookReviews = new ObservableCollection<BookReviewSQL>(resp.Reviews);
                        BookReviews = bookReviews;
                        PrevButtonVisible = true;
                        if (TotalReviews <= PageNumber * PageSize)
                        {
                            NextButtonVisible = false;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                nextPagePressed = false;
            }
        }

        private bool prevPagePressed = false;
        async void PreviousPage()
        {
            try
            {
                if (!prevPagePressed)
                {
                    prevPagePressed = true;
                    PageNumber -= 1;
                    var resp = await RequestsHelper.MakeGetRequest<GetBookReviewsResponse>($"reviews/getBookReviews/?isbn={GlobalVars.VisitedBook.ISBN}&pageNumber={PageNumber}&pageSize={PageSize}");
                    if (resp.ErrorCode == 0)
                    {
                        ObservableCollection<BookReviewSQL> bookReviews = new ObservableCollection<BookReviewSQL>(resp.Reviews);
                        BookReviews = bookReviews;
                        NextButtonVisible = true;
                        if (PageNumber == 1)
                        {
                            PrevButtonVisible = false;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                prevPagePressed = false;
            }
        }

        async void RateBook()
        {
            var page = new RateBookPage();
            await GlobalVars.Master.Navigation.PushPopupAsync(page, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}