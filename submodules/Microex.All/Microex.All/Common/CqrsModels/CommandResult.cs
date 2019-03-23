namespace Microex.All.Common.CqrsModels
{
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class CommandResult<T>:CommandResult
    {
        public T Data { get; set; }
    }
}
