namespace ToDoMVC.UI
{
    using System;
    using System.Linq.Expressions;
    using Incoding;

    public class TodoByClientWhereSpec : Specification<Todo>
    {
        readonly string clientId;

        public TodoByClientWhereSpec(string clientId)
        {
            this.clientId = clientId;
        }

        public override Expression<Func<Todo, bool>> IsSatisfiedBy()
        {
            return todo => todo.ClientId == this.clientId;
        }
    }
}