using System.Text.Json;

namespace KaizenLang.UI.Utils
{
    public static class JsonUtil
    {
        // Convierte un objeto a JSON
        public static string ToJson<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        // Convierte un JSON a un objeto de tipo T
        public static T FromJson<T>(string json)
        {
            var result = JsonSerializer.Deserialize<T>(json);
            if (result == null)
                throw new InvalidOperationException("Deserialization returned null.");
            return result;
        }

        // * Guardar y cargar *
        public static void SaveToFile<T>(T obj, string filePath)
        {
            var json = ToJson(obj);
            File.WriteAllText(filePath, json);
        }

        public static T LoadFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            var json = File.ReadAllText(filePath);
            return FromJson<T>(json);
        }

        // carga el json a string
        public static string LoadJsonStringFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            return File.ReadAllText(filePath);
        }


        // * Recibe un objecto y rescribe sus valores con los de un JSON *
        public static void OverwriteFromJson<T>(T obj, string json)
        {
            var newObj = FromJson<T>(json);
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                if (prop.CanWrite)
                {
                    var newValue = prop.GetValue(newObj);
                    prop.SetValue(obj, newValue);
                }
            }
        }
    }
}