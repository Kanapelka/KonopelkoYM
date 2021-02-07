namespace Athena.Core.Result
{
    public class Result<TPayload>
    {
        public ResultType ResultType { get; set; }
        public TPayload Payload { get; set; }
        public string Message { get; set; }
    }
}