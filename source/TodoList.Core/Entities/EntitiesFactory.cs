namespace TodoList.Core.Entities
{
    public sealed class EntitiesFactory : IEntitiesFactory
    {
        public IItem NewTodo()  
        {
            var todo = new Item();
            return todo;
        }
    }
}