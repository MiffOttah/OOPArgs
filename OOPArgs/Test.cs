using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPArgs
{
    internal class TestProgram
    {
        internal class TestArgs
        {
            [Argument(Name = "bool", DefaultValue = false)]
            public bool SomeBoolean { get; set; }

            [Argument(Alias = 'f', DefaultValue = "hello")]
            public string Foo { get; set; }

            [Argument]
            public string Bar { get; set; }

            [Argument(Alias='n', DefaultValue=42)]
            public int Number { get; set; }

            [Argument(Alias='a')]
            public bool TestShortA { get; set; }

            [Argument(Alias = 'b')]
            public bool TestShortB { get; set; }

            [Argument(Alias = 'c')]
            public bool TestShortC { get; set; }

            [PositionalArgument(0)]
            public string PositionalArgument1 { get; set; }

            [PositionalArgument(1, DefaultValue = "DefaultForPositional2")]
            public string PositionalArgument2 { get; set; }

            [PositionalArgument(-1)]
            public string PositionalArgumentLast { get; set; }

            [AllPositionalArguments]
            public IEnumerable<string> Positionals { get; set; }
        }

        static void Main(string[] rawArgs)
        {
            var args = ArgsParser.Parse<TestArgs>(rawArgs);

            foreach (var prop in args.GetType().GetProperties())
            {
                object value = prop.GetMethod.Invoke(args, null);
                Console.WriteLine("args.{0} = {1}", prop.Name, value);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
