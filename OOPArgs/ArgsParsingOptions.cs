using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPArgs
{
    /// <summary>
    /// Options to control ArgsParser.Parse
    /// </summary>
    public sealed class ArgsParsingOptions
    {
        public ArgsParsingOptions()
        {
            this.AllowGnuStyle = true;
            this.AllowDosStyle = true;
            this.AllowBlankArgs = false;
            this.AllowNamedArgsAfterPoistionalArgs = false;
            this.DosStyleValueSeperators = DosStyleValueSeperators.EqualsSign;
            this.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// True to allow Gnu-style arguments (-a for short args, --name for long
        /// args, and - or -- by itself to switch to positional args). Default is true.
        /// </summary>
        public bool AllowGnuStyle { get; set; }

        /// <summary>
        /// True to allow Dos-style arguments  (/a for short args, /name for long
        /// args). Default is true.
        /// </summary>
        public bool AllowDosStyle { get; set; }

        /// <summary>
        /// Whether blank entires in the raw arguments array are treated as positional
        /// arguments (true), or ignored (false). Default is false.
        /// </summary>
        public bool AllowBlankArgs { get; set; }

        /// <summary>
        /// Whether or not to continue parsing named args after positional args are
        /// given. If true, parsing continues, if false, all arguments after the first
        /// positional one are treated as positional. If GNU-style arguments are allowed,
        /// everything after a - or -- will always be treated as potisional. Default is
        /// false.
        /// </summary>
        public bool AllowNamedArgsAfterPoistionalArgs { get; set; }

        /// <summary>
        /// Which sepeartor to use between the key and value of DOS style arguments.
        /// Default is DosStyleValueSeperators.Colon. This option is ignored if
        /// AllowDosStyle is false.
        /// </summary>
        public DosStyleValueSeperators DosStyleValueSeperators { get; set; }

        internal char[] GetDosStyleValueSeperators()
        {
            switch (DosStyleValueSeperators)
            {
                case OOPArgs.DosStyleValueSeperators.EqualsSign: return new char[] { '=' };
                case OOPArgs.DosStyleValueSeperators.Colon: return new char[] { ':' };
                default: return new char[] { '=', ':' };
            }
        }

        /// <summary>
        /// The format provider used to parse command-line arguments such as integers.
        /// Default is invariant culture.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }
    }

    /// <summary>
    /// The character that seperates argument values from their keys in Dos-style
    /// arguments.
    /// </summary>
    [Flags]
    public enum DosStyleValueSeperators
    {
        /// <summary>
        /// An equals sign, as in /key=value
        /// </summary>
        EqualsSign = 1,

        /// <summary>
        /// A colon, as in /key:value
        /// </summary>
        Colon = 2,

        /// <summary>
        /// Either an equals sign, as in /key=value, or a colon, as
        /// in /key:value.
        /// </summary>
        EqualsSignOrColon = 3
    }
}
