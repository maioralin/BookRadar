using Books.OtherClasses;
using Books.Responses;
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
    public partial class MyLibrary : ContentPage
    {
        public MyLibrary()
        {
            InitializeComponent();
            BindingContext = new MyLibraryViewModel();
        }

        class MyLibraryViewModel : INotifyPropertyChanged
        {
            public MyLibraryViewModel()
            {
                ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
                MyLibraryAppearingCommand = new Command(MyLibraryAppearing);
                BookSelectedCommand = new Command((parameter) => BookSelected(parameter));
                NextPageCommand = new Command(NextPage);
                PreviousPageCommand = new Command(PreviousPage);
            }

            public ICommand BookSelectedCommand { get; }
            public ICommand MyLibraryAppearingCommand { get; }
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

                    BookSelectedCommand.Execute(selectedItem);

                    SelectedItem = null;
                }
            }

            private bool itemPressed = false;
            async void BookSelected(object item)
            {
                try
                {
                    if (!itemPressed)
                    {
                        itemPressed = true;
                        Book parameter = item as Book;
                        var resp = await RequestsHelper.MakeGetRequest<BookByIdResponse>($"books/getBookById/?BookId={parameter.Id}");
                        if (resp != null)
                        {
                            GlobalVars.VisitedBook = resp.Book;
                            GlobalVars.VisitedBook.Id = parameter.Id;
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

            async void MyLibraryAppearing()
            {
                var resp = await RequestsHelper.MakeGetRequest<UserBooksResponse>($"books/getBooksByUserId/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                if (resp.ErrorCode == 0)
                {
                    ObservableCollection<Book> requests = new ObservableCollection<Book>(resp.Books);
                    MyWishlist = requests;
                    if (resp.Books.Count > 0 && resp.TotalRows > PageSize)
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
                        var resp = await RequestsHelper.MakeGetRequest<UserBooksResponse>($"books/getBooksByUserId/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                        if (resp.ErrorCode == 0)
                        {
                            ObservableCollection<Book> requests = new ObservableCollection<Book>(resp.Books);
                            MyWishlist = requests;
                            PrevButtonVisible = true;
                            if (resp.TotalRows <= PageNumber * PageSize)
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
                        PageNumber -= -1;
                        var resp = await RequestsHelper.MakeGetRequest<UserBooksResponse>($"books/getBooksByUserId/?UserId={GlobalVars.UserId}&PageNumber={PageNumber}&PageSize={PageSize}");
                        if (resp.ErrorCode == 0)
                        {
                            ObservableCollection<Book> requests = new ObservableCollection<Book>(resp.Books);
                            MyWishlist = requests;
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

            ObservableCollection<Book> myWhishlist;
            public ObservableCollection<Book> MyWishlist
            {
                get { return myWhishlist; }
                set { myWhishlist = value; OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}