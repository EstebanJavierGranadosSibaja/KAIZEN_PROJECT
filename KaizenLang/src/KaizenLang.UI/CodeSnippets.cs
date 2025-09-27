namespace KaizenLang.UI
{
    public static class CodeSnippets
    {
        public const string ReservedWords =
@"// --- Palabras Reservadas ---
// KaizenLang utiliza estas palabras clave para controlar el flujo, definir tipos y realizar operaciones.
// No pueden ser usadas como nombres de variables.

// Entrada/Salida:
// output: Imprime un valor en la consola.
// input: Lee un valor desde el usuario.
output(""Hola, Mundo"");
string nombre = input(""¿Cómo te llamas?"");

// Tipos de Datos:
// integer, float, double, string, bool
integer edad = 25;
float altura = 1.75;
double temperatura = 21.0;
bool esEstudiante = true;

// Control de Flujo:
// if, else: Para tomar decisiones.
// while, for: Para crear bucles.
if (edad > 18) ying
    output(""Eres mayor de edad"");
yang else ying
    output(""Eres menor de edad"");
yang

// Funciones:
// Declaración con tipo de retorno y parámetros tipados.
// return: Devuelve un valor desde una función.
integer sumar(integer a, integer b) ying
    return a + b;
yang";

        public const string IfStatement =
@"// --- Estructura Condicional if-else ---
// Permite ejecutar bloques de código basados en si una condición es verdadera o falsa.

// Ejemplo 1: if simple
// Se ejecuta solo si la condición es verdadera.
integer numero = 10;
if (numero > 5) ying
    output(""El número es mayor que 5"");
yang

// Ejemplo 2: if-else
// Proporciona un camino alternativo si la condición es falsa.
integer t = 15;
if (t > 25) ying
    output(""Hace calor"");
yang else ying
    output(""No hace calor"");
yang
// Nota: Evita bloques de control anidados para respetar las reglas semánticas actuales.
";

        public const string WhileLoop =
@"// --- Bucle while ---
// Repite un bloque de código mientras una condición sea verdadera.

// Ejemplo: Contar hasta 5
// La variable 'contador' se incrementa en cada iteración.
// El bucle se detiene cuando 'contador' ya no es menor o igual a 5.
integer contador = 1;
while (contador <= 5) ying
    output(""El contador es: "" + contador);
    contador = contador + 1;
yang";

        public const string ForLoop =
@"// --- Bucle for ---
// Repite un bloque de código un número específico de veces.
// Es ideal para iterar sobre secuencias con un inicio, una condición y un incremento definidos.

// Estructura: for (inicialización; condición; incremento) { ... }

// Ejemplo: Contar del 1 al 5
// 1. 'int i = 1;': Se ejecuta una sola vez al inicio.
// 2. 'i <= 5;': Se evalúa antes de cada iteración. Si es falso, el bucle termina.
// 3. 'i = i + 1;': Se ejecuta al final de cada iteración.
for (integer i = 1; i <= 5; i = i + 1) ying
    output(""El número es: "" + i);
yang";

        public const string FunctionDeclaration =
@"// --- Declaración de Funciones ---
// Las funciones son bloques de código reutilizables que realizan una tarea específica.
// Ayudan a organizar el código y evitar la repetición.

// Estructura: tipo_retorno nombre(tipo parametro1, tipo parametro2) ying ... return valor; yang

// Ejemplo 1: Función sin parámetros que no devuelve valor
void saludar() ying
    output(""¡Hola desde una función!"");
yang
// Llamada a la función (las llamadas son expresiones y terminan con ';')
saludar();


// Ejemplo 2: Función con parámetros que devuelve un valor
// Esta función toma dos enteros y devuelve su suma.
integer sumar(integer a, integer b) ying
    return a + b;
yang
// Llamada a la función y almacenamiento del resultado
integer resultado = sumar(5, 3);
output(""El resultado de la suma es: "" + resultado);";

        public const string DataTypes =
@"// --- Tipos de Datos en KaizenLang ---
// Definen la naturaleza de los valores que una variable puede contener.

// integer: Números enteros (sin decimales).
integer edad = 28;
integer cantidad = -100;

// float: Números de punto flotante (con decimales).
float precio = 19.99;
float pi = 3.14159;

// double: Números de doble precisión.
double tasa = 0.05;

// string: Secuencias de caracteres (texto).
string saludo = ""Hola, KaizenLang"";
string nombre = ""Ana"";

// bool: Valores de verdad (verdadero o falso).
bool esActivo = true;
bool haTerminado = false;

// Ejemplo de uso combinado:
output(""Nombre: "" + nombre + "", Edad: "" + edad);
if (esActivo) ying
    output(""El usuario está activo."");
yang";
        
        public const string Operations =
@"// --- Operaciones en KaizenLang ---

// Operaciones Aritméticas:
integer a = 10;
integer b = 3;
output(""Suma: "" + (a + b));          // 13
output(""Resta: "" + (a - b));         // 7
output(""Multiplicación: "" + (a * b)); // 30
output(""División: "" + (a / b));       // 3 (división entera)

float c = 10.0;
float d = 3.0;
output(""División float: "" + (c / d)); // 3.333...

// Operaciones de Comparación (devuelven boolean):
output(""Mayor que: "" + (a > b));      // true
output(""Menor que: "" + (a < b));      // false
output(""Igual a: "" + (a == 10));     // true
output(""Diferente de: "" + (a != b));   // true

// Operaciones Lógicas (con booleanos):
bool x = true;
bool y = false;
output(""AND lógico: "" + (x && y)); // false
output(""OR lógico: "" + (x || y));  // true
output(""NOT lógico: "" + (!x));   // false
";

        public const string Semantics =
@"// --- Semántica de KaizenLang ---

// Asignación de variables:
// Se usa el operador '=' para asignar un valor a una variable.
// La variable debe ser declarada con su tipo.
integer miNumero = 42;
string miTexto = ""Kaizen"";

// Alcance (Scope):
// Las variables declaradas dentro de un bloque (delimitado por ying ... yang)
// solo existen dentro de ese bloque.
integer fuera = 1;
if (fuera == 1) ying
    integer dentro = 2;
    output(""Dentro del if: "" + dentro); // Válido
yang
// output(dentro); // Esto daría un error: 'dentro' no existe aquí.

// Inferencia de tipos (No soportado):
// En KaizenLang, siempre debes declarar explícitamente el tipo de una variable.
// integer edad = 25; // Correcto
// edad = 25;        // Incorrecto si no se ha declarado antes.
";
    }
}
