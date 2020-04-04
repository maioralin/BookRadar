using Books.Requests;
using Books.Responses;
using Books.SqlClasses;
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
    public partial class UserDetailsPage : ContentPage
    {
        public UserDetailsPage(Guid userId)
        {
            InitializeComponent();
            BindingContext = new UserDetailsViewModel(userId);
        }
    }

    class UserDetailsViewModel : INotifyPropertyChanged
    {
        public UserDetailsViewModel(Guid userId)
        {
            ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
            UserDetailsAppearingCommand = new Command(UserDetailsAppearing);
            ViewReviewsCommand = new Command(ViewReviews);
            NextPageCommand = new Command(NextPage);
            PreviousPageCommand = new Command(PreviousPage);
            this.userId = userId;
        }

        public bool showAds;
        public bool ShowAds
        {
            get { return showAds; }
            set { showAds = value; OnPropertyChanged(); }
        }

        string profilePicture = string.Empty;
        public string ProfilePicture
        {
            get { return profilePicture; }
            set { profilePicture = value; OnPropertyChanged(); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        string bookCount = string.Empty;
        public string BookCount
        {
            get { return bookCount; }
            set { bookCount = value; OnPropertyChanged(); }
        }

        string bookQuality = string.Empty;
        public string BookQuality
        {
            get { return bookQuality; }
            set { bookQuality = value; OnPropertyChanged(); }
        }

        string timeQuality = string.Empty;
        public string TimeQuality
        {
            get { return timeQuality; }
            set { timeQuality = value; OnPropertyChanged(); }
        }

        bool displayReviewsButton = true;
        public bool DisplayReviewsButton
        {
            get { return displayReviewsButton; }
            set { displayReviewsButton = value; OnPropertyChanged(); }
        }

        bool displayReviews = false;
        public bool DisplayReviews
        {
            get { return displayReviews; }
            set { displayReviews = value; OnPropertyChanged(); }
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

        ObservableCollection<UserReview> reviews;
        public ObservableCollection<UserReview> Reviews
        {
            get { return reviews; }
            set { reviews = value; OnPropertyChanged(); }
        }

        private Guid userId;
        private int pageNumber = 1;
        private int pageSize = 10;

        public ICommand UserDetailsAppearingCommand { get; }
        public ICommand ViewReviewsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        async void ViewReviews()
        {
            DisplayReviews = true;
            DisplayReviewsButton = false;
            var resp = await RequestsHelper.MakeGetRequest<UserReviewsResponse>($"reviews/getUserReviews/?UserId={userId}&pageNumber={pageNumber}&pageSize={pageSize}");
            if (resp.ErrorCode == 0)
            {
                ObservableCollection<UserReview> reviews = new ObservableCollection<UserReview>(resp.Reviews);
                Reviews = reviews;
                if (reviews.Count > 0 && reviews.FirstOrDefault().TotalRows > pageSize)
                {
                    NextButtonVisible = true;
                    PrevButtonVisible = false;
                }
            }
        }

        private bool nextPageClicked = false;
        async void NextPage()
        {
            try
            {
                if (!nextPageClicked)
                {
                    nextPageClicked = true;
                    pageNumber += 1;
                    var resp = await RequestsHelper.MakeGetRequest<UserReviewsResponse>($"reviews/getUserReviews/?UserId={userId}&pageNumber={pageNumber}&pageSize={pageSize}");
                    if (resp.ErrorCode == 0)
                    {
                        ObservableCollection<UserReview> reviews = new ObservableCollection<UserReview>(resp.Reviews);
                        Reviews = reviews;
                        PrevButtonVisible = true;
                        if (reviews.FirstOrDefault().TotalRows <= pageNumber * pageSize)
                        {
                            NextButtonVisible = false;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                nextPageClicked = false;
            }
        }

        private bool prevPageClicked = false;
        async void PreviousPage()
        {
            try
            {
                if (!prevPageClicked)
                {
                    prevPageClicked = true;
                    pageNumber -= 1;
                    var resp = await RequestsHelper.MakeGetRequest<UserReviewsResponse>($"reviews/getUserReviews/?UserId={userId}&pageNumber={pageNumber}&pageSize={pageSize}");
                    if (resp.ErrorCode == 0)
                    {
                        ObservableCollection<UserReview> reviews = new ObservableCollection<UserReview>(resp.Reviews);
                        Reviews = reviews;
                        NextButtonVisible = true;
                        if (pageNumber == 1)
                        {
                            PrevButtonVisible = false;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                prevPageClicked = false;
            }
        }

        async void UserDetailsAppearing()
        {
            var resp = await RequestsHelper.MakeGetRequest<UserResponse>($"facebook/getDetails/?userId={userId}");
            if(resp != null && resp.ErrorCode == 0)
            {
                Name = resp.User.Name;
                ProfilePicture = resp.User.Picture;
                BookCount = resp.User.BookCount.ToString();
                BookQuality = resp.User.BookQuality.ToString();
                TimeQuality = resp.User.TimeQuality.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}