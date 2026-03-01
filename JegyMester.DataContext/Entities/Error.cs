using JegyMester.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
