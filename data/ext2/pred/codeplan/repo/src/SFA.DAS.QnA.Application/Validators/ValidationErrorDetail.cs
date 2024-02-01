namespace SFA.DAS.QnA.Application.Validators
{
    public class ValidationErrorDetail
    {
        public ValidationErrorDetail()
        {
        }

        public ValidationErrorDetail(string field, string errorMessage)
        {
            Field = field;
            ErrorMessage = errorMessage;
        }

        public ValidationErrorDetail(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string Field { get; set; }
        public string ErrorMessage { get; set; }

    }
}
