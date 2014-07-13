using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.Conference.Core.ViewModels.SessionLists;

namespace Cirrious.Conference.Core.ViewModels.HomeViewModels
{
    public class WelcomeViewModel
        : BaseConferenceViewModel
    {
        public ICommand ShowSeasonsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<HomeViewModel>()); }
        }

        public ICommand ShowTopicsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<TopicsViewModel>()); }
        }

        public ICommand ShowSpeakersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<SpeakersViewModel>()); }
        }

        public ICommand ShowSponsorsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<SponsorsViewModel>()); }
        }

        public ICommand ShowExhibitionCommand
        {
            get { return new MvxCommand(() => ShowViewModel<ExhibitionViewModel>()); }
        }

        public ICommand ShowMapCommand
        {
            get { return new MvxCommand(() => ShowViewModel<MapViewModel>()); }
        }

        public ICommand ShowAboutCommand
        {
            get { return new MvxCommand(() => ShowViewModel<AboutViewModel>()); }
        }
    }
}