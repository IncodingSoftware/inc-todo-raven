namespace ToDoMVC.UI
{
    #region << Using >>

    using Incoding.CQRS;

    #endregion

    public class ToggleTodoCommand : CommandBase
    {
        #region Properties

        public string Id { get; set; }

        #endregion

        public override void Execute()
        {
            var todo = Repository.GetById<Todo>(Id);
            todo.Active = !todo.Active;
        }
    }
}