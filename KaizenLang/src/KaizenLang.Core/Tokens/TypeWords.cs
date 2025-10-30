namespace ParadigmasLang;

public static class TypeWords
{
    // Constants for type names - USE THESE instead of hardcoded strings
    public const string INTEGER = "integer";
    public const string FLOAT = "float";
    public const string DOUBLE = "double";
    public const string BOOL = "bool";
    public const string STRING = "string";

    public const string CHAINSAW = "chainsaw";
    public const string HOGYOKU = "hogyoku";

    public static readonly HashSet<string> Words = new HashSet<string>
    {
    // Base (primitive) types. Composite types (chainsaw/hogyoku) are parsed
        // by the parser using the generic-like syntax `chainsaw<type>` and
        // `hogyoku<type>`; do NOT list composed names like "chainsaw_integer" here.
        INTEGER, FLOAT, DOUBLE, BOOL, STRING
    };

    public static readonly HashSet<string> CompositeWrappers = new HashSet<string>
    {
        CHAINSAW,
        HOGYOKU
    };
}
