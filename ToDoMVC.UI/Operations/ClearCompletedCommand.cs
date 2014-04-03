namespace ToDoMVC.UI
{
    #region << Using >>

    using Incoding.CQRS;
    using Incoding.Extensions;

    #endregion

    public class ClearCompletedCommand : CommandBase
    {
        #region Properties

        public string ClientId { get; set; }

        #endregion

        public override void Execute()
        {
            foreach (var todo in Repository.Query(whereSpecification: new TodoByClientWhereSpec(ClientId)
                                                          .And(new TodoOnlyActiveWhereSpec())))
                Repository.Delete(todo);
        }
    }
}