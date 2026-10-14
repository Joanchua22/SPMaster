using System;

namespace SPMaster
{
    public partial class Dashboard : System.Web.UI.MasterPage
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            string currentPath =
                Request
                .AppRelativeCurrentExecutionFilePath
                .ToLowerInvariant();


            /*
             * Only these three main Admin pages
             * should NOT display the white top header.
             *
             * Admin Dashboard keeps the top header.
             * Student and Lecturer pages also keep it.
             */
            bool hideTopBar =
                currentPath ==
                    "~/admin/usermanagement.aspx"

                || currentPath ==
                    "~/admin/contentaudit.aspx"

                || currentPath ==
                    "~/admin/feedbackreports.aspx";


            if (hideTopBar)
            {
                pageBody.Attributes["class"] =
                    "admin-no-topbar";
            }
            else
            {
                pageBody.Attributes.Remove(
                    "class"
                );
            }
        }
    }
}