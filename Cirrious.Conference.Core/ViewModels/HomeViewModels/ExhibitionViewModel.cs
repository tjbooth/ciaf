using System.Linq;

namespace Cirrious.Conference.Core.ViewModels.SessionLists
{
    public class ExhibitionViewModel
        : BaseReloadingSessionListViewModel<string>
    {
        protected override void LoadSessions()
        {
            if (Service.Exhibitors == null)
                return;

            var grouped = Service.Exhibitors
                .Values
                .OrderBy(exhibitors => exhibitors.Name)
                .Select(slot => new SessionGroup(
                                exhibitors.Name,
                                exhibitors
                                    .OrderBy(session => session.Session.When),
                                NavigateToSession));

            GroupedList = grouped.ToList();
        }
    }
}