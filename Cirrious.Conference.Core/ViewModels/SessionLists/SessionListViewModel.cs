using System;
using System.Linq;

namespace Cirrious.Conference.Core.ViewModels.SessionLists
{
    public class SessionListViewModel
        : BaseReloadingSessionListViewModel<DateTime>
    {
        private int _dayOfMonth;

        public void Init(int dayOfMonth)
        {
            _dayOfMonth = dayOfMonth;
            Title = DayFrom(_dayOfMonth);            
        }

        protected override void LoadSessions()
        {
            if (Service.Sessions == null)
                return;

            var grouped = Service.Sessions
                .Values
                .Where(slot => slot.Session.Day == _dayOfMonth)
                .GroupBy(slot => slot.Session.When)
                .OrderBy(slot => slot.Key)
                .Select(slot => new SessionGroup(
                                    slot.Key,
                                    slot.OrderBy(session => session.Session.Title),
                                    NavigateToSession));

            GroupedList = grouped.ToList();
        }

        private static string DayFrom(int dayOfMonth)
        {
            string day;
            switch (dayOfMonth)
            {
                case 1:
                    day = "Friday";
                    break;
                case 2:
                    day = "Saturday";
                    break;
                case 3:
                default:
                    day = "Sunday";
                    break;
            }
            return day;
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(() => Title); }
        }
    }
}