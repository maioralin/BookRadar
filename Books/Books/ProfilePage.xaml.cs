using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Geolocator;
using Books.Requests;
using Books.Responses;
using Books.OtherClasses;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Essentials;

namespace Books
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfilePageViewModel();
        }
    }

    class ProfilePageViewModel : INotifyPropertyChanged
    {

        public ProfilePageViewModel()
        {
            LogoutCommand = new Command(Logout);
            ProfilePageAppearingCommand = new Command(ProfilePageAppearing);
            UpdateGeoLocationCommand = new Command(UpdateGeoLocation);
            SubmitReferralCodeCommand = new Command(SubmitReferralCode);
            SubmitInviteCodeCommand = new Command(SubmitInviteCode);
            ShareReferralCodeCommand = new Command(ShareReferralCode);
            DeleteAccountCommand = new Command(DeleteAccount);
            RemoveAdsCommand = new Command(RemoveAds);
            if (GlobalVars.ChatClient == null)
            {
                GlobalVars.ChatClient = new ChatClient(RequestsHelper.Root);
                GlobalVars.ChatClient.Start().ContinueWith(task =>
                {

                    if (task.IsFaulted)
                    {
                        ;
                    }
                    else
                    {
                        GlobalVars.ChatClient.Connect(GlobalVars.FacebookDetails.LastName, GlobalVars.UserId);
                    }
                });
            }
        }

        string myReferalCode = GlobalVars.MyReferralCode ?? string.Empty;
        public string MyReferalCode
        {
            get { return myReferalCode; }
            set { myReferalCode = value; OnPropertyChanged(); }
        }

        string myInviteCode = string.Empty;
        public string MyInviteCode
        {
            get { return myInviteCode; }
            set { myInviteCode = value; OnPropertyChanged(); }
        }

        bool showMyReferralCode = GlobalVars.MyReferralCode == null;
        public bool ShowMyReferralCode
        {
            get { return showMyReferralCode; }
            set { showMyReferralCode = value; OnPropertyChanged(); }
        }

        bool showReferralCodeLabel = GlobalVars.MyReferralCode != null;
        public bool ShowReferralCodeLabel
        {
            get { return showReferralCodeLabel; }
            set { showReferralCodeLabel = value; OnPropertyChanged(); }
        }

        bool showMyInviteCode = GlobalVars.InviteCode == null;
        public bool ShowMyInviteCode
        {
            get { return showMyInviteCode; }
            set { showMyInviteCode = value; OnPropertyChanged(); }
        }

        string lastNameWelcome = $"Welcome {GlobalVars.FacebookDetails.LastName}";
        public string LastNameWelcome
        {
            get { return lastNameWelcome; }
        }

        string profilePicture = GlobalVars.FacebookDetails.ProfilePicture;
        public string ProfilePicture
        {
            get { return profilePicture; }
        }

        double latitude = 0;
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; OnPropertyChanged(); }
        }

        double longitude = 0;
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; OnPropertyChanged(); }
        }

        public ICommand LogoutCommand { get; }
        public ICommand ProfilePageAppearingCommand { get; }
        public ICommand UpdateGeoLocationCommand { get; }
        public ICommand SubmitInviteCodeCommand { get; }
        public ICommand SubmitReferralCodeCommand { get; }
        public ICommand ShareReferralCodeCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand RemoveAdsCommand { get; }

        async void UpdateGeoLocation()
        {
            UpdateUserLocationRequest request = new UpdateUserLocationRequest
            {
                UserId = GlobalVars.UserId,
                Latitude = Latitude,
                Longitude = Longitude
            };
            GlobalVars.GeoLocation.Latitude = (decimal)Latitude;
            GlobalVars.GeoLocation.Longitude = (decimal)Longitude;
            await RequestsHelper.MakePostRequest<string>("facebook/updateUserLocation", request);
        }

        async void ShareReferralCode()
        {
            if (!CrossShare.IsSupported)
                return;

            await CrossShare.Current.Share(new ShareMessage
            {
                Title = "Share referral code",
                Text = $"Join BookRadar using my referral code: {MyReferalCode}",
                Url = "https://bookradarapp.com/"
            });
        }

        async void DeleteAccount()
        {
            DeleteAccountRequest request = new DeleteAccountRequest
            {
                UserId = GlobalVars.UserId
            };
            var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("facebook/delete", request);
            if(resp.ErrorCode == 1)
            {
                await App.Current.MainPage.DisplayAlert("Error", "You can't delete your account while you have unreturned books. Please return your books to their owner and try again.", "OK");
            }
            else
            {
                if(resp.ErrorCode == 0)
                {
                    Logout();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", resp.ErrorMessage, "OK");
                }
            }
        }

        async void RemoveAds()
        {
            try
            {
                var productId = "PaidAds";

                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    return;
                }
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.InAppPurchase, "PaidAds");
                if (purchase == null)
                {
                }
                else
                {
                    var id = purchase.Id;
                    var token = purchase.PurchaseToken;
                    var state = purchase.State;
                    PurchaseAdFreeRequest request = new PurchaseAdFreeRequest
                    {
                        PurchaseId = id,
                        PurchaseToken = token,
                        UserId = GlobalVars.UserId
                    };
                    var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("facebook/purchaseAdFree", request);
                    if (resp.ErrorCode == 0)
                    {
                        GlobalVars.PurchaseId = id;
                        await App.Current.MainPage.DisplayAlert("Success!", "Your purchase has been succesfully saved. Enjoy the ad-free experience!", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", resp.ErrorMessage, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }


        async void Logout()
        {
            await DependencyService.Get<IPlatformSpecificFunctions>().Logout();
            if (!GlobalVars.LoggedIn)
            {
                GlobalVars.ChatClient.Start().ContinueWith(task => {

                    if (task.IsFaulted)
                    {
                        ;
                    }
                    else
                    {
                        GlobalVars.ChatClient.Disconnect();
                    }
                }).Wait();
                App.Current.MainPage = new Home();
            }
        }

        async void SubmitReferralCode()
        {
            if (!string.IsNullOrEmpty(MyReferalCode))
            {
                ReferalCodeRequest request = new ReferalCodeRequest
                {
                    UserId = GlobalVars.UserId,
                    Code = MyReferalCode
                };
                var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("facebook/addReferalCode", request);
                if(resp.ErrorCode == 0)
                {
                    GlobalVars.MyReferralCode = MyReferalCode;
                    ShowMyReferralCode = false;
                    ShowReferralCodeLabel = true;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", resp.ErrorMessage, "OK");
                }
            }
        }

        async void SubmitInviteCode()
        {
            if (!string.IsNullOrEmpty(MyInviteCode))
            {
                ReferalCodeRequest request = new ReferalCodeRequest
                {
                    UserId = GlobalVars.UserId,
                    Code = MyInviteCode
                };
                var resp = await RequestsHelper.MakePostRequest<DefaultResponse>("facebook/addInviteCode", request);
                if (resp.ErrorCode == 0)
                {
                    GlobalVars.InviteCode = MyInviteCode;
                    ShowMyInviteCode = false;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", resp.ErrorMessage, "OK");
                }
            }
        }

        async void ProfilePageAppearing()
        {
            try
            {
                var location = new GeolocationRequest(GeolocationAccuracy.Medium);
                var position = await Geolocation.GetLocationAsync(location);
                if (position != null)
                {
                    GlobalVars.GeoLocation = new LocationDetails
                    {
                        Latitude = (decimal)position.Latitude,
                        Longitude = (decimal)position.Longitude
                    };
                    UpdateUserLocationRequest request = new UpdateUserLocationRequest
                    {
                        UserId = GlobalVars.UserId,
                        Latitude = position.Latitude,
                        Longitude = position.Longitude
                    };
                    await RequestsHelper.MakePostRequest<string>("facebook/updateUserLocation", request);
                }
                else
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
            }
            catch
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
            if (GlobalVars.Notification != null)
            {
                if (GlobalVars.Notification.NotificationAction == NotificationAction.Requests)
                {
                    GlobalVars.Notification = null;
                    var page = (Page)Activator.CreateInstance(typeof(MyRequestedBooks));
                    await GlobalVars.Master.Navigation.PushAsync(page);
                }
                else
                {
                    if (GlobalVars.Notification.NotificationAction == NotificationAction.Messages)
                    {
                        int parameterId = int.Parse(GlobalVars.Notification.Param);
                        GlobalVars.Notification = null;
                        var resp = await RequestsHelper.MakeGetRequest<BookRequestResponse>($"borrow/getRequest/?RequestId={parameterId}");
                        if (resp != null)
                        {
                            GlobalVars.CurrentRequest = resp.BookRequest;
                            var page = (Page)Activator.CreateInstance(typeof(BookRequestPage));
                            await GlobalVars.Master.Navigation.PushAsync(page);
                        }
                    }
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
