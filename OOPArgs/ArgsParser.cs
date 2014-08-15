using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPArgs
{
    /// <summary>
    /// Parses argument strings into argumetns objects.
    /// </summary>
    public sealed class ArgsParser
    {
        private Dictionary<char, string> m_ShortArgs;
        private Dictionary<string, string> m_LongArgs;
        private List<string> m_PositionalArgs;

        private ArgsParser()
        {
            m_ShortArgs = new Dictionary<char, string>();
            m_LongArgs = new Dictionary<string, string>();
            m_PositionalArgs = new List<string>();
        }

        private void AddKeyValue(string key, string value)
        {
            if (key.Length == 1)
            {
                m_ShortArgs[key[0]] = value;
            }
            else if (key.Length > 1)
            {
                m_LongArgs[key] = value;
            }
        }

        /// <summary>
        /// Parses the string array, such as passed to the Main() function,
        /// and fills any matching properties or fields with
        /// ArgumentAttributes in a new T object.
        /// </summary>
        public static T Parse<T>(string[] rawArgs, ArgsParsingOptions parsingOptions = null) where T : new()
        {
            // default options if none provided
            if (parsingOptions == null)
            {
                parsingOptions = new ArgsParsingOptions();
            }

            //////////
            // step 1: parse rawArgs

            var parser = new ArgsParser();

            // get the permitted dos-style value seperators
            var dosValueSeperators = parsingOptions.GetDosStyleValueSeperators();

            // once set to false, no longer parses named arguments
            bool allowNamedArgs = true;

            foreach (string s in rawArgs)
            {
                // skip blank args if configured to
                if (string.IsNullOrEmpty(s) && !parsingOptions.AllowBlankArgs) continue;

                bool handleAsPositionalArgs = true;

                if (allowNamedArgs && s.Length >= 1)
                {
                    if (parsingOptions.AllowDosStyle && s[0] == '/') // dos-style argument
                    {
                        string k, v;
                        SplitOn(s.Substring(1), out k, out v, dosValueSeperators);
                        parser.AddKeyValue(k, v);

                        handleAsPositionalArgs = false;
                    }
                    else if (parsingOptions.AllowGnuStyle && s[0] == '-') // gnu-style argument
                    {
                        if (s == "-" || s == "--")
                        {
                            // force all remaining args to be positional
                            allowNamedArgs = false;
                            handleAsPositionalArgs = false;
                        }
                        else if (s[1] == '-') // s.Length must be > 1 becuse s != "-" && s[0] == '-'
                        {
                            string k, v;
                            SplitOn(s.Substring(2), out k, out v, '=');
                            parser.AddKeyValue(k, v);

                            handleAsPositionalArgs = false;
                        }
                        else // series of short arguments
                        {
                            string k, v;
                            SplitOn(s.Substring(1), out k, out v, '=');

                            if (string.IsNullOrEmpty(v))
                            {
                                // series of short arguments, like -a -b -c
                                foreach (char c in k)
                                {
                                    parser.AddKeyValue(c.ToString(), null);
                                }
                            }
                            else
                            {
                                // single argument, like --abc=...
                                parser.AddKeyValue(k, v);
                            }

                            handleAsPositionalArgs = false;
                        }
                    }
                }

                if (handleAsPositionalArgs)
                {
                    parser.m_PositionalArgs.Add(s);

                    if (allowNamedArgs && !parsingOptions.AllowNamedArgsAfterPoistionalArgs)
                    {
                        // don't allow any more named arguments now that we're parsing positional args
                        allowNamedArgs = false;
                    }
                }
            }

            //////////
            // step 2: apply the parsed arguments to the final object

            var args = new T();
            var argsType = args.GetType();
            
            foreach (PropertyInfo prop in argsType.GetProperties())
            {
                var argumentAttributes = prop.GetCustomAttributes<ArgumentAttribute>().ToArray();
                var positionalArgumentAttributes = prop.GetCustomAttributes<PositionalArgumentAttribute>().ToArray();
                var allPositionalsAttributes = prop.GetCustomAttributes<AllPositionalArgumentsAttribute>().ToArray();

                if ((argumentAttributes.Length + positionalArgumentAttributes.Length + allPositionalsAttributes.Length) > 1)
                {
                    throw new InvalidArgumentsClassException(string.Format("{0}: Multiple/conflicting attributes on {1}", argsType.FullName, prop.Name));
                }
                else if (argumentAttributes.Length == 1)
                {
                    var name = argumentAttributes[0].Name ?? prop.Name.ToLowerInvariant();
                    object value;
                    if (parser.WrangleNamedArgument(name, prop.PropertyType, argumentAttributes[0], parsingOptions.FormatProvider, out value))
                    {
                        prop.SetMethod.Invoke(args, new object[] { value });
                    }
                    else if (argumentAttributes[0].DefaultValue != null)
                    {
                        if (prop.PropertyType.IsAssignableFrom(argumentAttributes[0].DefaultValue.GetType()))
                        {
                            prop.SetMethod.Invoke(args, new object[] { argumentAttributes[0].DefaultValue });
                        }
                        else
                        {
                            throw new InvalidArgumentsClassException(string.Format("{0}: Incompatible default value on {1}", argsType.FullName, prop.Name));
                        }
                    }
                }
                else if (positionalArgumentAttributes.Length == 1)
                {
                    int n = positionalArgumentAttributes[0].N;
                    if (n < 0) n += parser.m_PositionalArgs.Count;

                    object value;

                    if (n >= 0 && n < parser.m_PositionalArgs.Count && StringToObjectParser.ParseStringToObject(parser.m_PositionalArgs[n], prop.PropertyType, parsingOptions.FormatProvider, out value))
                    {
                        prop.SetMethod.Invoke(args, new object[] { value });
                    }
                    else if (positionalArgumentAttributes[0].DefaultValue != null)
                    {
                        if (prop.PropertyType.IsAssignableFrom(positionalArgumentAttributes[0].DefaultValue.GetType()))
                        {
                            prop.SetMethod.Invoke(args, new object[] { positionalArgumentAttributes[0].DefaultValue });
                        }
                        else
                        {
                            throw new InvalidArgumentsClassException(string.Format("{0}: Incompatible default value on {1}", argsType.FullName, prop.Name));
                        }
                    }
                }
                else if (allPositionalsAttributes.Length == 1)
                {
                    if (prop.PropertyType.IsAssignableFrom(typeof(string[])))
                    {
                        prop.SetMethod.Invoke(args, new object[] { parser.m_PositionalArgs.ToArray() });
                    }
                    else
                    {
                        throw new InvalidArgumentsClassException(string.Format("{0}: AllPositionalArguments properties must be of type string[] or an interface string[] implements. {1} is not.", argsType.FullName, prop.Name));
                    }
                }
            }

            return args;
        }

        /// <summary>
        /// Determines if a argument's property should be set, and if it should, gets the strongly-typed object that it should be set to.
        /// </summary>
        /// <param name="name">The name of the argument we're looking for.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="argumentAttributes">The ArgumentAttribute on the property.</param>
        /// <param name="value">The value that the argument should be set to, if the return value is true.</param>
        /// <returns>If true, the property should be set; if false, the property should be left alone.</returns>
        private bool WrangleNamedArgument(string name, Type propertyType, ArgumentAttribute argumentAttribute, IFormatProvider formatProvider, out object value)
        {
            var alias = argumentAttribute.Alias;
            var defaultValue = argumentAttribute.DefaultValue;

            // valid name for an argument?
            if (!ArgumentNameValid(name)) throw new InvalidArgumentNameException(name);

            // string value
            string strValue = null;

            // is the value defined? (full names override aliases)
            if (this.m_LongArgs.ContainsKey(name))
            {
                strValue = m_LongArgs[name];
            }
            else if (alias != '\0' && this.m_ShortArgs.ContainsKey(alias))
            {
                strValue = m_ShortArgs[alias];
            }
            else if (propertyType == typeof(bool)) // bools are always set false when omitted
            {
                value = false;
                return true;
            }
            else if (defaultValue != null) // no value, fall back to default
            {
                value = defaultValue;
                return true;
            }
            else // no value or default, leave the property or field unset.
            {
                value = null;
                return false;
            }

            // bools are special, so if we have a value, we always return true here.
            if (propertyType == typeof(bool))
            {
                value = true;
                return true;
            }

            // We have a value, now we try to parse it.
            return StringToObjectParser.ParseStringToObject(strValue, propertyType, formatProvider, out value);
        }

        
        /// <summary>
        /// Checks to see if the name is long enough and doesn't contain
        /// any invalid characters. Valid names are 2 or more characters
        /// in length and contain no = or : characters.
        /// </summary>
        private static bool ArgumentNameValid(string name)
        {
            if (name.Length < 2) return false;

            foreach (char c in name)
            {
                switch (c)
                {
                    case '=':
                    case ':':
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Helper function for splitting strings.
        /// </summary>
        private static bool SplitOn(string str, out string part1, out string part2, params char[] splitChars)
        {
            int n = str.IndexOfAny(splitChars);

            if (n < 0)
            {
                part1 = str;
                part2 = null;
                return false;
            }
            else
            {
                part1 = str.Remove(n);
                part2 = str.Substring(n + 1);
                return true;
            }
        }
    }
}
