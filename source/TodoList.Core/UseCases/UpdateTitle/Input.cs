namespace TodoList.Core.UseCases.UpdateTitle {
    using System;

    public sealed class Input {
        internal Input () { }
        public Guid TodoItemId { get; internal set; }
        public string Title { get; internal set; }
    }
}