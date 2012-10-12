namespace Griffin.Decoupled.Pipeline.Messages
{
    /// <summary>
    /// Pipeline has been started
    /// </summary>
    /// <remarks>Means that the handlers can initialize any startup work.</remarks>
    public class StartHandlers : IDownstreamMessage
    {
    }
}