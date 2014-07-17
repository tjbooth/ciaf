using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace Cirrious.Conference.Core.ViewModels
{
    public class MapViewModel
        : BaseViewModel
    {

        public string Name { get { return "Bentley Wildfowl & Motor Museum"; } }
        public string Address { get { return "Halland, East Sussex BN8 5AF"; } }
        public string LocationWebPage { get { return "http://www.chilledinafieldfestival.co.uk/the-location.html"; } }

        public string Phone { get { return "+44 (0)7795 666588"; } }
        public string Email { get { return "it@chilledinafieldfestival.co.uk"; } }
        public double Latitude { get { return 50.923427; } }
        public double Longitude { get { return 0.112631; } }

        public MapViewModel()
        {
        }

        public ICommand PhoneCommand
        {
            get
            {
                return new MvxCommand(() => MakePhoneCall(Name, Phone));
            }
        }

        public ICommand EmailCommand
        {
            get
            {
                return new MvxCommand(() => ComposeEmail(Email, "About Chilled in a Field - sent from the app", "This years Chilled in a Field"));
            }
        }

        public ICommand WebPageCommand
        {
            get
            {
                return new MvxCommand(() => ShowWebPage(LocationWebPage));
            }
        }
    }
}