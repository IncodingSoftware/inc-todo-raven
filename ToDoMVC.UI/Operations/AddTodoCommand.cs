namespace ToDoMVC.UI
{
    using System;
    using Incoding.CQRS;

    public class AddTodoCommand : CommandBase
    {
        public string Title { get; set; }

        public string ClientId { get; set; }

        public override void Execute()
        {
            Repository.Save(new Todo
                                {
                                        Active = false,
                                        ClientId = ClientId,
                                        Title = Title,
                                        CreateDt = DateTime.Now
                                });
        }
    }
}