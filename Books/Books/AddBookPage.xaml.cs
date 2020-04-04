using Books.OtherClasses;
using Books.Requests;
using Books.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Books
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddBookPage : ContentPage
    {
        public AddBookPage()
        {
            BindingContext = new AddBookViewModel();
            InitializeComponent();
        }
    }

    class AddBookViewModel : INotifyPropertyChanged
    {
        Book currentBook;

        ObservableCollection<ListBook> bookList;
        public ObservableCollection<ListBook> BookList
        {
            get { return bookList; }
            set { bookList = value; OnPropertyChanged(); }
        }

        public class OptionsClass
        {
            public string Label { get; set; }
            public string Value { get; set; }
        }

        ListBook selectedItem;
        public ListBook SelectedItem
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

        public bool showAds;
        public bool ShowAds
        {
            get { return showAds; }
            set { showAds = value; OnPropertyChanged(); }
        }

        int pageNumber = 0;
        public int PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; OnPropertyChanged(); }
        }

        List<OptionsClass> options;
        public List<OptionsClass> Options
        {
            get { return options; }
            set { options = value; OnPropertyChanged(); }
        }

        OptionsClass bookOptions;
        public OptionsClass BookOptions
        {
            get { return bookOptions; }
            set { bookOptions = value; OnPropertyChanged(); }
        }

        OptionsClass ownOptions;
        public OptionsClass OwnOptions
        {
            get { return ownOptions; }
            set { ownOptions = value; OnPropertyChanged(); }
        }

        int pageSize = 0;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; OnPropertyChanged(); }
        }

        string[] isbns;
        public string[] Isbns
        {
            get { return isbns; }
            set { isbns = value; OnPropertyChanged(); }
        }

        public AddBookViewModel()
        {
            AddByISBNCommand = new Command(AddByISBN);
            SearchByISBNCommand = new Command(SearchByISBN);
            AddByISBNMakeRequest = new Command(AddByISBNRequest);
            //SearchByQueryCommand = new Command(SearchByQuery);
            //AddByQueryCommand = new Command(AddByQuery);
            //NextPageCommand = new Command(NextPage);
            //PreviousPageCommand = new Command(PreviousPage);
            AddOwnBookCommand = new Command(AddOwnBook);
            AddOwnSubmit = new Command(AddOwn);
            ScanBookCommand = new Command(ScanBook);
            BookSelectedCommand = new Command((parameter) => BookSelected(parameter));
            Options = new List<OptionsClass>();
            Options.Add(new OptionsClass { Label = "Borrow", Value = "Value" });
            Options.Add(new OptionsClass { Label = "Sell", Value = "Sell" });
            Options.Add(new OptionsClass { Label = "Donate", Value = "Donate" });
            BookOptions = Options.FirstOrDefault();
            OwnOptions = Options.FirstOrDefault();
            ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
        }

        bool selectionDisplay = true;
        public bool SelectionDisplay
        {
            get { return selectionDisplay; }
            set { selectionDisplay = value; OnPropertyChanged(); }
        }

        bool addByISBNDisplay = false;
        public bool AddByISBNDisplay
        {
            get { return addByISBNDisplay; }
            set { addByISBNDisplay = value; OnPropertyChanged(); }
        }

        /*bool addByQueryDisplay = false;
        public bool AddByQueryDisplay
        {
            get { return addByQueryDisplay; }
            set { addByQueryDisplay = value; OnPropertyChanged(); }
        }*/

        bool bookListDisplay = false;
        public bool BookListDisplay
        {
            get { return bookListDisplay; }
            set { bookListDisplay = value; OnPropertyChanged(); }
        }

        bool addOwnBookShow = false;
        public bool AddOwnBookShow
        {
            get { return addOwnBookShow; }
            set { addOwnBookShow = value; OnPropertyChanged(); }
        }

        /*bool prevButtonVisible = false;
        public bool PrevButtonVisible
        {
            get { return prevButtonVisible; }
            set { prevButtonVisible = value; OnPropertyChanged(); }
        }*/

        bool nextButtonVisible = false;
        public bool NextButtonVisible
        {
            get { return nextButtonVisible; }
            set { nextButtonVisible = value; OnPropertyChanged(); }
        }

        bool addByISBNShow = false;
        public bool AddByISBNShow
        {
            get { return addByISBNShow; }
            set { addByISBNShow = value; OnPropertyChanged(); }
        }

        /*string titleQuery = string.Empty;
        public string TitleQuery
        {
            get { return titleQuery; }
            set { titleQuery = value; OnPropertyChanged(); }
        }

        string authorQuery = string.Empty;
        public string AuthorQuery
        {
            get { return authorQuery; }
            set { authorQuery = value; OnPropertyChanged(); }
        }*/

        string isbn = string.Empty;
        public string ISBN
        {
            get { return isbn; }
            set { isbn = value; OnPropertyChanged(); }
        }

        string bookName = string.Empty;
        public string BookName
        {
            get { return bookName; }
            set { bookName = value; OnPropertyChanged(); }
        }

        string authorName = string.Empty;
        public string AuthorName
        {
            get { return authorName; }
            set { authorName = value; OnPropertyChanged(); }
        }

        string bookCover = string.Empty;
        public string BookCover
        {
            get { return bookCover; }
            set { bookCover = value; OnPropertyChanged(); }
        }

        string ownName = string.Empty;
        public string OwnName
        {
            get { return ownName; }
            set { ownName = value; OnPropertyChanged(); }
        }

        string ownAuthor = string.Empty;
        public string OwnAuthor
        {
            get { return ownAuthor; }
            set { ownAuthor = value; OnPropertyChanged(); }
        }

        string ownISBN = string.Empty;
        public string OwnISBN
        {
            get { return ownISBN; }
            set { ownISBN = value; OnPropertyChanged(); }
        }

        string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        string ownDescription = string.Empty;
        public string OwnDescription
        {
            get { return ownDescription; }
            set { ownDescription = value; OnPropertyChanged(); }
        }

        public ICommand SearchByISBNCommand { get; }
        //public ICommand SearchByQueryCommand { get; }
        //public ICommand NextPageCommand { get; }
        //public ICommand PreviousPageCommand { get; }
        public ICommand BookSelectedCommand { get; }
        public ICommand AddOwnBookCommand { get; }

        void BookSelected(object item)
        {
            /*ListBook parameter = item as ListBook;
            var resp = await RequestsHelper.MakeGetRequest<BookByISBNResponse>($"books?bibkeys=ISBN:{parameter.ISBN}&jscmd=data&format=json", "https://openlibrary.org/api/", true);
            if (resp != null)
            {
                currentBook = new Book
                {
                    ISBN = parameter.ISBN,
                    SmallCover = resp.ISBN.Cover != null ? resp.ISBN.Cover.Small : @"https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png",
                    MediumCover = resp.ISBN.Cover != null ? resp.ISBN.Cover.Medium : @"https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png",
                    LargeCover = resp.ISBN.Cover != null ? resp.ISBN.Cover.Large : @"https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png",
                    Title = resp.ISBN.Title,
                    Authors = string.Join(", ", resp.ISBN.Authors.Select(auth => auth.Name))
                };
                BookListDisplay = false;
                AddByISBNShow = true;
                BookName = $"Title: {currentBook.Title}";
                AuthorName = $"Author(s): {currentBook.Authors}";
                BookCover = currentBook.MediumCover;
            }*/
        }

        /*async void NextPage()
        {
            PageNumber += 1;
            ObservableCollection<ListBook> books = new ObservableCollection<ListBook>();
            string[] nextBooks = Isbns.Skip(PageSize * (PageNumber - 1)).Take(PageSize).ToArray();
            foreach (string b in nextBooks)
            {
                var resp2 = await RequestsHelper.MakeGetRequest<BookByISBNResponse>($"books?bibkeys=ISBN:{b}&jscmd=data&format=json", "https://openlibrary.org/api/", true);
                if (resp2 != null)
                {
                    ListBook _book = new ListBook
                    {
                        ISBN = b,
                        Title = resp2.ISBN.Title,
                        Authors = new List<string>()
                    };
                    if (resp2.ISBN.Authors != null)
                    {
                        foreach (var author in resp2.ISBN.Authors)
                        {
                            _book.Authors.Add(author.Name);
                        }
                    }
                    books.Add(_book);
                }
            }
            BookList = books;
            if (PageNumber * PageSize >= Isbns.Length)
            {
                NextButtonVisible = false;
            }
            PrevButtonVisible = true;
        }*/

        /*async void PreviousPage()
        {
            PageNumber -= 1;
            ObservableCollection<ListBook> books = new ObservableCollection<ListBook>();
            string[] nextBooks = Isbns.Skip(PageSize * (PageNumber - 1)).Take(PageSize).ToArray();
            foreach (string b in nextBooks)
            {
                var resp2 = await RequestsHelper.MakeGetRequest<BookByISBNResponse>($"books?bibkeys=ISBN:{b}&jscmd=data&format=json", "https://openlibrary.org/api/", true);
                if (resp2 != null)
                {
                    ListBook _book = new ListBook
                    {
                        ISBN = b,
                        Title = resp2.ISBN.Title,
                        Authors = new List<string>()
                    };
                    if (resp2.ISBN.Authors != null)
                    {
                        foreach (var author in resp2.ISBN.Authors)
                        {
                            _book.Authors.Add(author.Name);
                        }
                    }
                    books.Add(_book);
                }
            }
            BookList = books;
            NextButtonVisible = true;
            if (PageNumber == 1)
            {
                PrevButtonVisible = false;
            }
        }*/

        /*async void SearchByQuery()
        {
            string searchQuery = "search.json?";
            if (!string.IsNullOrEmpty(TitleQuery))
            {
                searchQuery += $"title={TitleQuery}";
            }
            if (!string.IsNullOrEmpty(AuthorQuery))
            {
                if (searchQuery == "search.json?")
                {
                    searchQuery += $"author={AuthorQuery}";
                }
                else
                {
                    searchQuery += $"&author={AuthorQuery}";
                }
            }

            var resp = await RequestsHelper.MakeGetRequest<BookByQueryResponse>(searchQuery, "http://openlibrary.org");
            resp.Docs = resp.Docs.Where(e => e.Isbn != null).ToArray();
            Isbns = resp.Docs.SelectMany(e => e.Isbn).Where(e => e.ToCharArray().All(e2 => char.IsDigit(e2))).ToArray();
            Isbns = isbns.Distinct().ToArray();
            PageSize = 10;
            PageNumber = 1;
            ObservableCollection<ListBook> books = new ObservableCollection<ListBook>();
            string[] nextBooks = Isbns.Take(PageSize).ToArray();
            foreach (string b in nextBooks)
            {
                var resp2 = await RequestsHelper.MakeGetRequest<BookByISBNResponse>($"books?bibkeys=ISBN:{b}&jscmd=data&format=json", "https://openlibrary.org/api/", true);
                if (resp2 != null)
                {
                    ListBook _book = new ListBook
                    {
                        ISBN = b,
                        Title = resp2.ISBN.Title,
                        Authors = new List<string>()
                    };
                    if (resp2.ISBN.Authors != null)
                    {
                        foreach (var author in resp2.ISBN.Authors)
                        {
                            _book.Authors.Add(author.Name);
                        }
                    }
                    books.Add(_book);
                }
            }
            BookList = books;
            if(PageNumber * PageSize < Isbns.Length)
            {
                NextButtonVisible = true;
            }
            //AddByQueryDisplay = false;
            BookListDisplay = true;
        }*/

        async void SearchByISBN()
        {
            var resp = await RequestsHelper.MakeGoodReadsRequest($"{ISBN}");
            if(resp != null && resp.Search != null && resp.Search.Results != null && resp.Search.Results.Work != null && resp.Search.Results.Work.Count > 0)
            {
                var book = resp.Search.Results.Work[0];
                currentBook = new Book
                {
                    WorkId = book.Id.Text,
                    GoodreadsId = book.Best_book.Id.Text,
                    Title = book.Best_book.Title,
                    AuthorName = book.Best_book.Author.Name,
                    LargeCover = book.Best_book.Image_url
                };
                AddByISBNDisplay = false;
                AddByISBNShow = true;
                BookName = $"Title: {book.Best_book.Title}";
                AuthorName = $"Author(s): {book.Best_book.Author.Name}";
                BookCover = book.Best_book.Image_url;
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Could not find book by ISBN. Please use the 'Own your own book' option", "OK");
            }
            /*var resp = await RequestsHelper.MakeGetRequest<SearchByISBNResponse>($"books/searchByISBN?isbn={ISBN}");
            if (resp != null)
            {
                if (resp.ErrorCode == 0 && resp.Found)
                {
                    currentBook = new Book
                    {
                        ISBN = resp.Book.ISBN13
                    };
                    AddByISBNDisplay = false;
                    AddByISBNShow = true;
                    BookName = $"Title: {resp.Book.Title}";
                    AuthorName = $"Author(s): {resp.Book.Authors}";
                    BookCover = resp.Book.Image;
                }
            }*/

        }

        public ICommand AddByISBNCommand { get; }

        private bool addByISBNPressed = false;
        void AddByISBN()
        {
            try
            {
                if (!addByISBNPressed)
                {
                    addByISBNPressed = true;
                    SelectionDisplay = false;
                    AddByISBNDisplay = true;
                }
            }
            catch { }
            finally
            {
                addByISBNPressed = false;
            }
        }

        /*public ICommand AddByQueryCommand { get; }

        void AddByQuery()
        {
            SelectionDisplay = false;
            AddByQueryDisplay = true;
        }*/

        public ICommand AddByISBNMakeRequest { get; }

        private bool addByISBNRequestPressed = false;
        async void AddByISBNRequest()
        {
            try
            {
                if (!addByISBNRequestPressed)
                {
                    addByISBNRequestPressed = true;
                    bool exchange = BookOptions.Value == "Donate";
                    bool giveaway = BookOptions.Value == "Sell";
                    var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("books/add/", new AddBookRequest { Book = currentBook, UserId = GlobalVars.UserId, Exchange = exchange, Giveaway = giveaway, OwnBook = false, Description = Description });
                    if (resp.ErrorCode == 0)
                    {
                        await App.Current.MainPage.DisplayAlert("Book added", "Book succesfully added", "OK");
                    }
                }
            }
            catch { }
            finally
            {
                addByISBNRequestPressed = false;
            }
        }

        public ICommand AddOwnSubmit { get; }
        private bool addOwnPressed = false;
        async void AddOwn()
        {
            try
            {
                if (!addOwnPressed)
                {
                    addOwnPressed = true;
                    string errorMessage = string.Empty;
                    if (string.IsNullOrEmpty(OwnName))
                    {
                        errorMessage += "Please fill in the book title\n";
                    }
                    if (string.IsNullOrEmpty(OwnAuthor))
                    {
                        errorMessage += "Please fill in the author name\n";
                    }
                    if (errorMessage == string.Empty)
                    {
                        Book ownBook = new Book
                        {
                            //ISBN = OwnISBN,
                            /*SmallCover = @"https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png",
                            MediumCover = @"https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png",
                            LargeCover = @"https://rachelandrew.co.uk/perch/resources/sb3-smashing-book-3-redesign-the-web1.png",*/
                            Title = OwnName,
                            Authors = OwnAuthor
                        };
                        bool donateOwn = OwnOptions.Value == "Donate";
                        bool giveawayOwn = OwnOptions.Value == "Sell";
                        var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("books/add/", new AddBookRequest { Book = ownBook, UserId = GlobalVars.UserId, Exchange = donateOwn, Giveaway = giveawayOwn, OwnBook = true, Description = OwnDescription });
                        if (resp.ErrorCode == 0)
                        {
                            await App.Current.MainPage.DisplayAlert("Book added", "Book succesfully added", "OK");
                        }
                    }
                }
            }
            catch { }
            finally
            {
                addOwnPressed = false;
            }
        }

        public ICommand ScanBookCommand { get; }
        async void ScanBook()
        {
            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = false,
                TryHarder = true,
                PossibleFormats = new List<ZXing.BarcodeFormat>
                    {
                       ZXing.BarcodeFormat.EAN_13
                    }
            };

            var scanPage = new ZXingScannerPage(options)
            {
                DefaultOverlayTopText = "Align the barcode within the frame",
                DefaultOverlayBottomText = string.Empty,
                DefaultOverlayShowFlashButton = true
            };
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await GlobalVars.Master.Navigation.PopAsync();
                    ISBN = result.Text;

                    var resp = await RequestsHelper.MakeGoodReadsRequest($"{ISBN}");
                    if (resp != null && resp.Search != null && resp.Search.Results != null && resp.Search.Results.Work != null && resp.Search.Results.Work.Count > 0)
                    {
                        var book = resp.Search.Results.Work[0];
                        currentBook = new Book
                        {
                            WorkId = book.Id.Text,
                            GoodreadsId = book.Best_book.Id.Text,
                            Title = book.Best_book.Title,
                            AuthorName = book.Best_book.Author.Name,
                            LargeCover = book.Best_book.Image_url
                        };
                        AddByISBNDisplay = false;
                        AddByISBNShow = true;
                        BookName = $"Title: {book.Best_book.Title}";
                        AuthorName = $"Author(s): {book.Best_book.Author.Name}";
                        BookCover = book.Best_book.Image_url;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Could not find book by ISBN. Please use the 'Own your own book' option", "OK");
                    }
                });
            };
            await GlobalVars.Master.Navigation.PushAsync(scanPage);
        }

        bool addOwnBookPressed = false;

        void AddOwnBook()
        {
            try
            {
                if (!addOwnBookPressed)
                {
                    SelectionDisplay = false;
                    AddOwnBookShow = true;
                    addOwnBookPressed = true;
                }
            }
            catch(Exception e)
            {

            }
            finally
            {
                addOwnBookPressed = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}