namespace ToDoMVC.UI
{
    using System.Web.Mvc;
    

    public static class HtmlExtensions
    {
         public static TodoHelper Todo(this HtmlHelper helper )
         {
             return new TodoHelper(helper);
         }
    }
}