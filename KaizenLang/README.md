# 🚀 KaizenLang

<div align="center">

**Lenguaje de programación educativo con tipado estricto y sintaxis única**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-Educational-blue)](LICENSE)

[Características](#-características) •
[Instalación](#-instalación) •
[Uso](#-uso) •
[Sintaxis](#-sintaxis) •
[Ejemplos](#-ejemplos) •
[Documentación](#-documentación)

</div>

---

## 📖 Descripción

**KaizenLang** es un lenguaje de programación educativo desarrollado como proyecto académico para el curso de Paradigmas de Programación de la Universidad Nacional de Costa Rica. El proyecto implementa un compilador e intérprete completo con IDE integrado.

### Filosofía del Proyecto

El nombre "Kaizen" (改善) proviene del japonés y significa "mejora continua". Este lenguaje fue diseñado para enseñar conceptos fundamentales de programación con una sintaxis clara y tipado estricto que previene errores comunes.

---

## ✨ Características

### 🎯 Lenguaje

- **Tipado estricto**: No conversiones implícitas peligrosas, todas las variables deben declararse explícitamente
- **Sintaxis única**: Usa `ying` y `yang` como delimitadores de bloques, inspirados en el concepto de balance
- **5 tipos de datos simples**: `integer`, `float`, `double`, `bool`, `string`
- **2 tipos de datos compuestos**: `chainsaw<T>`, `hogyoku<T>` (colecciones 1D y 2D)
- **Estructuras de control**: `if-else`, `while`, `for`
- **Funciones**: Con retorno de valores y funciones `void`
- **Operaciones**: Aritméticas (`+`, `-`, `*`, `/`), lógicas (`&&`, `||`, `!`), comparación (`>`, `<`, `==`, etc.)
- **Entrada/Salida**: Funciones builtin `input()` y `output()`

### 🖥️ IDE Integrado

- **Editor con syntax highlighting**: Resalta palabras clave, tipos, operadores y literales
- **Compilación en tiempo real**: Detecta errores de sintaxis y semánticos
- **Ejecución interactiva**: Interpreta código directamente desde el IDE
- **Generación automática de código**: Menú con snippets para todas las estructuras
- **Tema oscuro moderno**: Inspirado en el logo Kaizen con paleta verde y dorado
- **Barra de estado inteligente**: Muestra línea/columna, estado de compilación y hora

### 🔧 Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                      Pipeline de Compilación                 │
├─────────────────────────────────────────────────────────────┤
│  Código Fuente                                              │
│       ↓                                                      │
│  Lexer (Tokenización)    → Tokens con línea/columna        │
│       ↓                                                      │
│  Parser (Análisis Sintáctico) → AST (Árbol Sintáctico)     │
│       ↓                                                      │
│  Semantic Analyzer       → Validación de tipos y símbolos   │
│       ↓                                                      │
│  Interpreter             → Ejecución del AST                │
│       ↓                                                      │
│  Output / Errores                                           │
└─────────────────────────────────────────────────────────────┘
```

**Componentes modulares:**
- `Lexeme`: Tokenización y análisis léxico
- `Syntax`: Parser y construcción del AST
- `Semantic`: Análisis semántico, tabla de símbolos, validación de tipos
- `Interpreter`: Ejecución del código
- `UI`: Interfaz gráfica con syntax highlighting

---

## 🚀 Instalación

### Requisitos

- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** o superior
- **Sistema Operativo**: Windows 10/11, Linux, macOS
- **IDE recomendado**: Visual Studio 2022, Visual Studio Code o Rider

### Clonar el Repositorio

```bash
git clone https://github.com/EstebanJavierGranadosSibaja/KAIZEN_PROJECT.git
cd KAIZEN_PROJECT/KaizenLang
```

### Compilar el Proyecto

```bash
dotnet restore
dotnet build
```

### Ejecutar Tests

```bash
dotnet test tools/Tests/Tests.csproj
```

---

## 💻 Uso

### Ejecutar el IDE

```bash
dotnet run --project src/KaizenLang.App/KaizenLang.App.csproj
```

O directamente desde el ejecutable compilado:

```bash
# Windows
.\src\KaizenLang.App\bin\Debug\net9.0-windows\KaizenLang.App.exe

# Linux/macOS
./src/KaizenLang.App/bin/Debug/net9.0-windows/KaizenLang.App
```

### Herramientas de Desarrollo

#### 1. **CompilationTester** - Testing E2E desde consola

```bash
dotnet run --project tools/CompilationTester/CompilationTester.csproj
```

Muestra tokens, AST completo y errores semánticos para snippets de código.

#### 2. **AstDump** - Volcado de AST para debugging

```bash
dotnet run --project tools/AstDump/AstDump.csproj
```

#### 3. **IDERunner** - Ejecución de archivos `.kaizen`

```bash
dotnet run --project tools/IDERunner/IDERunner.csproj -- sample.kaizen
```

#### 4. **QuickRunner** - Ejecución rápida de snippets

```bash
dotnet run --project tools/QuickRunner/QuickRunner.csproj
```

---

## 📝 Sintaxis

### Declaración de Variables

```kaizen
// Tipado explícito obligatorio
integer edad = 25;
float altura = 1.75;
double temperatura = 36.6;
bool esEstudiante = true;
string nombre = "Ana";
```

### Estructuras de Control

```kaizen
// Condicional if-else con sintaxis ying/yang
if (edad > 18) ying
    output("Mayor de edad");
yang else ying
    output("Menor de edad");
yang

// Bucle while
integer contador = 0;
while (contador < 5) ying
    output("Iteración: " + contador);
    contador = contador + 1;
yang

// Bucle for
for (integer i = 0; i < 10; i = i + 1) ying
    output("Número: " + i);
yang
```

### Funciones

```kaizen
// Función con retorno
integer suma(integer a, integer b) ying
    return a + b;
yang

// Función void
void saludar(string nombre) ying
    output("Hola " + nombre + "!");
yang

// Llamadas a funciones
integer resultado = suma(10, 5);
saludar("KaizenLang");
```

### Chainsaw y Hogyoku

```kaizen
// Chainsaw con tipo genérico
chainsaw<integer> numeros = [1, 2, 3, 4, 5];
chainsaw<string> nombres = ["Ana", "Luis", "Pedro"];

// Acceso por índice
integer primero = numeros[0];
string persona = nombres[1];

// Hogyoku (colecciones 2D)
hogyoku<integer> tabla = [
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9]
];

integer elemento = tabla[0][1];  // Valor: 2
```

### Entrada y Salida

```kaizen
// Solicitar entrada del usuario
string nombre = input();
output("Hola " + nombre);

// Obtener longitud de un chainsaw
chainsaw<integer> datos = [10, 20, 30];
integer tam = length(datos);
output("Tamaño del chainsaw: " + tam);
```

### Operaciones

```kaizen
// Aritméticas
integer x = 10 + 5 * 2;     // 20
float y = 10.0 / 3.0;        // 3.333...

// Comparación
bool mayorQue = (x > 15);    // true
bool igual = (y == 3.33);    // false

// Lógicas
bool resultado = (x > 10) && (y < 5);  // true
bool negacion = !resultado;             // false
```

---

## 📚 Ejemplos

### Ejemplo 1: Factorial Recursivo

```kaizen
integer factorial(integer n) ying
    if (n <= 1) ying
        return 1;
    yang
    return n * factorial(n - 1);
yang

integer resultado = factorial(5);
output("5! = " + resultado);  // Output: 5! = 120
```

### Ejemplo 2: Suma de Chainsaw

```kaizen
chainsaw<integer> numeros = [10, 20, 30, 40, 50];
integer suma = 0;
integer tam = length(numeros);

for (integer i = 0; i < tam; i = i + 1) ying
    suma = suma + numeros[i];
yang

output("La suma es: " + suma);  // Output: La suma es: 150
```

### Ejemplo 3: Validación de Entrada

```kaizen
void validarEdad() ying
    string input_texto = input();
    // Nota: En versión actual, conversión manual de string a integer
    // está delegada al runtime para inputs

    if (edad >= 18) ying
        output("Acceso permitido");
    yang else ying
        output("Acceso denegado");
    yang
yang

validarEdad();
```

### Más Ejemplos

Consulta la carpeta `Resources/Examples/` para más ejemplos:

- `example.txt` - Ejemplos básicos
- `arrays-matrices-validation.txt` - Validación de colecciones (chainsaw/hogyoku)
- `types-validation.txt` - Validación de tipos
- `nested-blocks-validation.txt` - Bloques anidados
- `input-and-functioncalls.txt` - Entrada/salida y funciones

---

## 📖 Documentación

### Estructura del Proyecto

```
KaizenLang/
├── src/
│   ├── KaizenLang/              # Punto de entrada de la aplicación
│   ├── KaizenLang.Core/         # Compilador e intérprete
│   │   ├── Lexeme/              # Análisis léxico
│   │   ├── Syntax/              # Análisis sintáctico
│   │   ├── Semantic/            # Análisis semántico
│   │   ├── Interpreter/         # Ejecución
│   │   └── Tokens/              # Definición de tokens
│   └── KaizenLang.UI/           # Interfaz gráfica
│       └── Theme/               # Sistema de temas
├── tools/
│   ├── Tests/                   # Tests automatizados (xUnit)
│   ├── CompilationTester/       # Herramienta de testing E2E
│   ├── AstDump/                 # Volcado de AST
│   ├── IDERunner/               # Ejecutor de archivos .kaizen
│   └── QuickRunner/             # Ejecutor de snippets
├── Resources/
│   ├── HelpFiles/               # Archivos de ayuda del IDE
│   └── Examples/                # Ejemplos de código
└── docs/                        # Documentación técnica
    ├── REVISION_PROYECTO.md     # Revisión completa del proyecto
    ├── DEV-quickstart.md        # Guía de inicio rápido
    └── Buenas-Practicas-C#.md   # Estándares de código
```

### Documentación Técnica

- **[REVISION_PROYECTO.md](docs/REVISION_PROYECTO.md)** - Revisión exhaustiva comparando con requisitos del curso
- **[DEV-quickstart.md](docs/DEV-quickstart.md)** - Guía de desarrollo rápido
- **[ESTRUCTURA_PROYECTO.md](docs/ESTRUCTURA_PROYECTO.md)** - Arquitectura del sistema
- **[Buenas-Practicas-C#.md](docs/Buenas-Practicas-C#.md)** - Convenciones de código
- **[descripción-proyecto.md](docs/descripción-proyecto.md)** - Requisitos originales del curso

### Ayuda Integrada en el IDE

El IDE incluye un menú completo con documentación y snippets:

- **Palabras Reservadas** - Lista de keywords y su uso
- **Sintaxis** → Control, Funciones, Operaciones
- **Semántica** - Reglas del lenguaje
- **Tipos de Datos** - Documentación de tipos simples y compuestos

---

## 🧪 Testing

El proyecto incluye tests automatizados con **xUnit**:

```bash
# Ejecutar todos los tests
dotnet test tools/Tests/Tests.csproj

# Ejecutar con detalles
dotnet test tools/Tests/Tests.csproj --logger "console;verbosity=detailed"

# Ejecutar con cobertura (requiere herramientas adicionales)
dotnet test tools/Tests/Tests.csproj --collect:"XPlat Code Coverage"
```

### Tests Disponibles

- **DeclarationCheckerTests** - Validación de declaraciones
- **CollectionSemanticTests** - Validación de chainsaw/hogyoku
- **FunctionTests** - Validación de funciones

**Estado actual**: 7 tests pasando, 1 test omitido (lexer variability).

---

## 🎨 Características del IDE

### Syntax Highlighting

El editor incluye resaltado de sintaxis para:

- **Comentarios** - Verde e itálica (`//`)
- **Palabras clave** - Bold (`if`, `else`, `for`, `while`, `ying`, `yang`)
- **Tipos** - Bold (`integer`, `string`, `chainsaw<>`, `hogyoku<>`)
- **Funciones builtin** - Color especial (`input`, `output`, `length`)
- **Cadenas de texto** - Color distintivo
- **Números** - Resaltados
- **Operadores** - Destacados

### Menú de Generación de Código

Accede rápidamente a snippets de código desde el menú:

1. **Estructuras del Lenguaje** →
   - Palabras Reservadas
   - Sintaxis (Control, Funciones, Operaciones)
   - Semántica
   - Tipos de Datos

Cada opción inserta automáticamente código de ejemplo en el editor.

### Barra de Estado

La barra inferior muestra:

- 📍 **Posición del cursor** (Línea, Columna)
- ✅ **Estado de compilación** (Éxito / Error)
- 🕒 **Hora actual**

### Compilación y Ejecución

1. **Botón Compilar** 🔧
   - Ejecuta análisis léxico, sintáctico y semántico
   - Muestra errores con línea y columna exacta
   - Feedback visual inmediato

2. **Botón Ejecutar** ▶️
   - Interpreta el código compilado
   - Maneja entrada/salida interactiva
   - Muestra resultados en panel de output

---

## 🔍 Validación de Errores

KaizenLang valida errores en **todas las fases**:

### Errores Léxicos

```kaizen
integer x = 123abc;  // ❌ Token inválido
```

### Errores Sintácticos

```kaizen
integer x = 10  // ❌ Falta punto y coma
if x > 5 ying   // ❌ Falta paréntesis en condición
chainsaw<integer> = [1, 2, 3];  // ❌ Falta identificador
```

### Errores Semánticos

```kaizen
integer x = "texto";          // ❌ Tipo incompatible
integer y = z + 10;           // ❌ Variable 'z' no declarada
integer suma(integer a) ying  // ❌ Falta return en función no-void
    integer b = 10;
yang
chainsaw<integer> nums = [1, "dos", 3];  // ❌ Elementos heterogéneos
hogyoku<integer> mat = [[1, 2], [3]];  // ❌ Hogyoku no rectangular
```

### Errores de Runtime

```kaizen
integer x = 10 / 0;           // ❌ División por cero
chainsaw<integer> a = [1, 2, 3];
integer y = a[10];            // ❌ Índice fuera de rango
```

---

## 🎓 Proyecto Académico

### Universidad Nacional de Costa Rica
**Sede Regional Brunca - Campus Coto**

- **Curso**: Paradigmas de Programación
- **Profesor**: MSc. Josías Ariel Chaves Murillo
- **Proyecto**: #1 - Diseño e implementación de lenguaje de programación
- **Evaluación**: Documentación (10%), Planteamiento (20%), Funcionamiento (60%), Defensa (10%)

### Requisitos Cumplidos

✅ Nombre del lenguaje definido
✅ 5 tipos de datos simples (integer, float, double, bool, string)
✅ 2 tipos de datos compuestos (chainsaw, hogyoku)
✅ Palabras reservadas documentadas
✅ Sintaxis completa (control, funciones, operaciones)
✅ Semántica clara y documentada
✅ Tipado estricto sin conversiones peligrosas
✅ Sintaxis única (ying/yang)
✅ Validación exhaustiva de errores
✅ Interfaz gráfica completa
✅ Menú con generación de código
✅ Compilación y ejecución funcionales

### Autores

**Equipo de Desarrollo:**
- [Tu Nombre]
- [Nombre del Compañero - si aplica]

---

## 🛠️ Tecnologías Utilizadas

- **Lenguaje**: C# 12.0
- **Framework**: .NET 9.0
- **UI**: Windows Forms con tema personalizado
- **Testing**: xUnit
- **Control de versiones**: Git
- **IDE Desarrollo**: Visual Studio 2022 / VS Code

---

## 📜 Licencia

Este proyecto es de carácter **educativo** y fue desarrollado como parte del curso de Paradigmas de Programación de la Universidad Nacional de Costa Rica.

---

## 🤝 Contribuciones

Este es un proyecto académico cerrado. Sin embargo, si encuentras bugs o tienes sugerencias, puedes abrir un issue en el repositorio para discusión académica.

---

## 📞 Contacto

Para preguntas sobre el proyecto, contactar a través del repositorio o el profesor del curso.

---

<div align="center">

**🌟 KaizenLang - Mejora Continua en Programación 🌟**

Hecho con ❤️ para la Universidad Nacional de Costa Rica

</div>
