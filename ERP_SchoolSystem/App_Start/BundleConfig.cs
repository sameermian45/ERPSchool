using System.Web;
using System.Web.Optimization;

namespace ERP_SchoolSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/JS").Include(
                        "~/Content/Loader/toastr.min.js",
                        "~/Content/Loader/loader.js",
                        //Core
                        "~/Content/JS/main/jquery.min.js",
                        "~/Content/JS/main/bootstrap.bundle.min.js",
                        "~/Content/JS/plugins/loaders/blockui.min.js",
                        "~/Content/JS/plugins/ui/ripple.min.js",
                        //Datatable
                        "~/Content/JS/plugins/tables/datatables/datatables.min.js",
                        //wizards
                        "~/Content/JS/plugins/forms/wizards/steps.min.js",
                        "~/Content/JS/plugins/forms/styling/uniform.min.js",
                        "~/Content/JS/plugins/forms/inputs/inputmask.js",
                        "~/Content/JS/plugins/forms/validation/validate.min.js",
                        "~/Content/JS/plugins/extensions/cookie.js",
                         //pickadate
                         "~/Content/JS/plugins/ui/moment/moment.min.js",
                        "~/Content/JS/plugins/pickers/daterangepicker.js",
                        "~/Content/JS/plugins/pickers/anytime.min.js",
                        "~/Content/JS/plugins/pickers/pickadate/picker.js",
                        "~/Content/JS/plugins/pickers/pickadate/picker.date.js",
                        "~/Content/JS/plugins/pickers/pickadate/picker.time.js",
                        "~/Content/JS/plugins/pickers/pickadate/legacy.js",
                        "~/Content/JS/plugins/notifications/jgrowl.min.js",



                        //select2
                        "~/Content/JS/plugins/forms/selects/select2.min.js",
                        //charts
                        "~/Content/JS/plugins/visualization/d3/d3.min.js",
                        "~/Content/JS/plugins/visualization/d3/d3_tooltip.js",
                        "~/Content/JS/plugins/forms/styling/switchery.min.js",
                        "~/Content/JS/plugins/forms/selects/bootstrap_multiselect.js",
                        "~/Content/JS/plugins/forms/styling/switch.min.js",
                        "~/Content/JS/plugins/extensions/jquery_ui/core.min.js",
                        "~/Content/JS/plugins/forms/inputs/typeahead/typeahead.bundle.min.js",
                        "~/Content/JS/plugins/forms/tags/tagsinput.min.js",
                        "~/Content/JS/plugins/forms/tags/tokenfield.min.js",
                        "~/Content/JS/plugins/forms/inputs/touchspin.min.js",
                        "~/Content/JS/plugins/forms/inputs/maxlength.min.js",
                        "~/Content/JS/plugins/forms/inputs/formatter.min.js",
                        "~/Content/JS/plugins/notifications/pnotify.min.js",
                        "~/Content/JS/plugins/ui/prism.min.js",
                        "~/Content/JS/plugins/extensions/jquery_ui/interactions.min.js",


                        "~/Content/JS/app.js",
                        "~/Content/JS/demo_pages/datatables_basic.js",
                        "~/Content/JS/demo_pages/form_select2.js",
                       "~/Content/JS/demo_pages/form_wizard.js",
                        "~/Content/JS/JS/demo_pages/form_multiselect.js",
                        "~/Content/JS/JS/demo_pages/form_tags_input.js",
                        "~/Content/JS/demo_pages/form_validation.js",
                        "~/Content/JS/demo_pages/form_input_groups.js",
                        "~/Content/JS/demo_pages/form_actions.js",
                        "~/Content/JS/demo_pages/form_checkboxes_radios.js",
                        "~/Content/JS/demo_pages/form_floating_labels.js",
                        "~/Content/JS/demo_pages/picker_date.js",

                        "~/Content/JS/demo_pages/form_layouts.js",
                        "~/Content/JS/demo_pages/dashboard.js"));



            bundles.Add(new StyleBundle("~/Content/CSS").Include(
                "~/Content/CSS/styles.min.css",
                "~/Content/CSS/bootstrap.min.css",
                "~/Content/CSS/bootstrap_limitless.min.css",
                "~/Content/CSS/layout.min.css",
                "~/Content/CSS/components.min.css",
                "~/Content/Loader/Loader.css",
                "~/Content/Loader/spinner.css",
                "~/Content/Loader/toastr.min.css",
                "~/Content/CSS/colors.min.css"));
        }
    }
}
