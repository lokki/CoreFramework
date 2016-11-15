﻿using Framework.AspNetCore.Extensions;
using Framework.Common.Extensions;
using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.AspNetCore.HtmlTags
{
    public class TagConventions : HtmlConventionRegistry
    {
        public TagConventions(params IElementBuilderPolicy[] builderPolicies)
        {

            Editors.Always.AddClass("form-control");

            Editors.IfPropertyIs<DateTime?>().ModifyWith(m => m.CurrentTag
                .AddPattern("9{1,2}/9{1,2}/9999")
                .AddPlaceholder("MM/DD/YYYY")
                .AddClass("datepicker")
                .Value(m.Value<DateTime?>() != null ? m.Value<DateTime>().ToShortDateString() : string.Empty));
            Editors.If(er => er.Accessor.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase)).BuildBy(a => new HiddenTag().Value(a.StringValue()));
            Editors.IfPropertyIs<byte[]>().BuildBy(a => new HiddenTag().Value(Convert.ToBase64String(a.Value<byte[]>())));


            Labels.Always.AddClass("control-label");
            Labels.Always.AddClass("col-md-2");
            Labels.ModifyForAttribute<DisplayAttribute>((t, a) => t.Text(a.Name));

            foreach(var policy in builderPolicies)
            {
                Editors.Add(policy);
            }

            DisplayLabels.Always.BuildBy<DefaultDisplayLabelBuilder>();
            DisplayLabels.ModifyForAttribute<DisplayAttribute>((t, a) => t.Text(a.Name));
            Displays.IfPropertyIs<DateTime>().ModifyWith(m => m.CurrentTag.Text(m.Value<DateTime>().ToShortDateString()));
            Displays.IfPropertyIs<DateTime?>().ModifyWith(m => m.CurrentTag.Text(m.Value<DateTime?>() == null ? null : m.Value<DateTime?>().Value.ToShortDateString()));
            Displays.IfPropertyIs<decimal>().ModifyWith(m => m.CurrentTag.Text(m.Value<decimal>().ToString("C")));

            this.Defaults();
        }

        public ElementCategoryExpression DisplayLabels => new ElementCategoryExpression(Library.TagLibrary.Category("DisplayLabels").Profile(TagConstants.Default));
    }
}
