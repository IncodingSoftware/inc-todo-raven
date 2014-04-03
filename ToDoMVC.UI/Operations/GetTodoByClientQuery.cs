namespace ToDoMVC.UI
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Incoding.CQRS;
    using Incoding.Extensions;

    #endregion

    public class GetTodoByClientQuery : QueryBase<List<GetTodoByClientQuery.Response>>
    {
        #region Properties

        public string ClientId { get; set; }

        public TypeOfTodo Type { get; set; }

        #endregion

        #region Nested classes

        public class Tmpl
        {
            #region Properties

            public string FooterId { get; set; }

            #endregion
        }

        public class Response
        {
            #region Properties

            public string Id { get; set; }

            public string Title { get; set; }

            public bool Active { get; set; }

            #endregion
        }

        #endregion

        #region Enums

        public enum TypeOfTodo
        {
            All, 

            Active, 

            Completed
        }

        #endregion

        protected override List<Response> ExecuteResult()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                ClientId = Guid.NewGuid().ToString();
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(CookieManager.ClientId, ClientId));
            }

            return Repository.Query(whereSpecification: new TodoByClientWhereSpec(ClientId)
                                            .And(new TodoByTypeWhereSpec(Type)),
                                    orderSpecification: new TodoByOrderSpec())
                             .ToList()
                             .Select(r => new Response
                                              {
                                                      Id = r.Id.ToString(),
                                                      Title = r.Title,
                                                      Active = r.Active
                                              })
                             .ToList();
        }
    }
}