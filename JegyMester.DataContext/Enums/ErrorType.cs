using System;
using System.Collections.Generic;
using System.Text;

namespace JegyMester.DataContext.Enums
{
    public enum ErrorType
    {
        BadRequest,
        DatabaseError,
        Forbidden,
        NotFound,
        ValidationError,
        Other
    }
}
