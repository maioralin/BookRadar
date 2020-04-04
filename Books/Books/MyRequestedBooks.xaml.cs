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
    public partial class MyRequestedBooks : ContentPage
    {
        public MyRequestedBooks()
        {
            InitializeComponent();
            BindingContext = new MyRequestedBooksViewModel();
        }

        class MyRequestedBooksViewModel : INotifyPropertyChanged
        {
            public MyRequestedBooksViewModel()
            {
                ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
                MyRequestedBooksAppearingCommand = new Command(MyRequestedBooksAppearing);
                RequestSelectedCommand = new Command((parameter) => RequestSelected(parameter));
                NextPageCommand = new Command(NextPage);
                PreviousPageCommand = new Command(PreviousPage);
                DeleteCommand = new Command(Delete);
            }

            public ICommand MyRequestedBooksAppearingCommand { get; }
            public ICommand RequestSelectedCommand { get; }
            public ICommand NextPageCommand { get; }
            public ICommand PreviousPageCommand { get; }
            public ICommand DeleteCommand { get; }

            public bool showAds;
            public bool ShowAds
            {
                get { return showAds; }
                set { showAds = value; OnPropertyChanged(); }
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

            RequestMinInfo selectedItem;
            public RequestMinInfo SelectedItem
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

                    RequestSelectedCommand.Execute(selectedItem);

                    SelectedItem = null;
                }
            }

            private bool itemPressed = false;
            async void RequestSelected(object item)
            {
                try
                {
                    if (!itemPressed)
                    {
                        itemPressed = true;
                        RequestMinInfo parameter = item as RequestMinInfo;
                        var resp = await RequestsHelper.MakeGetRequest<BookRequestResponse>($"borrow/getRequest/?RequestId={parameter.Id}");
                        if (resp != null)
                        {
                            GlobalVars.CurrentRequest = resp.BookRequest;
                            GlobalVars.CurrentRequest.Title = parameter.Title;
                            GlobalVars.CurrentRequest.Authors = parameter.Authors;
                            GlobalVars.CurrentRequest.Cover = parameter.SmallCover;
                            var page = (Page)Activator.CreateInstance(typeof(BookRequestPage));
                            await GlobalVars.Master.Navigation.PushAsync(page);
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    itemPressed = false;
                }
            }

            async void MyRequestedBooksAppearing()
            {
                var resp = await RequestsHelper.MakeGetRequest<RequestsResponse>($"borrow/getBooksRequestedFromMe/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                if (resp.ErrorCode == 0)
                {
                    ObservableCollection<RequestMinInfo> requests = new ObservableCollection<RequestMinInfo>(resp.Requests);
                    MyRequestedBooks = requests;
                    if (resp.Requests.Count > 0 && resp.Requests.FirstOrDefault().TotalRows > PageSize)
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
                        var resp = await RequestsHelper.MakeGetRequest<RequestsResponse>($"borrow/getBooksRequestedFromMe/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                        if (resp.ErrorCode == 0)
                        {
                            ObservableCollection<RequestMinInfo> requests = new ObservableCollection<RequestMinInfo>(resp.Requests);
                            MyRequestedBooks = requests;
                            PrevButtonVisible = true;
                            if (resp.Requests.FirstOrDefault().TotalRows <= PageNumber * PageSize)
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
                        var resp = await RequestsHelper.MakeGetRequest<RequestsResponse>($"borrow/getBooksRequestedFromMe/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                        if (resp.ErrorCode == 0)
                        {
                            ObservableCollection<RequestMinInfo> requests = new ObservableCollection<RequestMinInfo>(resp.Requests);
                            MyRequestedBooks = requests;
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

            async void Delete(object item)
            {
                RequestMinInfo obj = (RequestMinInfo)item;
                CloseChatRequest request = new CloseChatRequest
                {
                    RequestId = obj.Id
                };
                var resp = await RequestsHelper.MakePostRequest<DefaultResponse>($"borrow/closeRequest/", request);
                if (resp != null && resp.ErrorCode == 0)
                {
                    MyRequestedBooks.Remove(obj);
                }
            }

            ObservableCollection<RequestMinInfo> myRequestedBooks;
            public ObservableCollection<RequestMinInfo> MyRequestedBooks
            {
                get { return myRequestedBooks; }
                set { myRequestedBooks = value; OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}