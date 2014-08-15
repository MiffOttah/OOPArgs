using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPArgs
{
    public class InvalidArgumentsClassException : Exception
    {
        public InvalidArgumentsClassException(string message) : base(message) { }
    }

    public class InvalidArgumentNameException : Exception
    {
        public InvalidArgumentNameException(string name) : base(string.Format("\"{0}\" is an invalid name for an argument.", name)) { }
    }

    public class InvalidArgumentTypeException : Exception
    {
        public InvalidArgumentTypeException(Type type) : base(string.Format("\"{0}\" is an invalid type for an argument.", type.FullName)) { }
    }
}
