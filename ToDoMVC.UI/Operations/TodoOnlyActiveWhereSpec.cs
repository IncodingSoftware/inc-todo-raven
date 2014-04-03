namespace ToDoMVC.UI
{
    #region << Using >>

    using System;
    using System.Linq.Expressions;
    using Incoding;

    #endregion

    public class TodoOnlyActiveWhereSpec : Specification<Todo>
    {
        public override Expression<Func<Todo, bool>> IsSatisfiedBy()
        {
            return todo => todo.Active;
        }
    }
}