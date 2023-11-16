using System.Web.Optimization;

namespace MMS_Models
{
    public class BundleConfig
    {
                 public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));


                                      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
            "~/Scripts/jquery.inputmask/inputmask.js",
            "~/Scripts/jquery.inputmask/jquery.inputmask.js",
            "~/Scripts/jquery.inputmask/inputmask.extensions.js",
            "~/Scripts/jquery.inputmask/inputmask.date.extensions.js",
                             "~/Scripts/jquery.inputmask/inputmask.numeric.extensions.js"


            ));

            bundles.Add(new ScriptBundle("~/bundles/allScripts").Include(
                "~/Scripts/plugins/jquery/jquery-1.11.1.min.js",
                "~/Scripts/plugins/jquery/jquery-ui-1.10.1.custom.min.js",
                "~/Scripts/plugins/jquery/jquery-migrate-1.1.1.min.js",
                "~/Scripts/plugins/jquery/globalize.js",
                "~/Scripts/plugins/other/excanvas.js",
                "~/Scripts/plugins/other/jquery.mousewheel.min.js",
                "~/Scripts/plugins/bootstrap/bootstrap.min.js",
                "~/Scripts/plugins/cookies/jquery.cookies.2.2.0.min.js",
                "~/Scripts/plugins/fancybox/jquery.fancybox.pack.js",
                "~/Scripts/plugins/jflot/jquery.flot.js",
                "~/Scripts/plugins/jflot/jquery.flot.stack.js",
                "~/Scripts/plugins/jflot/jquery.flot.pie.js",
                "~/Scripts/plugins/jflot/jquery.flot.resize.js",
                "~/Scripts/plugins/epiechart/jquery.easy-pie-chart.js",
                "~/Scripts/plugins/knob/jquery.knob.js",
                "~/Scripts/plugins/sparklines/jquery.sparkline.min.js",
                "~/Scripts/plugins/pnotify/jquery.pnotify.min.js",
                "~/Scripts/plugins/fullcalendar/fullcalendar.min.js",
                "~/Scripts/plugins/wookmark/jquery.wookmark.js",
                "~/Scripts/plugins/jbreadcrumb/jquery.jBreadCrumb.1.1.js",
                "~/Scripts/plugins/mcustomscrollbar/jquery.mCustomScrollbar.min.js",
                "~/Scripts/plugins/uniform/jquery.uniform.min.js",
                "~/Scripts/plugins/select/select2.min.js",
                "~/Scripts/plugins/tagsinput/jquery.tagsinput.min.js",
                "~/Scripts/plugins/maskedinput/jquery.maskedinput-1.3.min.js",
                "~/Scripts/plugins/multiselect/jquery.multi-select.min.js",
                "~/Scripts/plugins/validationEngine/languages/jquery.validationEngine-en.js",
                "~/Scripts/plugins/validationEngine/jquery.validationEngine.js",
                "~/Scripts/plugins/stepywizard/jquery.stepy.js",
                "~/Scripts/plugins/animatedprogressbar/animated_progressbar.js",
                "~/Scripts/plugins/hoverintent/jquery.hoverIntent.minified.js",
                "~/Scripts/plugins/media/mediaelement-and-player.min.js",
                "~/Scripts/plugins/cleditor/jquery.cleditor.js",
                "~/Scripts/plugins/shbrush/XRegExp.js",
                "~/Scripts/plugins/shbrush/shCore.js",
                "~/Scripts/plugins/shbrush/shBrushXml.js",
                "~/Scripts/plugins/shbrush/shBrushJScript.js",
                "~/Scripts/plugins/shbrush/shBrushCss.js",
                "~/Scripts/plugins/filetree/jqueryFileTree.js",
                "~/Scripts/plugins/slidernav/slidernav-min.js",
                "~/Scripts/plugins/isotope/jquery.isotope.min.js",
                "~/Scripts/plugins/jnotes/jquery-notes_1.0.8_min.js",
                "~/Scripts/plugins/jcrop/jquery.Jcrop.min.js",
                "~/Scripts/plugins/ibutton/jquery.ibutton.min.js",
                "~/Scripts/plugins/scrollup/jquery.scrollUp.min.js",
                "~/Scripts/plugins.js",
                "~/Scripts/charts.js",
                "~/Scripts/actions.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/MazenMainJava.js",
                "~/Scripts/plugins/datatables/jquery.dataTables.min.js",
                "~/Scripts/plugins/datatables/jquery.jeditable.js",
                "~/Scripts/plugins/datatables/jquery.dataTables.editable.js",
                "~/Scripts/plugins/datatables/dataTables.tableTools.min.js"
                ));

        }
    }
}




