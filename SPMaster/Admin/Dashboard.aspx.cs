using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;


namespace SPMaster.Admin
{
    public partial class Dashboard :
        System.Web.UI.Page
    {
        // =========================================================
        // ACTIVITY GRAPH DATA
        // =========================================================

        protected int[] ActivityCounts =
            new int[7];


        protected string ActivityPolylinePoints
        {
            get;
            private set;
        }


        protected string ActivityPolygonPoints
        {
            get;
            private set;
        }


        protected string ActivityCircles
        {
            get;
            private set;
        }



        // =========================================================
        // Y AXIS VALUES
        // =========================================================

        protected int ActivityYAxisMax
        {
            get;
            private set;
        }


        protected int ActivityYAxisMidHigh
        {
            get;
            private set;
        }


        protected int ActivityYAxisMidLow
        {
            get;
            private set;
        }



        // =========================================================
        // DATABASE CONNECTION
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings[
                        "SPMasterConnectionString"
                    ]
                    .ConnectionString;
            }
        }



        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }



        // =========================================================
        // LOAD DASHBOARD
        // =========================================================

        private void LoadDashboard()
        {
            try
            {
                using (
                    SqlConnection connection =
                        new SqlConnection(
                            ConnectionString
                        )
                )
                {
                    connection.Open();


                    // =============================================
                    // DATABASE CONNECTION SUCCESSFUL
                    // =============================================

                    lblSystemHealth.Text =
                        "System Health: All services operational";


                    lblDatabaseStatus.Text =
                        "Database: Connected";



                    // =============================================
                    // LOAD ALL DASHBOARD DATA
                    // =============================================

                    LoadSummaryCards(
                        connection
                    );


                    LoadActivity(
                        connection
                    );


                    LoadRecentUsers(
                        connection
                    );


                    LoadPendingReports(
                        connection
                    );
                }
            }
            catch (Exception)
            {
                // =============================================
                // DATABASE CONNECTION FAILED
                // =============================================

                lblSystemHealth.Text =
                    "System Health: Database unavailable";


                lblDatabaseStatus.Text =
                    "Database: Connection failed";


                lblTotalUsers.Text =
                    "—";


                lblNewUsersThisWeek.Text =
                    "0";


                lblActiveLecturers.Text =
                    "—";


                lblPendingReports.Text =
                    "—";


                lblDatabaseSize.Text =
                    "—";


                CreateEmptyChart();
            }
        }



        // =========================================================
        // GET CURRENT WEEK START
        //
        // Monday = beginning of week
        // =========================================================

        private DateTime GetWeekStart()
        {
            DateTime today =
                DateTime.Today;


            int daysFromMonday =
                (
                    (int)today.DayOfWeek
                    + 6
                )
                % 7;


            return today.AddDays(
                -daysFromMonday
            );
        }



        // =========================================================
        // LOAD SUMMARY CARDS
        // =========================================================

        private void LoadSummaryCards(
            SqlConnection connection)
        {
            DateTime weekStart =
                GetWeekStart();


            DateTime nextWeek =
                weekStart.AddDays(7);



            // =====================================================
            // TOTAL USERS
            // =====================================================

            const string totalUsersSql = @"
                SELECT
                    COUNT(*)

                FROM Users;
            ";


            using (
                SqlCommand command =
                    new SqlCommand(
                        totalUsersSql,
                        connection
                    )
            )
            {
                int totalUsers =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                lblTotalUsers.Text =
                    totalUsers.ToString(
                        "N0"
                    );
            }



            // =====================================================
            // NEW USERS THIS WEEK
            // =====================================================

            const string newUsersSql = @"
                SELECT
                    COUNT(*)

                FROM Users

                WHERE
                    CreatedAt >= @WeekStart

                    AND

                    CreatedAt < @NextWeek;
            ";


            using (
                SqlCommand command =
                    new SqlCommand(
                        newUsersSql,
                        connection
                    )
            )
            {
                command.Parameters.Add(
                    "@WeekStart",
                    SqlDbType.DateTime2
                ).Value =
                    weekStart;


                command.Parameters.Add(
                    "@NextWeek",
                    SqlDbType.DateTime2
                ).Value =
                    nextWeek;


                int newUsers =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                lblNewUsersThisWeek.Text =
                    newUsers.ToString(
                        "N0"
                    );
            }



            // =====================================================
            // ACTIVE LECTURERS
            //
            // RoleId 2 = Lecturer
            // =====================================================

            const string lecturerSql = @"
                SELECT
                    COUNT(*)

                FROM Users

                WHERE
                    RoleId = 2

                    AND

                    IsActive = 1;
            ";


            using (
                SqlCommand command =
                    new SqlCommand(
                        lecturerSql,
                        connection
                    )
            )
            {
                int activeLecturers =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                lblActiveLecturers.Text =
                    activeLecturers.ToString(
                        "N0"
                    );
            }



            // =====================================================
            // PENDING FEEDBACK REPORTS
            //
            // Pending =
            // Open + Under Review
            // =====================================================

            const string pendingSql = @"
                SELECT
                    COUNT(*)

                FROM Feedback

                WHERE
                    Status IN
                    (
                        'Open',
                        'Under Review'
                    );
            ";


            using (
                SqlCommand command =
                    new SqlCommand(
                        pendingSql,
                        connection
                    )
            )
            {
                int pendingReports =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                lblPendingReports.Text =
                    pendingReports.ToString(
                        "N0"
                    );
            }



            // =====================================================
            // DATABASE SIZE
            // =====================================================

            const string databaseSizeSql = @"
                SELECT

                    SUM(size)
                    * 8.0
                    / 1024.0

                FROM sys.database_files;
            ";


            using (
                SqlCommand command =
                    new SqlCommand(
                        databaseSizeSql,
                        connection
                    )
            )
            {
                object result =
                    command.ExecuteScalar();


                double sizeMb =
                    result == DBNull.Value
                    ? 0
                    : Convert.ToDouble(
                        result
                    );


                if (
                    sizeMb >= 1024
                )
                {
                    lblDatabaseSize.Text =
                        (
                            sizeMb / 1024
                        )
                        .ToString(
                            "0.0"
                        )
                        + " GB";
                }
                else
                {
                    lblDatabaseSize.Text =
                        sizeMb.ToString(
                            "0.0"
                        )
                        + " MB";
                }
            }
        }



        // =========================================================
        // LOAD PLATFORM ACTIVITY
        //
        // Activity =
        // Number of quiz attempts started each day
        //
        // Monday -> Sunday
        // =========================================================

        private void LoadActivity(
            SqlConnection connection)
        {
            DateTime weekStart =
                GetWeekStart();


            DateTime nextWeek =
                weekStart.AddDays(7);



            const string sql = @"
                SELECT

                    CAST(
                        StartedAt
                        AS DATE
                    ) AS ActivityDate,

                    COUNT(*) AS AttemptCount


                FROM QuizAttempts


                WHERE
                    StartedAt >= @WeekStart

                    AND

                    StartedAt < @NextWeek


                GROUP BY

                    CAST(
                        StartedAt
                        AS DATE
                    )


                ORDER BY

                    ActivityDate;
            ";



            using (
                SqlCommand command =
                    new SqlCommand(
                        sql,
                        connection
                    )
            )
            {
                command.Parameters.Add(
                    "@WeekStart",
                    SqlDbType.DateTime2
                ).Value =
                    weekStart;


                command.Parameters.Add(
                    "@NextWeek",
                    SqlDbType.DateTime2
                ).Value =
                    nextWeek;



                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    while (
                        reader.Read()
                    )
                    {
                        DateTime activityDate =
                            Convert.ToDateTime(
                                reader[
                                    "ActivityDate"
                                ]
                            );


                        int attemptCount =
                            Convert.ToInt32(
                                reader[
                                    "AttemptCount"
                                ]
                            );



                        int dayIndex =
                            (
                                activityDate.Date
                                -
                                weekStart.Date
                            )
                            .Days;



                        if (
                            dayIndex >= 0
                            &&
                            dayIndex < 7
                        )
                        {
                            ActivityCounts[
                                dayIndex
                            ] =
                                attemptCount;
                        }
                    }
                }
            }



            BuildActivityChart();
        }



        // =========================================================
        // BUILD ACTIVITY CHART
        // =========================================================

        private void BuildActivityChart()
        {
            int[] xPositions =
            {
                20,
                130,
                240,
                350,
                460,
                570,
                680
            };



            // =====================================================
            // FIND HIGHEST ACTIVITY VALUE
            // =====================================================

            int highestValue =
                0;


            for (
                int i = 0;
                i < ActivityCounts.Length;
                i++
            )
            {
                if (
                    ActivityCounts[i]
                    > highestValue
                )
                {
                    highestValue =
                        ActivityCounts[i];
                }
            }



            // =====================================================
            // CREATE Y AXIS SCALE
            //
            // Minimum maximum is 4 so an empty /
            // low-activity chart still looks meaningful.
            //
            // Round maximum upward to an even number.
            // Example:
            // Highest = 5 -> Max = 6
            // Highest = 6 -> Max = 6
            // Highest = 7 -> Max = 8
            // =====================================================

            ActivityYAxisMax =
                Math.Max(
                    highestValue,
                    4
                );


            if (
                ActivityYAxisMax % 2 != 0
            )
            {
                ActivityYAxisMax++;
            }



            // =====================================================
            // Y AXIS INTERMEDIATE LABELS
            //
            // Example for maximum 6:
            //
            // 6
            // 4
            // 2
            // 0
            // =====================================================

            ActivityYAxisMidHigh =
                (int)Math.Round(
                    ActivityYAxisMax
                    * 2.0
                    / 3.0
                );


            ActivityYAxisMidLow =
                (int)Math.Round(
                    ActivityYAxisMax
                    / 3.0
                );



            // =====================================================
            // BUILD SVG POINTS
            // =====================================================

            StringBuilder linePoints =
                new StringBuilder();


            StringBuilder circles =
                new StringBuilder();



            for (
                int i = 0;
                i < 7;
                i++
            )
            {
                double ratio =
                    (double)
                    ActivityCounts[i]
                    /
                    ActivityYAxisMax;



                // Chart drawing range:
                //
                // Top    = 30
                // Bottom = 150

                double y =
                    150
                    -
                    (
                        ratio
                        * 120
                    );



                if (
                    linePoints.Length > 0
                )
                {
                    linePoints.Append(
                        " "
                    );
                }



                linePoints.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0},{1:0.##}",
                    xPositions[i],
                    y
                );



                circles.AppendFormat(
                    CultureInfo.InvariantCulture,

                    "<circle "
                    +
                    "cx=\"{0}\" "
                    +
                    "cy=\"{1:0.##}\" "
                    +
                    "r=\"5\" "
                    +
                    "fill=\"#9148ff\">"
                    +
                    "</circle>",

                    xPositions[i],
                    y
                );
            }



            ActivityPolylinePoints =
                linePoints.ToString();



            // Close the polygon at the bottom
            // so the purple shaded area is displayed.

            ActivityPolygonPoints =
                ActivityPolylinePoints
                +
                " 680,160 20,160";



            ActivityCircles =
                circles.ToString();
        }



        // =========================================================
        // CREATE EMPTY GRAPH
        // =========================================================

        private void CreateEmptyChart()
        {
            ActivityCounts =
                new int[7];


            BuildActivityChart();
        }



        // =========================================================
        // LOAD RECENT USERS
        // =========================================================

        private void LoadRecentUsers(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT TOP 3

                    U.UserId,

                    U.FullName,

                    U.CreatedAt,

                    R.RoleName


                FROM Users U


                INNER JOIN Roles R

                    ON U.RoleId =
                       R.RoleId


                ORDER BY

                    U.CreatedAt DESC,

                    U.UserId DESC;
            ";



            DataTable table =
                new DataTable();



            using (
                SqlCommand command =
                    new SqlCommand(
                        sql,
                        connection
                    )
            )
            {
                using (
                    SqlDataAdapter adapter =
                        new SqlDataAdapter(
                            command
                        )
                )
                {
                    adapter.Fill(
                        table
                    );
                }
            }



            rptRecentUsers.DataSource =
                table;


            rptRecentUsers.DataBind();



            pnlNoRecentUsers.Visible =
                table.Rows.Count == 0;
        }



        // =========================================================
        // LOAD PENDING FEEDBACK REPORTS
        // =========================================================

        private void LoadPendingReports(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT TOP 3

                    F.FeedbackId,

                    F.Status,

                    F.CreatedAt,

                    CASE

                        WHEN Q.Title IS NULL
                        THEN 'General Feedback'

                        ELSE Q.Title

                    END AS DisplayTitle


                FROM Feedback F


                LEFT JOIN Quizzes Q

                    ON F.QuizId =
                       Q.QuizId


                WHERE

                    F.Status IN
                    (
                        'Open',
                        'Under Review'
                    )


                ORDER BY

                    F.CreatedAt DESC;
            ";



            DataTable table =
                new DataTable();



            using (
                SqlCommand command =
                    new SqlCommand(
                        sql,
                        connection
                    )
            )
            {
                using (
                    SqlDataAdapter adapter =
                        new SqlDataAdapter(
                            command
                        )
                )
                {
                    adapter.Fill(
                        table
                    );
                }
            }



            rptPendingReports.DataSource =
                table;


            rptPendingReports.DataBind();



            pnlNoPendingReports.Visible =
                table.Rows.Count == 0;
        }



        // =========================================================
        // RECENT USER DATE DISPLAY
        // =========================================================

        protected string GetJoinedText(
            object createdAtValue)
        {
            DateTime createdAt =
                Convert.ToDateTime(
                    createdAtValue
                );


            DateTime today =
                DateTime.Today;



            if (
                createdAt.Date
                ==
                today
            )
            {
                return
                    "joined today";
            }



            if (
                createdAt.Date
                ==
                today.AddDays(-1)
            )
            {
                return
                    "joined yesterday";
            }



            return
                "joined "
                +
                createdAt.ToString(
                    "dd MMM yyyy"
                );
        }



        // =========================================================
        // ROLE BADGE CSS
        // =========================================================

        protected string GetRoleBadgeClass(
            object roleValue)
        {
            string role =
                Convert.ToString(
                    roleValue
                );



            if (
                role.Equals(
                    "Student",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return
                    "role-badge student-badge";
            }



            if (
                role.Equals(
                    "Lecturer",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return
                    "role-badge lecturer-badge";
            }



            return
                "role-badge admin-badge";
        }



        // =========================================================
        // REPORT STATUS CSS
        // =========================================================

        protected string GetReportStatusClass(
            object statusValue)
        {
            string status =
                Convert.ToString(
                    statusValue
                );



            if (
                status.Equals(
                    "Under Review",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return
                    "report-tag under-review-report";
            }



            return
                "report-tag open-report";
        }
    }
}