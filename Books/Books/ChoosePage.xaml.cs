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
    public partial class ChoosePage : ContentPage
    {
        public static int SearchDistance;
        public ChoosePage(int searchDistance)
        {
            SearchDistance = searchDistance;
            InitializeComponent();
            BindingContext = new ChoosePageViewModel();
        }
    }

    class ChoosePageViewModel : INotifyPropertyChanged
    {
        public ChoosePageViewModel()
        {
            ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
            ChoosePageAppearingCommand = new Command(ChoosePageAppearing);
            SwipedLeftCommand = new Command((parameter) => SwipedLeft(parameter));
            SwipedRightCommand = new Command((parameter) => SwipedRight(parameter));
            CardClickedCommand = new Command((parameter) => CardClicked(parameter));
            NextPageCommand = new Command(NextPage);
            PreviousPageCommand = new Command(PreviousPage);
            BookList = new ObservableCollection<BookMinInfo>();
        }

        public ICommand ChoosePageAppearingCommand { get; }
        public ICommand CardClickedCommand { get; }
        public ICommand SwipedLeftCommand { get; }
        public ICommand SwipedRightCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public bool showAds;
        public bool ShowAds
        {
            get { return showAds; }
            set { showAds = value; OnPropertyChanged(); }
        }

        ObservableCollection<BookMinInfo> bookList;
        public ObservableCollection<BookMinInfo> BookList
        {
            get { return bookList; }
            set { bookList = value; OnPropertyChanged(); }
        }

        BookMinInfo topBook;
        public BookMinInfo TopBook
        {
            get { return topBook; }
            set { topBook = value; OnPropertyChanged(); }
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

        BookMinInfo selectedItem;
        public BookMinInfo SelectedItem
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

                CardClickedCommand.Execute(selectedItem);

                SelectedItem = null;
            }
        }

        private bool itemPressed = false;
        async void CardClicked(object item)
        {
            try
            {
                if (!itemPressed)
                {
                    itemPressed = true;
                    BookMinInfo parameter = item as BookMinInfo;
                    var resp = await RequestsHelper.MakeGetRequest<BookByIdResponse>($"books/getBookById/?BookId={parameter.BookId}");
                    if (resp != null)
                    {
                        GlobalVars.VisitedBook = resp.Book;
                        GlobalVars.VisitedBook.Id = parameter.BookId;
                        GlobalVars.VisitedBook.Distance = parameter.Distance;
                        var page = (Page)Activator.CreateInstance(typeof(VisitedBook));
                        await GlobalVars.Master.Navigation.PushAsync(page);
                    }
                }
            }
            catch { }
            finally
            {
                itemPressed = false;
            }
        }

        async void ChoosePageAppearing()
        {
            int distance = ChoosePage.SearchDistance;
            if(GlobalVars.GeoLocation == null)
            {
                var ipResponse = await RequestsHelper.MakePostRequest<IpAPIResponse>("json", null, "http://ip-api.com/");
                GlobalVars.GeoLocation = new LocationDetails
                {
                    Latitude = (decimal)ipResponse.Lat,
                    Longitude = (decimal)ipResponse.Lon
                };
                UpdateUserLocationRequest request = new UpdateUserLocationRequest
                {
                    UserId = GlobalVars.UserId,
                    Latitude = ipResponse.Lat,
                    Longitude = ipResponse.Lon
                };
                await RequestsHelper.MakePostRequest<string>("facebook/updateUserLocation", request);
            }
            var resp = await RequestsHelper.MakeGetRequest<BookSearchByDistanceResponse>($"books/getBooksByDistance/?UserId={GlobalVars.UserId}&Latitude={GlobalVars.GeoLocation.Latitude}&Longitude={GlobalVars.GeoLocation.Longitude}&MaxDistance={distance}&PageNumber={PageNumber}&PageSize={PageSize}");
            if (resp.Books != null && resp.Books.Count > 0)
            {
                ObservableCollection<BookMinInfo> books = new ObservableCollection<BookMinInfo>(resp.Books);
                BookList = books;
                if (resp.Books.FirstOrDefault().TotalRows > PageSize)
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
                    PageNumber += 1;
                    var resp = await RequestsHelper.MakeGetRequest<BookSearchByDistanceResponse>($"books/getBooksByDistance/?UserId={GlobalVars.UserId}&Latitude={GlobalVars.GeoLocation.Latitude}&Longitude={GlobalVars.GeoLocation.Longitude}&MaxDistance={ChoosePage.SearchDistance}&PageNumber={PageNumber}&PageSize={PageSize}");
                    if (resp.Books != null && resp.Books.Count > 0)
                    {
                        ObservableCollection<BookMinInfo> books = new ObservableCollection<BookMinInfo>(resp.Books);
                        BookList = books;
                        PrevButtonVisible = true;
                        if (resp.Books.FirstOrDefault().TotalRows <= PageNumber * PageSize)
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
                    PageNumber -= 1;
                    var resp = await RequestsHelper.MakeGetRequest<BookSearchByDistanceResponse>($"books/getBooksByDistance/?UserId={GlobalVars.UserId}&Latitude={GlobalVars.GeoLocation.Latitude}&Longitude={GlobalVars.GeoLocation.Longitude}&MaxDistance={ChoosePage.SearchDistance}&PageNumber={PageNumber}&PageSize={PageSize}");
                    if (resp.Books != null && resp.Books.Count > 0)
                    {
                        ObservableCollection<BookMinInfo> books = new ObservableCollection<BookMinInfo>(resp.Books);
                        BookList = books;
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
                prevPageClicked = false;
            }
        }

        async void SwipedLeft(object item)
        {
            int number = (int)item;
            Requests.RequestBookRequest request = new RequestBookRequest
            {
                UserId = GlobalVars.UserId,
                OwnerId = BookList[number].OwnerId,
                BookId = BookList[number].BookId,
                Wanted = false
            };
            await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/RequestBook/", request);
            if (number + 1 == BookList.Count)
            {
                int pageNumber = 1;
                int distance = ChoosePage.SearchDistance;
                int pageSize = 10;
                var resp2 = await RequestsHelper.MakeGetRequest<BookSearchByDistanceResponse>($"books/getBooksByDistance/?UserId={GlobalVars.UserId}&Latitude={GlobalVars.GeoLocation.Latitude}&Longitude={GlobalVars.GeoLocation.Longitude}&MaxDistance={distance}&PageNumber={pageNumber}&PageSize={pageSize}");
                if (resp2.Books != null && resp2.Books.Count > 0)
                {
                    ObservableCollection<BookMinInfo> books = new ObservableCollection<BookMinInfo>(resp2.Books);
                    foreach (var book in books)
                    {
                        BookList.Add(book);
                    }
                }
            }
        }

        async void SwipedRight(object item)
        {
            int number = (int)item;
            Requests.RequestBookRequest request = new RequestBookRequest
            {
                UserId = GlobalVars.UserId,
                OwnerId = BookList[number].OwnerId,
                BookId = BookList[number].BookId,
                Wanted = true
            };
            await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/RequestBook/", request);
            string message = $"{GlobalVars.FacebookDetails.LastName} has requested the book {BookList[number].Title}";
            string target = BookList[number].OwnerId.ToString();
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
            if (number + 1 == BookList.Count)
            {
                int pageNumber = 1;
                int distance = ChoosePage.SearchDistance;
                int pageSize = 10;
                var resp = await RequestsHelper.MakeGetRequest<BookSearchByDistanceResponse>($"books/getBooksByDistance/?UserId={GlobalVars.UserId}&Latitude={GlobalVars.GeoLocation.Latitude}&Longitude={GlobalVars.GeoLocation.Longitude}&MaxDistance={distance}&PageNumber={pageNumber}&PageSize={pageSize}");
                if (resp.Books != null && resp.Books.Count > 0)
                {
                    ObservableCollection<BookMinInfo> books = new ObservableCollection<BookMinInfo>(resp.Books);
                    foreach(var book in books)
                    {
                        BookList.Add(book);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}