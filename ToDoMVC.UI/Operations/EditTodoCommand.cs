namespace ToDoMVC.UI
{
    #region << Using >>

    using Incoding.CQRS;

    #endregion

    public class EditTodoCommand : CommandBase
    {
        #region Properties

        public string Id { get; set; }

        public string Title { get; set; }

        #endregion

        public override void Execute()
        {
            var todo = Repository.GetById<Todo>(Id);
            todo.Title = Title;
        }
    }
}