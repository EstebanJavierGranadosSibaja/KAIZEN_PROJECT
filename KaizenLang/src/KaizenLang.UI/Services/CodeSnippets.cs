namespace KaizenLang.UI.Services
{
    public static class CodeSnippets
    {
        public const string ReservedWords =
@"// Palabras Reservadas de KaizenLang
// Estas palabras están reservadas por el lenguaje y no pueden usarse como nombres de variables

// ENTRADA/SALIDA:
// output - Imprime valores en consola
// input - Lee entrada del usuario

// TIPOS DE DATOS:
// integer - Números enteros
// float - Números decimales de precisión simple
// double - Números decimales de doble precisión
// string - Cadenas de texto
// bool - Valores booleanos (true/false)

// CONTROL DE FLUJO:
// if - Estructura condicional
// else - Alternativa de condicional
// while - Bucle con condición previa
// for - Bucle con contador

// DELIMITADORES DE BLOQUE:
// ying - Inicio de bloque
// yang - Fin de bloque

// FUNCIONES:
// return - Devuelve un valor desde función
// void - Tipo de retorno sin valor

// LITERALES:
// true - Valor booleano verdadero
// false - Valor booleano falso
// null - Valor nulo

// COLECCIONES:
// Chainsaw - Array/Lista dinámica
// Hogyoku - Diccionario clave-valor";

        public const string IfSimple =
@"integer edad = 18;
if (edad >= 18) ying
    output(""Eres mayor de edad"");
yang";

        public const string IfElse =
@"integer temperatura = 25;
if (temperatura > 30) ying
    output(""Hace calor"");
yang else ying
    output(""El clima es agradable"");
yang";

        public const string IfComparacion =
@"integer nota = 85;
if (nota >= 90) ying
    output(""Excelente"");
yang else ying
    output(""Aprobado"");
yang";

        public const string IfBooleano =
@"bool esEstudiante = true;
if (esEstudiante) ying
    output(""Tienes descuento estudiantil"");
yang else ying
    output(""Precio regular"");
yang";

        public const string IfString =
@"string nombre = input(""Ingresa tu nombre: "");
if (nombre == ""Admin"") ying
    output(""Acceso administrativo concedido"");
yang else ying
    output(""Acceso de usuario regular"");
yang";

        public const string WhileContador =
@"integer contador = 1;
while (contador <= 5) ying
    output(""Contador: "" + contador);
    contador = contador + 1;
yang";

        public const string WhileSuma =
@"integer suma = 0;
integer i = 1;
while (i <= 10) ying
    suma = suma + i;
    i = i + 1;
yang
output(""La suma de 1 a 10 es: "" + suma);";

        public const string WhileCondicion =
@"bool continuar = true;
integer intentos = 0;
while (continuar) ying
    output(""Intento número: "" + intentos);
    intentos = intentos + 1;
    if (intentos >= 3) ying
        continuar = false;
    yang
yang";

        public const string WhileMenu =
@"bool continuar = true;
while (continuar) ying
    output(""--- Menú ---"");
    output(""1. Saludar"");
    output(""2. Despedir"");
    output(""3. Salir"");
    string entrada = input(""Elige opción: "");
    if (entrada == ""1"") ying
        output(""¡Hola!"");
    yang
    if (entrada == ""2"") ying
        output(""¡Adiós!"");
    yang
    if (entrada == ""3"") ying
        continuar = false;
        output(""Saliendo..."");
    yang
yang";
        public const string ForContador =
@"for (integer i = 1; i <= 5; i = i + 1) ying
    output(""Número: "" + i);
yang";

        public const string ForSuma =
@"integer suma = 0;
for (integer i = 1; i <= 100; i = i + 1) ying
    suma = suma + i;
yang
output(""Suma de 1 a 100: "" + suma);";

        public const string ForTabla =
@"integer numero = 7;
output(""Tabla del 7:"");
for (integer i = 1; i <= 10; i = i + 1) ying
    integer resultado = numero * i;
    output(""7 x "" + i + "" = "" + resultado);
yang";

        public const string ForPares =
@"output(""Números pares del 0 al 20:"");
for (integer i = 0; i <= 20; i = i + 2) ying
    output(i);
yang";

        public const string ForDescendente =
@"output(""Cuenta regresiva:"");
for (integer i = 10; i >= 1; i = i - 1) ying
    output(i);
yang
output(""¡Despegue!"");";
        public const string FunctionSimple =
@"void saludar() ying
    output(""¡Hola desde una función!"");
yang

saludar();";

        public const string FunctionConParametros =
@"integer sumar(integer a, integer b) ying
    return a + b;
yang

integer resultado = sumar(5, 3);
output(""Resultado: "" + resultado);";

        public const string FunctionMultiplicar =
@"integer multiplicar(integer x, integer y) ying
    return x * y;
yang

integer producto = multiplicar(4, 7);
output(""4 x 7 = "" + producto);";

        public const string FunctionEsPar =
@"bool esPar(integer numero) ying
    if (numero == 10) ying
        return true;
    yang
    if (numero == 8) ying
        return true;
    yang
    if (numero == 6) ying
        return true;
    yang
    if (numero == 4) ying
        return true;
    yang
    if (numero == 2) ying
        return true;
    yang
    return false;
yang

if (esPar(10)) ying
    output(""10 es par"");
yang
if (esPar(7)) ying
    output(""7 es par"");
yang else ying
    output(""7 es impar"");
yang";

        public const string FunctionMaximo =
@"integer maximo(integer a, integer b) ying
    if (a > b) ying
        return a;
    yang else ying
        return b;
    yang
yang

integer mayor = maximo(15, 23);
output(""El mayor es: "" + mayor);";
        public const string DataTypesBasico =
@"integer edad = 25;
float altura = 1.75;
double pi = 3.14159265359;
string nombre = ""Carlos"";
bool esActivo = true;

output(""Nombre: "" + nombre);
output(""Edad: "" + edad);
output(""Altura: "" + altura);";

        public const string DataTypesConversiones =
@"integer entero = 42;
float numeroDecimal = 3.14;
string texto = ""100"";

output(""Entero: "" + entero);
output(""Decimal: "" + numeroDecimal);
output(""Texto: "" + texto);

bool verdadero = true;
bool falso = false;
output(""Verdadero: "" + verdadero);
output(""Falso: "" + falso);";
        public const string OperationsAritmeticas =
@"integer a = 10;
integer b = 3;

output(""Suma: "" + (a + b));
output(""Resta: "" + (a - b));
output(""Multiplicación: "" + (a * b));
output(""División: "" + (a / b));

float x = 10.0;
float y = 3.0;
output(""División decimal: "" + (x / y));";

        public const string OperationsComparacion =
@"integer a = 10;
integer b = 5;

bool mayor = (a > b);
bool menor = (a < b);
bool igual = (a == b);
bool diferente = (a != b);
bool mayorIgual = (a >= b);
bool menorIgual = (a <= b);

output(""Mayor: "" + mayor);
output(""Menor: "" + menor);
output(""Igual: "" + igual);
output(""Diferente: "" + diferente);";
        public const string OperationsLogicas =
@"bool verdadero = true;
bool falso = false;

bool andResultado = (verdadero && falso);
bool orResultado = (verdadero || falso);
bool notResultado = (!verdadero);

output(""AND: "" + andResultado);
output(""OR: "" + orResultado);
output(""NOT: "" + notResultado);

integer edad = 20;
bool esMayor = (edad >= 18);
output(""Es mayor de edad: "" + esMayor);";
        public const string InputOutputBasico =
@"output(""¡Hola, Mundo!"");

string nombre = input(""¿Cómo te llamas? "");
output(""Hola, "" + nombre);

string edadTexto = input(""¿Cuántos años tienes? "");
output(""Tienes "" + edadTexto + "" años"");";

        public const string InputOutputCalculadora =
@"output(""=== Calculadora Simple ==="");

string nombre = input(""¿Cuál es tu nombre? "");
output(""Hola, "" + nombre + ""!"");

string edad = input(""¿Cuántos años tienes? "");
output(nombre + "" tiene "" + edad + "" años"");";
    }
}
