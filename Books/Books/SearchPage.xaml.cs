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
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = new SearchPageViewModel();
        }
    }

    class SearchPageViewModel : INotifyPropertyChanged
    {
        public SearchPageViewModel()
        {
            OpenAddBookPage = new Command(OpenAddBook);
            SearchPageAppearingCommand = new Command(SearchPageAppearing);
            SearchCommand = new Command(Search);
        }

        bool errorMessageDisplayed = GlobalVars.Books == null || GlobalVars.Books.Count == 0;
        public bool ErrorMessageDisplayed
        {
            get { return errorMessageDisplayed; }
            set { errorMessageDisplayed = value; OnPropertyChanged(); }
        }

        bool filterVisible = false;
        public bool FilterVisible
        {
            get { return filterVisible; }
            set { filterVisible = value; OnPropertyChanged(); }
        }

        int distance = 5;
        public int Distance
        {
            get { return distance; }
            set { distance = value; OnPropertyChanged(); }
        }

        ObservableCollection<BookMinInfo> bookList;
        public ObservableCollection<BookMinInfo> BookList
        {
            get { return bookList; }
            set { bookList = value; OnPropertyChanged(); }
        }

        public ICommand OpenAddBookPage { get; }
        public ICommand SearchPageAppearingCommand { get; }
        public ICommand SearchCommand { get; }

        /*async void BookSelected(object item)
        {
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
        }*/

        async void Search()
        {
            var page = (Page)Activator.CreateInstance(typeof(ChoosePage), Distance);
            await GlobalVars.Master.Navigation.PushAsync(page);
        }

        void OpenAddBook()
        {
            var page = (Page)Activator.CreateInstance(typeof(AddBookPage));
            GlobalVars.Master.Navigation.PushAsync(page);
        }

        async void SearchPageAppearing()
        {
            var resp = await RequestsHelper.MakeGetRequest<UserBooksResponse>($"books/getBooksByUserId/?UserId={GlobalVars.UserId}&PageNumber=1&PageSize=100");
            if (resp.ErrorCode == 0)
            {
                GlobalVars.Books = resp.Books;
                if (GlobalVars.Books.Count > 0)
                {
                    ErrorMessageDisplayed = false;
                    FilterVisible = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}