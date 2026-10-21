using System;


namespace SPMaster.Master
{
    public partial class Portal :
        System.Web.UI.MasterPage
    {
        protected string CurrentRole
        {
            get
            {
                if (Session["Role"] == null)
                {
                    return "";
                }

                return Session["Role"].ToString().Trim();
            }
        }

        protected bool IsLecturer
        {
            get
            {
                return CurrentRole.Equals("Lecturer",StringComparison.OrdinalIgnoreCase);
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
                if (Session["FullName"] == null)
                {
                    return "User";
                }

                return Session["FullName"].ToString().Trim();
            }
        }

        protected string DisplayRole
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CurrentRole))
                {
                    return "User";
                }

                return CurrentRole;
            }
        }

        protected string UserInitials
        {
            get
            {
                return GetInitials(DisplayName);
            }
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "U";
            }

            string[] parts = fullName.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpper();
            }

            string first = parts[0].Substring(0, 1);
            string last = parts[parts.Length - 1].Substring(0, 1);
            return (first + last).ToUpper();

        }
    }
}