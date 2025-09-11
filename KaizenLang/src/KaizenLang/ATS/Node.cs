namespace ParadigmasLang
{
    public class Node
    {
        public string Type { get; set; }
        public List<Node> Children { get; set; }
        public object? Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Node()
        {
            Type = string.Empty;
            Children = new List<Node>();
            Line = 0;
            Column = 0;
        }

        public Node(string type) : this()
        {
            Type = type;
        }

        public Node(string type, object? value) : this(type)
        {
            Value = value;
        }

        // Método para agregar un nodo hijo
        public void AddChild(Node child)
        {
            Children.Add(child);
        }

        // Método para buscar un hijo por tipo
        public Node? FindChild(string type)
        {
            return Children.FirstOrDefault(c => c.Type == type);
        }

        // Método para obtener todos los hijos de un tipo específico
        public List<Node> FindChildren(string type)
        {
            return Children.Where(c => c.Type == type).ToList();
        }

        // Método para imprimir el árbol de manera legible
        public string ToTreeString(int indent = 0)
        {
            string result = new string(' ', indent * 2) + Type;
            if (Value != null)
                result += $": {Value}";
            result += "\n";
            
            foreach (var child in Children)
                result += child.ToTreeString(indent + 1);
            
            return result;
        }

        // Método para verificar si el nodo tiene errores
        public bool HasErrors()
        {
            return Type.Contains("Error") || Type.Contains("Invalid") || 
                   Children.Any(c => c.HasErrors());
        }

        // Método para obtener todos los errores del árbol
        public List<string> GetAllErrors()
        {
            var errors = new List<string>();
            
            if (Type.Contains("Error") || Type.Contains("Invalid"))
            {
                if (Children.Count > 0 && !string.IsNullOrEmpty(Children[0].Type))
                    errors.Add(Children[0].Type);
                else
                    errors.Add($"Error en nodo: {Type}");
            }
            
            foreach (var child in Children)
                errors.AddRange(child.GetAllErrors());
            
            return errors;
        }
    }
}