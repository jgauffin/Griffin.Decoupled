namespace Griffin.Decoupled.Pipeline.Messages
{
    /// <summary>
    /// Shut down the pipeline (try to deliver all messages first)
    /// </summary>
    public class Shutdown : IDownstreamMessage
    {
    }
}