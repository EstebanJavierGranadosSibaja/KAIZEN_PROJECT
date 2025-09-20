namespace ParadigmasLang
{
    public static class TypeWords
    {
        public static readonly HashSet<string> Words = new HashSet<string>
        {
            // Base (primitive) types. Composite types (arrays/matrices) are parsed
            // by the parser using the generic-like syntax `array<type>` and
            // `matrix<type>`; do NOT list composed names like "array_integer" here.
            "integer", "float", "double", "bool", "string"
        };
    }
}
