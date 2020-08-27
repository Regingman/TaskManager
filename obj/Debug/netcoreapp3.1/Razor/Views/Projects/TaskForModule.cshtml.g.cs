#pragma checksum "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0d08ffe2c07d9df40479de5aa59514ed0242e227"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Projects_TaskForModule), @"mvc.1.0.view", @"/Views/Projects/TaskForModule.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\_ViewImports.cshtml"
using TaskManager;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\_ViewImports.cshtml"
using TaskManager.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0d08ffe2c07d9df40479de5aa59514ed0242e227", @"/Views/Projects/TaskForModule.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cc035caf78427af9c0e6c60ac75693646a0d7e84", @"/Views/_ViewImports.cshtml")]
    public class Views_Projects_TaskForModule : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<TaskManager.Data.Task>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "ModuleForProject", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
  
    ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Задачи в модуле ");
#nullable restore
#line 7 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(ViewBag.ModuleName);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</h1>

<table class=""table table-hover"">
    <thead>
        <tr>
            <th>
                Описание
            </th>

            <th>
                Пользователь
            </th>
            <th>
                Статус
            </th>

            <th>
                Причина невыполнения
            </th>
            <th>
                Срок выполнения
            </th>

            <th>
                Дата создания
            </th>

            <th>
                Дата обновления
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
");
#nullable restore
#line 41 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
         foreach (var item in @Model)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 45 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 48 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.ApplicationUser.UserName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 51 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.Status.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 54 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.Note));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n\r\n                <td>\r\n                    ");
#nullable restore
#line 58 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.DateOfBirth));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 61 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.CreateDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 64 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
               Write(Html.DisplayFor(modelItem => item.UpdateDate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n\r\n            </tr>\r\n");
#nullable restore
#line 68 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n\r\n\r\n\r\n<div>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0d08ffe2c07d9df40479de5aa59514ed0242e2277729", async() => {
                WriteLiteral("Назад");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 75 "E:\programming\project\ProjectGit\asp net\twoVersion\TaskManager\Views\Projects\TaskForModule.cshtml"
                                       WriteLiteral(ViewBag.ProjectId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</div>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<TaskManager.Data.Task>> Html { get; private set; }
    }
}
#pragma warning restore 1591
