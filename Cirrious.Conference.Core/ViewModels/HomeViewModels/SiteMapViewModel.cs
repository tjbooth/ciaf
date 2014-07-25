using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace Cirrious.Conference.Core.ViewModels
{
    public class SiteMapViewModel
        : BaseViewModel
    {

        public string Title { get { return "Site Map"; } }
        public string Subtitle { get { return "Chilled in a Field"; } }


        public SiteMapViewModel()
        {
        }

    }
}