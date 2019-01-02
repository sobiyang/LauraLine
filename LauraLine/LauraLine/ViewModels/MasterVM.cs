using Newtonsoft.Json;
using System.ComponentModel;
using Xamarin.Forms;

namespace LauraLine.ViewModels
{
    public class MasterVM : INotifyPropertyChanged
    {
        [JsonIgnore]
        public INavigation Navigation
        {
            get
            {
                if (Application.Current.MainPage.GetType().IsSubclassOf(typeof(MasterDetailPage)))
                    return (Application.Current.MainPage as MasterDetailPage).Detail.Navigation;
                else
                    return Application.Current.MainPage.Navigation;
            }
        }

        [JsonIgnore]
        public Page CurrentPage
        {
            get
            {
                var rootPage = Application.Current.MainPage;
                if (Application.Current.MainPage.GetType().IsSubclassOf(typeof(MasterDetailPage)))
                    rootPage = (Application.Current.MainPage as MasterDetailPage).Detail;

                var navPage = rootPage as NavigationPage;
                if (navPage != null)
                    return navPage.CurrentPage;
                else
                    return rootPage;
            }
        }

        private bool _loading;

        [JsonIgnore]
        public bool IsLoading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;

                if (_loading)
                {
                    _neverLoadBefore = false;
                    ErrorMessage = string.Empty;
                }

                OnPropertyChanged("IsLoading");
                OnPropertyChanged("ShowLoadingView");
                OnPropertyChanged("ShowContent");
            }
        }

        [JsonIgnore]
        public bool ShowLoadingView
        {
            get
            {
                return _loading || !string.IsNullOrEmpty(_errorMessage);
            }
        }

        [JsonIgnore]
        public bool ShowContent
        {
            get
            {
                return !_neverLoadBefore && !ShowLoadingView;
            }
        }

        private string _errorMessage;

        [JsonIgnore]
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                if (!string.IsNullOrEmpty(_errorMessage))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", _errorMessage, "OK");
                        ErrorMessage = string.Empty;
                        IsLoading = false;
                    });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool _neverLoadBefore = true;
        public virtual void Reset()
        {
            IsLoading = false;
            ErrorMessage = string.Empty;
            _neverLoadBefore = true;
        }

        public MasterVM()
        {
            IsLoading = false;
            ErrorMessage = string.Empty;
        }
    }
}
