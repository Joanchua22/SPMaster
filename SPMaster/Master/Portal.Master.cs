using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SPMaster.Master
{
    public partial class Portal : System.Web.UI.MasterPage
    {
        protected bool IsLecturer
        {
            get
            {
                return Request.AppRelativeCurrentExecutionFilePath
                    .StartsWith(
                        "~/Lecturer/",
                        StringComparison.OrdinalIgnoreCase
                    );
            }
        }

        protected string PortalFolder
        {
            get
            {
                return IsLecturer ? "~/Lecturer/" : "~/Student/";
            }
        }

        protected string DisplayName
        {
            get
            {
                return IsLecturer ? "Mr. Rahman" : "Aina Lee";
            }
        }

        protected string UserInitials
        {
            get
            {
                return IsLecturer ? "MR" : "AL";
            }
        }

        protected string DisplayRole
        {
            get
            {
                return IsLecturer ? "Lecturer" : "Student";
            }
        }
    }
}