using System.Web;
using System.Web.Optimization;

namespace AnonymousBidder
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                                        "~/Scripts/DefaultMVC/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                                        "~/Scripts/DefaultMVC/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                                        "~/Scripts/DefaultMVC/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                                          "~/Scripts/DefaultMVC/bootstrap.js",
                                          "~/Scripts/DefaultMVC/bootstrap-datepicker.js",
                                          "~/Scripts/DefaultMVC/respond.js"));

            bundles.Add(new StyleBundle("~/Style/css").Include(
                                        "~/Content/CSS/DefaultMVC/bootstrap.css",
                                         "~/Content/CSS/DefaultMVC/datepicker.css",
                                        "~/Content/CSS/Custom/Signin.css",
                                        "~/Content/CSS/Custom/vertical-tabs.css",
                                        "~/Content/CSS/Custom/EIP.css",
                                        "~/Content/CSS/Custom/fullcalendar.css",
                                          "~/Content/CSS/Custom/jquery-ui.min.css",
                                          "~/Content/CSS/Custom/jquery.contextMenu.css",
                                          "~/Content/CSS/Custom/dataTables.bootstrap.css"));

            bundles.Add(new StyleBundle("~/Style/CSS/DefaultMVC").Include(
                                        "~/Content/CSS/DefaultMVC/bootstrap.css",
                                        "~/Content/CSS/DefaultMVC/datepicker.css",
                                         "~/Content/CSS/Custom/Signin.css",

                                        "~/Content/CSS/DefaultMVC/bootstrap-theme.css"));
            bundles.Add(new ScriptBundle("~/bundles/momentjs").Include(
                                       "~/Scripts/Custom/moment.min.js",
                                       "~/Scripts/Custom/dateFormat.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/AddCommaToDecimal").Include(
                                       "~/Scripts/Custom/AddCommaToDecimal.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Common").Include(
                                     "~/Scripts/Custom/Util.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
              "~/Scripts/kendo/kendo.all.min.js",
                // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
              "~/Scripts/kendo/kendo.aspnetmvc.min.js",
              "~/Scripts/kendo/cultures/kendo.culture.en-SG.min.js"));

            //Please reference to this link for styling: http://docs.telerik.com/kendo-ui/styles-and-layout/appearance-styling#common-css-files
            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
               "~/Content/CSS/kendo/kendo.common-material.min.css",
               "~/Content/CSS/kendo/kendo.default.min.css",
               "~/Content/CSS/kendo/kendo.default.mobile.min.css"
               ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
