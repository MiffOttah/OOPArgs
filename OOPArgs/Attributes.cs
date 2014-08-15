using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPArgs
{
    /// <summary>
    /// Identifies this property as a named argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// The long name of the argument on the command line, defaults to the
        /// name of the property, lowercased.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The short (one-character) name of the argument on the command line,
        /// if it has one. It can be used as an alias in the style /x or -x,
        /// and if using GNU style options, can be combined such as -xyz.
        /// </summary>
        public char Alias { get; set; }

        /// <summary>
        /// The default value assigned to the attribute if the argument is not
        /// specified.
        /// </summary>
        public object DefaultValue { get; set; }

        public ArgumentAttribute()
        {
            this.Name = null;
            this.Alias = '\0';
            this.DefaultValue = null;
        }
    }

    /// <summary>
    /// Identifies this property as a positional argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PositionalArgumentAttribute : Attribute
    {
        /// <summary>
        /// The position that this argument occupies. 0 is the first argument. -1 is the last argument.
        /// </summary>
        public int N { get; set; }

        /// <summary>
        /// The default value assigned to the attribute if the argument is not
        /// specified.
        /// </summary>
        public object DefaultValue { get; set; }

        public PositionalArgumentAttribute(int n)
        {
            this.N = n;
            this.DefaultValue = null;
        }
    }

    /// <summary>
    /// Identifies that this property will be set to an object containing all positional arguments.
    /// The property must be a string[] or an interface string[] implements, such as IEnumerable&lt;string&gt;.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AllPositionalArgumentsAttribute : Attribute
    {
        public AllPositionalArgumentsAttribute()
        {
        }
    }
}
