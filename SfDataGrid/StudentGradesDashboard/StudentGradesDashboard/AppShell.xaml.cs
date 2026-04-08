using StudentGradesDashboard.Views;

namespace StudentGradesDashboard
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("student-detail", typeof(StudentDetailPage));
        }
    }
}
