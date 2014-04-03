namespace ToDoMVC.UI
{
    using System;
    using System.Linq.Expressions;
    using Incoding;

    public class TodoByTypeWhereSpec : Specification<Todo>
    {
        #region Fields

        readonly GetTodoByClientQuery.TypeOfTodo type;

        #endregion

        #region Constructors

        public TodoByTypeWhereSpec(GetTodoByClientQuery.TypeOfTodo type)
        {
            this.type = type;
        }

        #endregion

        public override Expression<Func<Todo, bool>> IsSatisfiedBy()
        {
            switch (this.type)
            {
                case GetTodoByClientQuery.TypeOfTodo.All:
                    return null;
                case GetTodoByClientQuery.TypeOfTodo.Active:
                    return todo => !todo.Active;
                case GetTodoByClientQuery.TypeOfTodo.Completed:
                    return todo => todo.Active;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}