namespace ParadigmasLang;

public static class TypeWords
{
    public const string INTEGER = "integer";
    public const string FLOAT = "float";
    public const string DOUBLE = "double";
    public const string BOOL = "bool";
    public const string STRING = "string";

    public const string CHAINSAW = "chainsaw";
    public const string HOGYOKU = "hogyoku";

    public static readonly HashSet<string> Words = new HashSet<string>
    {
        INTEGER,
        FLOAT,
        DOUBLE,
        BOOL,
        STRING
    };

    public static readonly HashSet<string> CompositeWrappers = new HashSet<string>
    {
        CHAINSAW,
        HOGYOKU
    };
}
