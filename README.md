**OOPArgs - a strongly-typed object-oriented arguments parser for .NET**

OOPArgs lets a developer define arguments using standard .NET class syntax. Parsing of numeric types and enumerations is handled automatically, too.

In order to use OOPArgs, create a POCO (plain old CLR object) class, give it properties (not fields!) that correspond to the arguments you wish to take in, and then attach the Argument, PositionalArgument, or AllPositionalArguments attributes to it. Then, use the `ArgsParser.Parse<T>` method to create and populate the object.

The object's type should have an no-parameters constructor, which is called to create the object at first. You can use this to define default values, or use the DefaultValue parameter of the Argument/PositionalArgument attributes. Or both.

The classes should be self-explainatory. Intellisense is your friend!
