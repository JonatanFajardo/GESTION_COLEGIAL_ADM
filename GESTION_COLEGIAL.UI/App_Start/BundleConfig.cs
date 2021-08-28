
using System.Web.Optimization;

namespace GESTION_COLEGIAL.UI
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/js/libs/jquery-3.1.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/bundles/datatable/js").Include(
            //            "~/Content/plugins/datatables.net/js/jquery.dataTables.min.js",
            //          "~/Content/plugins/datatables.net-bs4/js/dataTables.bootstrap4.min.js",
            //          "~/Content/plugins/datatables.net-buttons/js/dataTables.buttons.min.js",
            //          "~/Content/plugins/datatables.net-buttons-bs4/js/buttons.bootstrap4.min.js",
            //          "~/Content/plugins/datatables.net-responsive/js/dataTables.responsive.min.js",
            //          "~/Content/plugins/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js",
            //          "~/Content/plugins/datatables/jszip.min.js",
            //          "~/Content/plugins/datatables/pdfmake.min.js",
            //          "~/Content/plugins/datatables/vfs_fonts.js",
            //          "~/Content/plugins/datatables.net-buttons/js/buttons.html5.min.js",
            //          "~/Content/plugins/datatables.net-buttons/js/buttons.print.min.js",
            //          "~/Content/plugins/datatables.net-buttons/js/buttons.colVis.min.js",
            //          "~/Content/js/components/datatable.init.js"
            //          ));


        }
    }
}
