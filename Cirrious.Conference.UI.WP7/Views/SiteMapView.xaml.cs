using Cirrious.Conference.Core.ViewModels;

namespace Cirrious.Conference.UI.WP7.Views
{
    public abstract class BaseSiteMapView : BaseView<SiteMapViewModel>
    {
    }

    public partial class SiteMapView : BaseSiteMapView
    {
        public SiteMapView()
        {
            InitializeComponent();
        }
    }
}