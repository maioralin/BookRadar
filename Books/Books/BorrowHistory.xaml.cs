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
    public partial class BorrowHistory : ContentPage
    {
        public BorrowHistory()
        {
            InitializeComponent();
            BindingContext = new BorrowHistoryViewModel();
        }

        class BorrowHistoryViewModel : INotifyPropertyChanged
        {
            public BorrowHistoryViewModel()
            {
                ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
                BorrowHistoryAppearingCommand = new Command(BorrowHistoryAppearing);
                ItemSelectedCommand = new Command((parameter) => ItemSelected(parameter));
                NextPageCommand = new Command(NextPage);
                PreviousPageCommand = new Command(PreviousPage);
            }

            public ICommand BorrowHistoryAppearingCommand { get; }
            public ICommand ItemSelectedCommand { get; }
            public ICommand NextPageCommand { get; }
            public ICommand PreviousPageCommand { get; }

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

            BorrowHistorySQL selectedItem;
            public BorrowHistorySQL SelectedItem
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

                    ItemSelectedCommand.Execute(selectedItem);

                    SelectedItem = null;
                }
            }

            private bool itemPressed = false;
            async void ItemSelected(object item)
            {
                try
                {
                    if (!itemPressed)
                    {
                        itemPressed = true;
                        BorrowHistorySQL parameter = item as BorrowHistorySQL;
                        var resp = await RequestsHelper.MakeGetRequest<BookByIdResponse>($"books/getBookById/?BookId={parameter.BookId}");
                        if (resp != null)
                        {
                            GlobalVars.VisitedBook = resp.Book;
                            GlobalVars.VisitedBook.Id = parameter.BookId;
                            GlobalVars.VisitedBook.Distance = -1;
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

            ObservableCollection<BorrowHistorySQL> myBorrowHistory;
            public ObservableCollection<BorrowHistorySQL> MyBorrowHistory
            {
                get { return myBorrowHistory; }
                set { myBorrowHistory = value; OnPropertyChanged(); }
            }

            async void BorrowHistoryAppearing()
            {
                var resp = await RequestsHelper.MakeGetRequest<BorrowHistoryResponse>($"books/getBorrowHistory/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                if (resp.ErrorCode == 0)
                {
                    ObservableCollection<BorrowHistorySQL> myBorrowedBooks = new ObservableCollection<BorrowHistorySQL>(resp.BorrowedBooks);
                    MyBorrowHistory = myBorrowedBooks;
                    if (resp.BorrowedBooks.Count > 0 && resp.BorrowedBooks.FirstOrDefault().TotalRows > PageSize)
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
                        var resp = await RequestsHelper.MakeGetRequest<BorrowHistoryResponse>($"books/getBorrowHistory/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                        if (resp.ErrorCode == 0)
                        {
                            ObservableCollection<BorrowHistorySQL> myBorrowedBooks = new ObservableCollection<BorrowHistorySQL>(resp.BorrowedBooks);
                            MyBorrowHistory = myBorrowedBooks;
                            PrevButtonVisible = true;
                            if (resp.BorrowedBooks.FirstOrDefault().TotalRows <= PageNumber * PageSize)
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
                        var resp = await RequestsHelper.MakeGetRequest<BorrowHistoryResponse>($"books/getBorrowHistory/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                        if (resp.ErrorCode == 0)
                        {
                            ObservableCollection<BorrowHistorySQL> myBorrowedBooks = new ObservableCollection<BorrowHistorySQL>(resp.BorrowedBooks);
                            MyBorrowHistory = myBorrowedBooks;
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

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}