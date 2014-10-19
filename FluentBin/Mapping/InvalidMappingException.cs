using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBin.Mapping
{
    public class InvalidMappingException : ApplicationException
    {
        public InvalidMappingException()
        {
            
        }

        public InvalidMappingException(string message) 
            : base(message)
        {
            
        }
    }
}
