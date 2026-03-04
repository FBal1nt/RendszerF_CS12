using JegyMester.DataContext.Enums;

namespace JegyMester.DataContext.Entities
{
    public class Error
    {
        public int Id { get; set; }
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime ErrorDateTime { get; set; }
    }
}
