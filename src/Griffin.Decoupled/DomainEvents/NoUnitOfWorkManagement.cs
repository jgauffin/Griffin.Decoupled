namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Class that you can use to disable UoW/Transaction management.
    /// </summary>
    public class NoUnitOfWorkManagement : IUnitOfWorkAdapter
    {
        public void Register(IUnitOfWorkObserver observer)
        {
            // simply ignore transactions
        }
    }
}