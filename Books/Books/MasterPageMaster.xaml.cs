using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Books
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPageMaster : ContentPage
    {
        public ListView ListView;

        public MasterPageMaster()
        {
            InitializeComponent();

            BindingContext = new MasterPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MasterPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterPageMenuItem> MenuItems { get; set; }

            public MasterPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterPageMenuItem>(new[]
                {
                    new MasterPageMenuItem { Id = 1, Title = "Profile Page", TargetType = typeof(ProfilePage) },
                    new MasterPageMenuItem { Id = 2, Title = "Search", TargetType = typeof(SearchPage) },
                    new MasterPageMenuItem { Id = 3, Title = "Add Book", TargetType = typeof(AddBookPage) },
                    new MasterPageMenuItem { Id = 4, Title = "Book requests", TargetType = typeof(MyRequestedBooks) },
                    new MasterPageMenuItem { Id = 5, Title = "My Wishlist", TargetType = typeof(MyWishlist) },
                    new MasterPageMenuItem { Id = 6, Title = "My Borrow History", TargetType = typeof(BorrowHistory) },
                    new MasterPageMenuItem { Id = 7, Title = "My Library", TargetType = typeof(MyLibrary) },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}