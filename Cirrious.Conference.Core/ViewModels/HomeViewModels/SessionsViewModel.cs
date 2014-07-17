using System.Windows.Input;
using Cirrious.Conference.Core.ViewModels.SessionLists;
using Cirrious.MvvmCross.ViewModels;

namespace Cirrious.Conference.Core.ViewModels.HomeViewModels
{
    public class SessionsViewModel
        : BaseConferenceViewModel
    {
        public ICommand ShowExhibitorsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<ExhibitionViewModel>()); }
        }

        public ICommand ShowTopicsCommand
        {
            get { return new MvxCommand(() => ShowViewModel<TopicsViewModel>()); }
        }

        public ICommand ShowSpeakersCommand    
        {
            get { return new MvxCommand(() => ShowViewModel<SpeakersViewModel>()); }
        }

        public ICommand ShowDayCommand
        {
            get { return new MvxCommand<string>((day) => ShowViewModel<SessionListViewModel>(new {dayOfMonth = int.Parse(day)})); }
        }

        public ICommand ShowDay1Command
        {
            get { return MakeDayCommand(1); }
        }

        public ICommand ShowDay2Command
        {
            get { return MakeDayCommand(2); }
        }

        public ICommand ShowDay3Command
        {
            get { return MakeDayCommand(3); }
        }

        private ICommand MakeDayCommand(int whichDayOfMonth)
        {
            return new MvxCommand(() => ShowViewModel<SessionListViewModel>(new { dayOfMonth = whichDayOfMonth }));
        }
    }
}