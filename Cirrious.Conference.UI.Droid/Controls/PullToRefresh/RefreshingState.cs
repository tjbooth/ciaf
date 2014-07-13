using Android.Views;

namespace Cirrious.Conference.UI.Droid.Controls.PullToRefresh
{
    public class RefreshingState : IScrollingState
    {
        #region IScrollingState Members

        public bool TouchStopped(MotionEvent motionEvent,
                                 PullToRefreshComponent pullToRefreshComponent)
        {
            return true;
        }

        public bool HandleMovement(MotionEvent motionEvent,
                                   PullToRefreshComponent pullToRefreshComponent)
        {
            return true;
        }

        #endregion
    }
}