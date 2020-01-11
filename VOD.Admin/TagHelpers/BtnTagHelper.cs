using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace VOD.Admin.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("btn")]
    public class BtnTagHelper : AnchorTagHelper
    {
        public string Icon { get; set; } = string.Empty;

        const string btnPrimary = "btn-primary";
        const string btnDanger = "btn-danger";
        const string btnDefault = "btn-default";
        const string btnInfo = "btn-info";
        const string btnSuccess = "btn-success";
        const string btnWarning = "btn-warning";
        // Google's Material Icons provider name
        const string iconProvider = "material-icons";

        public BtnTagHelper(IHtmlGenerator generator) : base(generator) { }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            output.TagName = "a";




            base.Process(context, output);
        }
    }
}
