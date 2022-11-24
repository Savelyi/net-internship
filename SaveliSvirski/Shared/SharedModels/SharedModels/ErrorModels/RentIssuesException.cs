namespace SharedModels.ErrorModels
{
    public class RentIssuesException : Exception
    {
        public RentIssuesException(string message)
            : base(message)
        {
        }
    }
}