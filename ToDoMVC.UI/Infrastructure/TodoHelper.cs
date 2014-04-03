namespace ToDoMVC.UI
{
    using System;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;
    using Incoding.Extensions;
    using Incoding.MvcContrib;

    public class TodoHelper
    {
        readonly HtmlHelper helper;

        public TodoHelper(HtmlHelper helper)
        {
            this.helper = helper;
        }

        public class BeginFormSetting
        {
            public string TargetId { get; set; }

            public object Routes { get; set; }
        }

        public BeginTag BeginForm<T>(Action<BeginFormSetting> configure)
        {
            var setting = new BeginFormSetting();
            configure(setting);

            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            return this.helper.When(JqueryBind.Submit)
                       .DoWithPreventDefault()
                       .Submit(options =>
                                   {
                                       options.Url = url.Dispatcher()
                                                        .Push<T>(setting.Routes);
                                   })
                       .OnSuccess(dsl =>
                                      {
                                          dsl.WithId(setting.TargetId).Core().Trigger.Incoding();
                                          dsl.Self().Core().Form.Reset();
                                      })
                       .AsHtmlAttributes()
                       .ToBeginTag(this.helper, HtmlTag.Form);
        }



        public class ContainerSetting
        {
            public string Id { get; set; }

            public string Url { get; set; }

            public string Tmpl { get; set; }

            public string DependencyId { get; set; }
        }

        public MvcHtmlString Container(Action<ContainerSetting> configure)
        {
            var setting = new ContainerSetting();
            configure(setting);

            return helper.When(JqueryBind.InitIncoding | JqueryBind.IncChangeUrl)
                         .Do()
                         .AjaxGet(setting.Url)
                         .OnSuccess(dsl =>
                                        {
                                            dsl.Self().Core().Insert.WithTemplateByUrl(setting.Tmpl).Html();
                                            dsl.WithId(setting.DependencyId).Core().Trigger.Incoding();
                                        })
                         .AsHtmlAttributes(new { id = setting.Id })
                         .ToDiv();
        }


        public class LiHashSetting
        {            
            public bool IsFirst { get; set; }

            public string SelectedClass { get; set; }

            public GetTodoByClientQuery.TypeOfTodo Type { get; set; }
        }

        public MvcHtmlString LiHash(Action<LiHashSetting> configure)
        {
            var setting = new LiHashSetting();
            configure(setting);

            return helper.When(JqueryBind.InitIncoding)
                         .Do()
                         .Direct()
                         .OnSuccess(dsl =>
                                        {
                                            var type = Selector.Incoding.HashQueryString<GetTodoByClientQuery>(r => r.Type);
                                            if (setting.IsFirst)
                                                dsl.Self().Core().JQuery.Attributes.AddClass(setting.SelectedClass).If(s => s.Is(() => type == ""));

                                            dsl.Self().Core().JQuery.Attributes.AddClass(setting.SelectedClass).If(s => s.Is(() => type == setting.Type.ToString()));
                                        })
                         .When(JqueryBind.Click)
                         .Do()
                         .Direct()
                         .OnSuccess(dsl =>
                                        {
                                            dsl.WithSelf(r => r.Closest(HtmlTag.Ul).Find(HtmlTag.A)).Core().JQuery.Attributes.RemoveClass(setting.SelectedClass);
                                            dsl.Self().Core().JQuery.Attributes.AddClass(setting.SelectedClass);
                                        })
                         .AsHtmlAttributes(new { href = "#!".AppendToHashQueryString(new { Type = setting.Type }) })
                         .ToLink(setting.Type.ToString());
        }

        public class VerbSetting
        {
            public string Url { get; set; }

            public Action<IIncodingMetaLanguageCallbackBodyDsl> OnBegin { get; set; }

            public Action<IIncodingMetaLanguageCallbackBodyDsl> OnSuccess { get; set; }

            public object Attr { get; set; }

            public string Content { get; set; }
        }

        public MvcHtmlString Verb(Action<VerbSetting> configure)
        {
            var setting = new VerbSetting();
            configure(setting);

            return this.helper.When(JqueryBind.Click)
                       .Do()
                       .AjaxPost(setting.Url)
                       .OnBegin(dsl =>
                                    {
                                        if (setting.OnBegin != null)
                                            setting.OnBegin(dsl);
                                    })
                       .OnSuccess(dsl =>
                                      {
                                          if (setting.OnSuccess != null)
                                              setting.OnSuccess(dsl);
                                      })
                       .AsHtmlAttributes(setting.Attr)
                       .ToButton(setting.Content);
        }
    }
}