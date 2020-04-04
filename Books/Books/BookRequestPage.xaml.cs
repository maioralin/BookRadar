using Books.OtherClasses;
using Books.Requests;
using Books.Responses;
using Books.SqlClasses;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Common;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Books
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookRequestPage : ContentPage
    {
        public BookRequestPage()
        {
            try
            {
                InitializeComponent();
                BindingContext = new BookRequestPageViewModel();
                ((BookRequestPageViewModel)this.BindingContext).OnMessageAdded = ((obj) =>
                {
                    BooksListView.ScrollTo(obj, ScrollToPosition.End, false);
                });
            }
            catch(Exception e)
            {
                Device.BeginInvokeOnMainThread(() => Task.Run(async () => { await RequestsHelper.MakePostRequest<GetMessagesResponse>($"notificationSender/sendErrorEmail/", new EmailRequest { Exception = e }); }));
            }
        }

        class BookRequestPageViewModel : INotifyPropertyChanged
        {
            public BookRequestPageViewModel()
            {
                ShowAds = string.IsNullOrEmpty(GlobalVars.PurchaseId);
                Authors = GlobalVars.CurrentRequest.Authors;
                Title = GlobalVars.CurrentRequest.Title;
                OwnerName = GlobalVars.CurrentRequest.OwnerName;
                RequesterName = GlobalVars.CurrentRequest.RequesterName;
                Cover = GlobalVars.CurrentRequest.Cover;
                OfferButtonVisible = !GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.OwnerId == GlobalVars.UserId;
                AcceptButtonVisible = GlobalVars.CurrentRequest.RequesterId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer != null && GlobalVars.CurrentRequest.BookOffer.ProposedReturnDate.HasValue && !GlobalVars.CurrentRequest.BookOffer.BookAccepted;
                ReturnButtonVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.RequesterId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer.BookAccepted && !GlobalVars.CurrentRequest.BookOffer.ReturnOffered;
                AcceptReturnButtonVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.OwnerId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer.BookAccepted && !GlobalVars.CurrentRequest.BookOffer.ActualReturnDate.HasValue;
                ReviewRequesterButtonVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.OwnerId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer.BookAccepted && GlobalVars.CurrentRequest.BookOffer.ActualReturnDate.HasValue && !GlobalVars.CurrentRequest.IsUserReviewed;
                MakeBookAvailableButtonVisible = GlobalVars.CurrentRequest.Donate && GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.RequesterId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer.BookAccepted;
                OfferExchangeButtonVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && !GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.RequesterId == GlobalVars.UserId;
                OfferedExchangeTextVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && GlobalVars.CurrentRequest.OwnerId == GlobalVars.UserId && GlobalVars.CurrentRequest.ProposedBookId.HasValue;
                RequestTimeButtonVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && GlobalVars.CurrentRequest.RequesterId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer.Id.HasValue && !GlobalVars.CurrentRequest.BookOffer.ExtendedDate.HasValue && !GlobalVars.CurrentRequest.BookOffer.ActualReturnDate.HasValue;
                TimeRequestResponseVisible = !GlobalVars.CurrentRequest.Donate && !GlobalVars.CurrentRequest.Giveaway && GlobalVars.CurrentRequest.OwnerId == GlobalVars.UserId && GlobalVars.CurrentRequest.BookOffer.Id.HasValue && GlobalVars.CurrentRequest.BookOffer.ExtendedDate.HasValue;
                if(TimeRequestResponseVisible)
                {
                    ExtraTimeText = $"{GlobalVars.CurrentRequest.RequesterName} has requested extra time to return the book, until {GlobalVars.CurrentRequest.BookOffer.ExtendedDate.Value}";
                }
                if(OfferedExchangeTextVisible)
                {
                    string authors = GlobalVars.CurrentRequest.ProposedBook.Authors;
                    OfferedExchangeText = $"{GlobalVars.CurrentRequest.RequesterName} has offered {GlobalVars.CurrentRequest.ProposedBook.Title} by {authors}";
                }
                OfferBookCommand = new Command(OfferBook);
                OfferExchangeBookCommand = new Command(OfferExchangeBook);
                AcceptBookCommand = new Command(AcceptBook);
                ReturnBookCommand = new Command(ReturnBook);
                AcceptReturnBookCommand = new Command(AcceptReturnBook);
                SendMessageCommand = new Command(SendMessage);
                ReviewRequesterCommand = new Command(ReviewRequester);
                BookRequestPageAppearingCommand = new Command(BookRequestPageAppearing);
                LoadMessagesCommand = new Command(LoadMessages);
                RejectExchangeBookCommand = new Command(RejectExchangeBook);
                RequestTimeCommand = new Command(RequestTime);
                AcceptTimeRequestCommand = new Command(AcceptTimeRequest);
                RejectTimeRequestCommand = new Command(RejectTimeRequest);
                ViewUserCommand = new Command(ViewUser);
                ViewBookCommand = new Command(ViewBook);
                Messages = new ObservableCollection<Message>();
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
                GlobalVars.ChatClient.OnMessageReceived += (message, fromId, hubParams) => {

                    if (hubParams.RequestId == GlobalVars.CurrentRequest.Id)
                    {
                        string name = string.Empty;
                        Color frameColor = fromId == GlobalVars.UserId ? Color.FromHex("2db2ff") : Color.Gray;
                        Thickness margin = fromId == GlobalVars.UserId ? new Thickness(80,10,10,10) : new Thickness(10,10,80,10);
                        if (fromId == GlobalVars.CurrentRequest.RequesterId)
                        {
                            name = GlobalVars.CurrentRequest.RequesterName;
                        }
                        else
                        {
                            name = GlobalVars.CurrentRequest.OwnerName;
                        }
                        if (fromId != GlobalVars.UserId)
                        {
                            Device.BeginInvokeOnMainThread(() => Messages.Add(new Message { Name = name, Text = message, SentDate = hubParams.SentDate.ToLocalTime(), FrameColor = frameColor, Margin = margin }));
                        }
                    }
                };
                GlobalVars.ChatClient.OnBookExchangeRequest += (requestId, hubParams, bookId) => {

                    if (requestId == GlobalVars.CurrentRequest.Id && hubParams.ToUserId == GlobalVars.UserId)
                    {
                        GlobalVars.CurrentRequest.ProposedBookId = bookId;
                        OfferedExchangeText = $"{GlobalVars.CurrentRequest.RequesterName} has offered {hubParams.Title} by {hubParams.Authors}";
                        OfferedExchangeTextVisible = true;
                    }
                };

                GlobalVars.ChatClient.OnExchangeRejected += (requestId, toUserId) =>
                {
                    if (requestId == GlobalVars.CurrentRequest.Id && toUserId == GlobalVars.UserId)
                    {
                        Device.BeginInvokeOnMainThread(() => Task.Run(async () => { await App.Current.MainPage.DisplayAlert("Exchange rejectedd", "Owner has rejected the book exchange", "OK"); }));
                    }
                };

                GlobalVars.ChatClient.OnTimeRequestAnswered += (requestId, toUserId, answer) =>
                {
                    if (requestId == GlobalVars.CurrentRequest.Id && toUserId == GlobalVars.UserId)
                    {
                        RequestTimeButtonVisible = true;
                        if (answer)
                        {
                            GlobalVars.CurrentRequest.BookOffer.ProposedReturnDate = GlobalVars.CurrentRequest.BookOffer.ExtendedDate;
                            Device.BeginInvokeOnMainThread(() => Task.Run(async () => { await App.Current.MainPage.DisplayAlert("Request accepted", "Owner has accepted your extra time request", "OK"); }));
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() => Task.Run(async () => { await App.Current.MainPage.DisplayAlert("Request rejected", "Owner has rejected your extra time request", "OK"); }));
                        }
                        GlobalVars.CurrentRequest.BookOffer.ExtendedDate = null;
                        RequestTimeButtonVisible = true;
                    }
                };

                GlobalVars.ChatClient.OnExtraTimeRequested += (requestId, toUserId, proposedDate) =>
                {
                    if (requestId == GlobalVars.CurrentRequest.Id && toUserId == GlobalVars.UserId)
                    {
                        GlobalVars.CurrentRequest.BookOffer.ExtendedDate = proposedDate;
                        TimeRequestResponseVisible = true;
                        ExtraTimeText = $"{GlobalVars.CurrentRequest.RequesterName} has requested extra time to return the book, until {GlobalVars.CurrentRequest.BookOffer.ExtendedDate.Value}";
                    }
                };

                GlobalVars.ChatClient.OnBookOffered += (requestId, bookOfferId, date) =>
                {
                    if (requestId == GlobalVars.CurrentRequest.Id)
                    {
                        GlobalVars.CurrentRequest.BookOffer = new BookOffer
                        {
                            BookRequestId = GlobalVars.CurrentRequest.Id,
                            Id = bookOfferId
                        };
                        AcceptButtonVisible = true;
                    }
                };

                GlobalVars.ChatClient.OnReturnOffered += (requestId) =>
                {
                    if (requestId == GlobalVars.CurrentRequest.Id)
                    {
                        GlobalVars.CurrentRequest.BookOffer.ReturnOffered = true;
                        AcceptReturnButtonVisible = true;
                    }
                };
            }

            public Action<Message> OnMessageAdded { get; set; }

            ObservableCollection<Message> messages;
            public ObservableCollection<Message> Messages
            {
                get { return messages; }
                set { messages = value; OnPropertyChanged(); }
            }

            public ICommand OfferBookCommand { get; }
            public ICommand AcceptBookCommand { get; }
            public ICommand ReturnBookCommand { get; }
            public ICommand AcceptReturnBookCommand { get; }
            public ICommand ReviewRequesterCommand { get; }
            public ICommand SendMessageCommand { get; }
            public ICommand MakeBookAvailableCommand { get; }
            public ICommand BookRequestPageAppearingCommand { get; }
            public ICommand LoadMessagesCommand { get; }
            public ICommand OfferExchangeBookCommand { get; }
            public ICommand RejectExchangeBookCommand { get; }
            public ICommand RequestTimeCommand { get; }
            public ICommand AcceptTimeRequestCommand { get; }
            public ICommand RejectTimeRequestCommand { get; }
            public ICommand ViewUserCommand { get; }
            public ICommand ViewBookCommand { get; }

            public bool showAds;
            public bool ShowAds
            {
                get { return showAds; }
                set { showAds = value; OnPropertyChanged(); }
            }

            bool offerButtonVisible = false;
            public bool OfferButtonVisible
            {
                get { return offerButtonVisible; }
                set { offerButtonVisible = value; OnPropertyChanged(); }
            }

            string code = "alin";
            public string Code
            {
                get { return code; }
                set { code = value; OnPropertyChanged(); }
            }

            string offeredExchangeText = string.Empty;
            public string OfferedExchangeText
            {
                get { return offeredExchangeText; }
                set { offeredExchangeText = value; OnPropertyChanged(); }
            }

            EncodingOptions options = new EncodingOptions { Width = 50, Height = 50, Margin = 10 };
            public EncodingOptions Opions
            {
                get { return options; }
                set { options = value; OnPropertyChanged(); }
            }

            bool acceptButtonVisible = false;
            public bool AcceptButtonVisible
            {
                get { return acceptButtonVisible; }
                set { acceptButtonVisible = value; OnPropertyChanged(); }
            }

            bool offerExchangeButtonVisible = false;
            public bool OfferExchangeButtonVisible
            {
                get { return offerExchangeButtonVisible; }
                set { offerExchangeButtonVisible = value; OnPropertyChanged(); }
            }

            bool offeredExchangeTextVisible = false;
            public bool OfferedExchangeTextVisible
            {
                get { return offeredExchangeTextVisible; }
                set { offeredExchangeTextVisible = value; OnPropertyChanged(); }
            }

            bool makeBookAvailableButtonVisible = false;
            public bool MakeBookAvailableButtonVisible
            {
                get { return makeBookAvailableButtonVisible; }
                set { makeBookAvailableButtonVisible = value; OnPropertyChanged(); }
            }

            bool reviewRequesterButtonVisible = false;
            public bool ReviewRequesterButtonVisible
            {
                get { return reviewRequesterButtonVisible; }
                set { reviewRequesterButtonVisible = value; OnPropertyChanged(); }
            }

            bool returnButtonVisible = false;
            public bool ReturnButtonVisible
            {
                get { return returnButtonVisible; }
                set { returnButtonVisible = value; OnPropertyChanged(); }
            }

            bool loadMessagesButtonVisible = false;
            public bool LoadMessagesButtonVisible
            {
                get { return loadMessagesButtonVisible; }
                set { loadMessagesButtonVisible = value; OnPropertyChanged(); }
            }

            bool acceptReturnButtonVisible = false;
            public bool AcceptReturnButtonVisible
            {
                get { return acceptReturnButtonVisible; }
                set { acceptReturnButtonVisible = value; OnPropertyChanged(); }
            }

            bool requestTimeButtonVisible = false;
            public bool RequestTimeButtonVisible
            {
                get { return requestTimeButtonVisible; }
                set { requestTimeButtonVisible = value; OnPropertyChanged(); }
            }

            DateTime proposedDate = DateTime.Now.AddDays(1);
            public DateTime ProposedDate
            {
                get { return proposedDate; }
                set { proposedDate = value; OnPropertyChanged(); }
            }

            string messageText = string.Empty;
            public string MessageText
            {
                get { return messageText; }
                set { messageText = value; OnPropertyChanged(); }
            }

            string authors = string.Empty;
            public string Authors
            {
                get { return authors; }
                set { authors = value; OnPropertyChanged(); }
            }

            string title = string.Empty;
            public string Title
            {
                get { return title; }
                set { title = value; OnPropertyChanged(); }
            }

            string cover = string.Empty;
            public string Cover
            {
                get { return cover; }
                set { cover = value; OnPropertyChanged(); }
            }

            string ownerName = string.Empty;
            public string OwnerName
            {
                get { return ownerName; }
                set { ownerName = value; OnPropertyChanged(); }
            }

            string requesterName = string.Empty;
            public string RequesterName
            {
                get { return requesterName; }
                set { requesterName = value; OnPropertyChanged(); }
            }

            bool timeRequestResponseVisible = false;
            public bool TimeRequestResponseVisible
            {
                get { return timeRequestResponseVisible; }
                set { timeRequestResponseVisible = value; OnPropertyChanged(); }
            }

            string extraTimeText = string.Empty;
            public string ExtraTimeText
            {
                get { return extraTimeText; }
                set { extraTimeText = value; OnPropertyChanged(); }
            }

            private bool sendMessagePressed = false;
            async void SendMessage()
            {
                try
                {
                    if (!sendMessagePressed)
                    {
                        sendMessagePressed = true;
                        if (!string.IsNullOrWhiteSpace(MessageText))
                        {
                            string message = MessageText;
                            Message msg = new Message { Name = GlobalVars.FacebookDetails.LastName, Text = message, SentDate = DateTime.Now, FrameColor = Color.FromHex("2db2ff"), Margin = new Thickness(80, 10, 10, 10) };
                            Device.BeginInvokeOnMainThread(() => Messages.Add(msg));
                            Device.BeginInvokeOnMainThread(() => OnMessageAdded?.Invoke(msg));
                            MessageText = string.Empty;
                            Guid toUserId = GlobalVars.CurrentRequest.RequesterId == GlobalVars.UserId ? GlobalVars.CurrentRequest.OwnerId : GlobalVars.CurrentRequest.RequesterId;
                            SaveMessageRequest request = new SaveMessageRequest
                            {
                                From = GlobalVars.UserId,
                                To = toUserId,
                                Message = message,
                                RequestId = GlobalVars.CurrentRequest.Id
                            };
                            var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("messages/save/", request);
                            if (resp.ErrorCode == 0)
                            {
                                string target = toUserId.ToString();
                                Dictionary<string, string> param = new Dictionary<string, string>();
                                param.Add("action", "messages");
                                param.Add("requestId", GlobalVars.CurrentRequest.Id.ToString());
                                NotificationRequest notificationRequest = new NotificationRequest
                                {
                                    Title = $"{GlobalVars.FacebookDetails.LastName} sent you a message",
                                    Message = message,
                                    Target = target,
                                    Params = param
                                };
                                await RequestsHelper.MakePostRequest<DefaultResponse>("notificationSender/send", notificationRequest);
                                GlobalVars.ChatClient.SendPrivateMessage(message, toUserId, GlobalVars.CurrentRequest.Id);
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    sendMessagePressed = false;
                }
            }

            private bool offerBookPressed = false;
            async void OfferBook()
            {
                try
                {
                    if (!offerBookPressed)
                    {
                        offerBookPressed = true;
                        var resp = await RequestsHelper.MakePostRequest<GuidResponse>("borrow/offerBook/", new OfferBookRequest { RequestId = GlobalVars.CurrentRequest.Id, ProposedReturnDate = ProposedDate });
                        if (resp.ErrorCode == 0)
                        {
                            OfferButtonVisible = false;
                            GlobalVars.CurrentRequest.BookOffer = new Books.OtherClasses.BookOffer { Id = resp.Id };
                            GlobalVars.ChatClient.OfferBook(GlobalVars.CurrentRequest.Id, GlobalVars.CurrentRequest.RequesterId, resp.Id, ProposedDate);
                            var page = new OfferBookQRPage();
                            await GlobalVars.Master.Navigation.PushPopupAsync(page, true);
                        }
                    }
                }
                catch { }
                finally
                {
                    offerBookPressed = false;
                }
            }

            async void ReviewRequester()
            {
                var page = new ReviewUserPage();
                await GlobalVars.Master.Navigation.PushPopupAsync(page, true);
            }

            async void OfferExchangeBook()
            {
                var page = new OfferExchangeBook();
                await GlobalVars.Master.Navigation.PushPopupAsync(page, true);
            }

            async void RequestTime()
            {
                var page = new RequestTime();
                await GlobalVars.Master.Navigation.PushPopupAsync(page, true);
            }

            private bool rejectBookPressed = false;
            async void RejectExchangeBook()
            {
                try
                {
                    if (!rejectBookPressed)
                    {
                        rejectBookPressed = true;
                        BookExchangeRequest request = new BookExchangeRequest
                        {
                            RequestId = GlobalVars.CurrentRequest.Id
                        };
                        var resp = await RequestsHelper.MakePostRequest<BookRequestResponse>("borrow/rejectBookExchange/", request);
                        if (resp != null && resp.ErrorCode == 0)
                        {
                            OfferedExchangeTextVisible = false;
                            GlobalVars.ChatClient.RejectExchangeBook(GlobalVars.CurrentRequest.Id, GlobalVars.CurrentRequest.RequesterId);
                        }
                        else
                        {
                            //error message
                        }
                    }
                }
                catch { }
                finally
                {
                    rejectBookPressed = false;
                }
            }

            async void AcceptBook()
            {

                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    TryHarder = true,
                    PossibleFormats = new List<ZXing.BarcodeFormat>
                    {
                       ZXing.BarcodeFormat.QR_CODE
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
                        Guid id = new Guid(result.Text);
                        var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/acceptBook/", new AcceptBookRequest { BookOfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value, BookId = GlobalVars.CurrentRequest.BookId, RequesterId = GlobalVars.UserId });
                        if (resp.ErrorCode == 0)
                        {
                            AcceptButtonVisible = false;
                            await App.Current.MainPage.DisplayAlert("Book accepted", "Book succesfully accepted", "OK");
                        }
                    });
                };
                // Navigate to our scanner page
                await GlobalVars.Master.Navigation.PushAsync(scanPage);
            }

            private bool returnBookPressed = false;
            async void ReturnBook()
            {
                try
                {
                    if (!returnBookPressed)
                    {
                        returnBookPressed = true;
                        var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/returnBook/", new AcceptBookRequest { BookOfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value });
                        if (resp.ErrorCode == 0)
                        {
                            ReturnButtonVisible = false;
                            GlobalVars.ChatClient.ReturnBook(GlobalVars.CurrentRequest.Id, GlobalVars.CurrentRequest.BookOffer.Id.Value);
                            var page = new ReturnBookQRPage();
                            await GlobalVars.Master.Navigation.PushPopupAsync(page, true);
                        }
                    }
                }
                catch { }
                finally
                {
                    returnBookPressed = false;
                }
            }

            async void MakeBookAvailable()
            {
                var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/makeBookAvailable/", new BookAvailableRequest { BookId = GlobalVars.CurrentRequest.BookId, Available = true });
                if (resp.ErrorCode == 0)
                {
                    await App.Current.MainPage.DisplayAlert("Succes", "Book is now available for donation again!", "OK");
                }
            }

            async void AcceptTimeRequest()
            {
                var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/RequestTimeResponse/", new ExtendBookResponseRequest { Answer = true, OfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value, ProposedDate = GlobalVars.CurrentRequest.BookOffer.ExtendedDate.Value });
                if (resp.ErrorCode == 0)
                {
                    GlobalVars.ChatClient.AnswerTimeRequest(GlobalVars.CurrentRequest.Id, GlobalVars.CurrentRequest.RequesterId, true);
                    TimeRequestResponseVisible = false;
                }
            }

            async void RejectTimeRequest()
            {
                var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/RequestTimeResponse/", new ExtendBookResponseRequest { Answer = false, OfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value, ProposedDate = GlobalVars.CurrentRequest.BookOffer.ExtendedDate.Value });
                if (resp.ErrorCode == 0)
                {
                    GlobalVars.ChatClient.AnswerTimeRequest(GlobalVars.CurrentRequest.Id, GlobalVars.CurrentRequest.RequesterId, false);
                    TimeRequestResponseVisible = false;
                }
            }

            async void AcceptReturnBook()
            {
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    TryHarder = true,
                    PossibleFormats = new List<ZXing.BarcodeFormat>
                    {
                       ZXing.BarcodeFormat.QR_CODE
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
                        Guid id = new Guid(result.Text);
                        var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("borrow/acceptReturnBook/", new AcceptBookRequest { BookOfferId = GlobalVars.CurrentRequest.BookOffer.Id.Value, BookId = GlobalVars.CurrentRequest.BookId, RequesterId = GlobalVars.UserId, ProposedReturnBook = GlobalVars.CurrentRequest.ProposedBookId });
                        if (resp.ErrorCode == 0)
                        {
                            AcceptReturnButtonVisible = false;
                            await App.Current.MainPage.DisplayAlert("Book returned", "Book has been received!", "OK");
                        }
                    });
                };
                // Navigate to our scanner page
                await GlobalVars.Master.Navigation.PushAsync(scanPage);
            }

            async void ViewUser()
            {
                var page = (Page)Activator.CreateInstance(typeof(UserDetailsPage), GlobalVars.CurrentRequest.OwnerId);
                await GlobalVars.Master.Navigation.PushAsync(page);
            }

            private bool viewBookPressed = false;
            async void ViewBook()
            {
                try
                {
                    if (!viewBookPressed)
                    {
                        viewBookPressed = true;
                        var resp = await RequestsHelper.MakeGetRequest<BookByIdResponse>($"books/getBookById/?BookId={GlobalVars.CurrentRequest.BookId}");
                        if (resp != null)
                        {
                            GlobalVars.VisitedBook = resp.Book;
                            GlobalVars.VisitedBook.Id = GlobalVars.CurrentRequest.BookId;
                            GlobalVars.VisitedBook.Distance = -1;
                            var page = (Page)Activator.CreateInstance(typeof(VisitedBook));
                            await GlobalVars.Master.Navigation.PushAsync(page);
                        }
                    }
                }
                catch(Exception e)
                { }
                finally
                {
                    viewBookPressed = false;
                }
            }

            async void BookRequestPageAppearing()
            {
                try
                {
                    var resp = await RequestsHelper.MakeGetRequest<GetMessagesResponse>($"messages/getByRequestId/?RequestId={GlobalVars.CurrentRequest.Id}&StartDate={DateTime.UtcNow}");
                    if (resp.ErrorCode == 0 && resp.Messages != null)
                    {
                        ObservableCollection<Message> messages = new ObservableCollection<Message>();
                        resp.Messages = resp.Messages.OrderBy(r => r.SentDate).ToList();
                        foreach (var message in resp.Messages)
                        {
                            string messageFrom = string.Empty;
                            Color frameColor = message.MessageFrom == GlobalVars.UserId ? Color.FromHex("2db2ff") : Color.Gray;
                            Thickness margin = message.MessageFrom == GlobalVars.UserId ? new Thickness(80, 10, 10, 10) : new Thickness(10, 10, 80, 10);
                            if (message.MessageFrom == GlobalVars.CurrentRequest.RequesterId)
                            {
                                messageFrom = GlobalVars.CurrentRequest.RequesterName;
                            }
                            if (message.MessageFrom == GlobalVars.CurrentRequest.OwnerId)
                            {
                                messageFrom = GlobalVars.CurrentRequest.OwnerName;
                            }
                            messages.Add(new Message
                            {
                                Name = messageFrom,
                                SentDate = message.SentDate.ToLocalTime(),
                                Text = message.MessageContent,
                                FrameColor = frameColor,
                                Margin = margin
                            });
                        }
                        Messages = messages;
                        if (resp.Messages.Count > 0)
                        {
                            LoadMessagesButtonVisible = true;
                        }
                    }
                }
                catch(Exception e)
                {
                    var resp = await RequestsHelper.MakePostRequest<GetMessagesResponse>($"notificationSender/sendErrorEmail/", new EmailRequest { Exception = e });
                }
            }

            async void LoadMessages()
            {
                DateTime startDate = Messages.Min(m => m.SentDate).ToUniversalTime();
                var resp = await RequestsHelper.MakeGetRequest<GetMessagesResponse>($"messages/getByRequestId/?RequestId={GlobalVars.CurrentRequest.Id}&StartDate={startDate}");
                if (resp.ErrorCode == 0)
                {
                    ObservableCollection<Message> messages = new ObservableCollection<Message>();
                    foreach (var message in resp.Messages)
                    {
                        string messageFrom = string.Empty;
                        if (message.MessageFrom == GlobalVars.CurrentRequest.RequesterId)
                        {
                            messageFrom = GlobalVars.CurrentRequest.RequesterName;
                        }
                        if (message.MessageFrom == GlobalVars.CurrentRequest.OwnerId)
                        {
                            messageFrom = GlobalVars.CurrentRequest.OwnerName;
                        }
                        Messages.Insert(0, new Message
                        {
                            Name = messageFrom,
                            SentDate = message.SentDate,
                            Text = message.MessageContent
                        });
                    }
                    if (resp.Messages.Count < 30)
                    {
                        LoadMessagesButtonVisible = false;
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}