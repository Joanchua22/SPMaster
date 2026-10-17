using System;

namespace SPMaster
{
    public partial class Dashboard :
        System.Web.UI.MasterPage
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            // Shared dashboard header is displayed
            // on every dashboard page.
        }
    }
}