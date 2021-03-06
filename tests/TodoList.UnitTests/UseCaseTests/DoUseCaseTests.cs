namespace TodoList.UseCaseTests.UnitTests
{
    using System.Linq;
    using System;
    using TodoList.Core.Boundaries.Do;
    using TodoList.Core.Exceptions;
    using TodoList.Core.Gateways;
    using TodoList.Core.UseCases;
    using TodoList.Infrastructure.InMemoryGateway;
    using Xunit;

    public sealed class DoUseCaseTests
    {
        string existingTodoItemId = "3b35f11e-7080-45e2-a152-afff5a325508";

        [Fact]
        public void MarkItemCompletedSuccess()
        {
            InMemoryContext inMemory = new InMemoryContext();
            IItemGateway gateway = new InMemoryItemGateway(inMemory);
            IUseCase sut = new Do(gateway);

            sut.Execute(existingTodoItemId);

            Assert.NotEmpty(inMemory.Items.Where(e => e.Id == new Guid(existingTodoItemId) && e.Done));
        }

        [Fact]
        public void MarkItemCompleted_ThrowsException_WhenItemDoesNotExist()
        {
            InMemoryContext inMemory = new InMemoryContext();
            IItemGateway gateway = new InMemoryItemGateway(inMemory);
            IUseCase sut = new Do(gateway);

            Exception ex = Record.Exception(() => sut.Execute(Guid.NewGuid().ToString()));

            Assert.NotNull(ex);
            Assert.IsType<BusinessException>(ex);
        }
    }
}