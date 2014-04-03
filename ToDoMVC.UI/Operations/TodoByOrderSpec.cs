namespace ToDoMVC.UI
{
    using System;
    using Incoding.Data;

    public class TodoByOrderSpec : OrderSpecification<Todo>
    {
        public override Action<AdHocOrderSpecification<Todo>> SortedBy()
        {
            return specification => specification.OrderByDescending(r => r.CreateDt);
        }
    }
}