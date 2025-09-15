<!-- markdownlint-disable MD056 -->
<!-- markdownlint-disable MD033 -->

# 📁 ESTRUCTURA DEL PROYECTO KAIZENLANG

> **Compilador e Intérprete Educativo - Paradigmas de Programación**  
> Universidad Nacional - Sede Regional Brunca  
> Prof. MSc. Josías Ariel Chaves Murillo

---

## 🌲 Árbol de Directorios Completo

```
KAIZEN_PROJECT/
└── KaizenLang/                          # Proyecto principal
    ├── 📄 .gitignore                    # Configuración de Git
    ├── 📄 KaizenLang.csproj             # Archivo de proyecto .NET
    ├── 📄 KaizenLang.sln                # Solución de Visual Studio
    ├── 📄 README.md                     # Documentación principal
    │
    ├── 🗂️ bin/                          # Archivos compilados (generado)
    │   └── Debug/
    │       └── net9.0-windows/
    │           ├── KaizenLang.exe       # Ejecutable principal
    │           ├── KaizenLang.dll       # Librería compilada
    │           ├── KaizenLang.pdb       # Símbolos de depuración
    │           └── *.json               # Configuraciones de runtime
    │
    ├── 🗂️ obj/                          # Archivos temporales (generado)
    │   ├── project.assets.json          # Dependencias del proyecto
    │   ├── *.nuget.*                    # Configuración NuGet
    │   └── Debug/                       # Archivos de compilación temporal
    │
    ├── 📚 docs/                         # Documentación del proyecto
    │   ├── descripción-proyecto.md      # Especificaciones académicas
    │   ├── vs-config.md                 # Configuración de VS Code
    │   └── ESTRUCTURA_PROYECTO.md       # Este archivo
    │
    ├── 💡 Examples/                     # Ejemplos de código
    │   └── example.txt                  # Ejemplos del lenguaje KaizenLang
    │
    └── 💻 src/                          # Código fuente principal
        └── KaizenLang/                  # Namespace principal
            ├── 🔧 Program.cs            # Punto de entrada de la aplicación
            ├── 🔍 Lexer.cs              # Analizador léxico
            ├── 🌳 Parser.cs             # Analizador sintáctico
            ├── ⚡ Interpreter.cs         # Intérprete del lenguaje
            │
            ├── 🏗️ ATS/                  # Abstract Syntax Tree
            │   └── Node.cs              # Nodos del árbol sintáctico
            │
            ├── 🧠 Semantic/             # Análisis semántico
            │   └── SymbolTable.cs       # Tabla de símbolos y validaciones
            │
            ├── 🎯 Tokens/               # Definición de tokens
            │   ├── Token.cs             # Clase base de tokens
            │   ├── ReservedWords.cs     # Palabras reservadas del lenguaje
            │   ├── TypeWords.cs         # Tipos de datos
            │   ├── OperatorWords.cs     # Operadores aritméticos y lógicos
            │   ├── DelimiterWords.cs    # Delimitadores y puntuación
            │   └── LiteralWords.cs      # Literales y constantes
            │
            └── 🖥️ UI/                   # Interfaz de usuario
                ├── CompilationService.cs   # Servicio de compilación
                ├── ControlFactory.cs       # Factory para controles UI
                ├── ExecutionService.cs     # Servicio de ejecución
                ├── MainForm.cs             # Formulario principal
                ├── MenuBuilder.cs          # Constructor de menús
                └── UIConstants.cs          # Constantes de la interfaz
```

---

## 🗂️ DESCRIPCIÓN DETALLADA DE CARPETAS

### 📁 **Carpetas del Sistema**

#### `/bin/` - Archivos Binarios Compilados

```
bin/Debug/net9.0-windows/
├── KaizenLang.exe          # Ejecutable principal de la aplicación
├── KaizenLang.dll          # Librería compilada con toda la lógica
├── KaizenLang.pdb          # Símbolos para depuración en Visual Studio
├── KaizenLang.deps.json    # Dependencias y versiones
└── KaizenLang.runtimeconfig.json  # Configuración del runtime .NET
```

**Propósito**: Contiene todos los archivos necesarios para ejecutar la aplicación compilada.  
**Generación**: Automática durante `dotnet build` o `dotnet run`  
**Nota**: Esta carpeta se puede eliminar y regenerar sin problemas.

#### `/obj/` - Archivos Temporales de Compilación

```
obj/
├── project.assets.json           # Cache de dependencias NuGet
├── KaizenLang.csproj.nuget.*     # Configuración de paquetes NuGet
└── Debug/net9.0-windows/         # Archivos temporales de compilación
    ├── KaizenLang.AssemblyInfo.cs    # Información del ensamblado
    ├── *.cache                       # Archivos de cache
    └── ref/KaizenLang.dll           # Referencias de ensamblado
```

**Propósito**: Almacena archivos temporales durante el proceso de compilación.  
**Generación**: Automática durante la compilación  
**Nota**: Se puede eliminar para hacer una compilación limpia.

### 📁 **Carpetas de Documentación**

#### `/docs/` - Documentación del Proyecto

```
docs/
├── descripción-proyecto.md     # Especificaciones académicas del curso
├── vs-config.md               # Configuración de Visual Studio Code
└── ESTRUCTURA_PROYECTO.md     # Documentación de la estructura (este archivo)
```

**Propósito**: Toda la documentación técnica y académica del proyecto.  
**Contenido**:

- Requerimientos del proyecto universitario
- Configuración del entorno de desarrollo
- Explicación de la arquitectura del código

#### `/Examples/` - Ejemplos de Código

```
Examples/
└── example.txt    # Ejemplos del lenguaje KaizenLang
```

**Propósito**: Contiene ejemplos prácticos del lenguaje de programación.  
**Uso**: La aplicación puede cargar estos ejemplos automáticamente.

---

## 💻 DESCRIPCIÓN DETALLADA DEL CÓDIGO FUENTE

### 🔧 **Archivos Principales**

#### `Program.cs` - Punto de Entrada

```csharp
// Funcionalidad principal:
- Configuración de la aplicación Windows Forms
- Inicialización del formulario principal
- Manejo de excepciones globales
```

**Responsabilidades**:

- Configurar el estilo visual de la aplicación
- Crear y mostrar el formulario principal
- Establecer el punto de entrada único

#### `Lexer.cs` - Analizador Léxico

```csharp
// Funcionalidad principal:
- Tokenización del código fuente
- Reconocimiento de palabras reservadas, identificadores, operadores
- Detección de errores léxicos
- Generación de la secuencia de tokens
```

**Responsabilidades**:

- Leer caracteres del código fuente uno por uno
- Clasificar secuencias de caracteres en tokens específicos
- Validar sintaxis básica de literales (strings, números)
- Reportar errores como caracteres inválidos

#### `Parser.cs` - Analizador Sintáctico

```csharp
// Funcionalidad principal:
- Construcción del Árbol de Sintaxis Abstracta (AST)
- Verificación de gramática del lenguaje
- Detección de errores sintácticos
- Organización jerárquica del código
```

**Responsabilidades**:

- Analizar la secuencia de tokens del Lexer
- Verificar que la estructura siga las reglas gramaticales
- Construir un árbol que represente la estructura del programa
- Detectar errores como paréntesis no balanceados, expresiones incompletas

#### `Interpreter.cs` - Intérprete del Lenguaje

```csharp
// Funcionalidad principal:
- Ejecución del código compilado
- Manejo de variables y memoria
- Implementación de operaciones aritméticas y lógicas
- Control de flujo (if, while, for)
```

**Responsabilidades**:

- Recorrer el AST y ejecutar las instrucciones
- Mantener el estado de las variables durante la ejecución
- Implementar la semántica de cada operación
- Manejar entrada y salida de datos

### 🏗️ **Módulo ATS (Abstract Syntax Tree)**

#### `ATS/Node.cs` - Nodos del Árbol Sintáctico

```csharp
// Estructura de datos principal:
public class Node {
    string Type          // Tipo de nodo (Expression, Statement, etc.)
    List<Node> Children  // Nodos hijos en la jerarquía
    // Métodos para navegar y manipular el árbol
}
```

**Propósito**: Representa cada elemento del código como un nodo en un árbol jerárquico.  
**Ejemplos de nodos**:

- `VariableDeclaration`: `int x = 5;`
- `Expression`: `x + y * 2`
- `IfStatement`: `if (condition) { ... }`

### 🧠 **Módulo Semantic**

#### `Semantic/SymbolTable.cs` - Análisis Semántico

```csharp
// Funcionalidades principales:
- Validación de tipos de datos
- Verificación de scope de variables
- Detección de variables no declaradas
- Compatibilidad de tipos en operaciones
- Manejo de tablas de símbolos anidadas
```

**Responsabilidades**:

- Recorrer el AST y validar la coherencia semántica
- Mantener registro de variables declaradas en cada scope
- Verificar que las operaciones sean válidas entre tipos
- Detectar errores como variables no inicializadas, tipos incompatibles

**Validaciones implementadas**:

- ✅ Tipado estricto: `int x = "texto"` → Error
- ✅ Variables declaradas: `y = x + 1` → Error si `x` no existe
- ✅ Scope correcto: Variables locales no accesibles fuera de su bloque
- ✅ Compatibilidad de tipos: `int + string` → Error

### 🎯 **Módulo Tokens**

#### `Tokens/Token.cs` - Clase Base de Tokens

```csharp
// Estructura básica:
public class Token {
    string Type     // Tipo de token (IDENTIFIER, NUMBER, OPERATOR, etc.)
    string Value    // Valor literal del token
    int Position    // Posición en el código fuente
}
```

#### `Tokens/ReservedWords.cs` - Palabras Reservadas

```csharp
// Palabras reservadas del lenguaje:
"output", "input", "void", "do", "while", "for", 
"if", "else", "return", "true", "false", "null"
```

#### `Tokens/TypeWords.cs` - Tipos de Datos

```csharp
// Tipos simples (5):
"int", "float", "double", "boolean", "char"

// Tipos compuestos (2):
"string", "array"
```

#### `Tokens/OperatorWords.cs` - Operadores

```csharp
// Aritméticos: +, -, *, /, %
// Comparación: ==, !=, <, >, <=, >=
// Lógicos: &&, ||, !
// Asignación: =, +=, -=, *=, /=
```

#### `Tokens/DelimiterWords.cs` - Delimitadores

```csharp
// Puntuación y delimitadores:
";", "{", "}", "(", ")", "[", "]", ",", "."
```

#### `Tokens/LiteralWords.cs` - Literales

```csharp
// Patrones para reconocer:
- Números enteros: 123, -45
- Números decimales: 3.14, -2.5
- Strings: "Hola mundo", 'A'
- Booleanos: true, false
- Null: null
```

### 🖥️ **Módulo UI (Interfaz de Usuario)**

#### `UI/MainForm.cs` - Formulario Principal

```csharp
// Componentes principales:
- TextBox para escribir código
- Botones "Compilar" y "Ejecutar"
- Área de output para mostrar resultados
- Menú con opciones del lenguaje
- StatusBar con información del proyecto
```

**Responsabilidades**:

- Manejar la interacción del usuario
- Coordinar entre los servicios de compilación y ejecución
- Mostrar resultados y errores de forma visual
- Proporcionar ejemplos automáticos del lenguaje

#### `UI/CompilationService.cs` - Servicio de Compilación

```csharp
// Funcionalidades:
- Coordinar las tres fases: Léxico → Sintáctico → Semántico
- Recopilar estadísticas de compilación
- Formatear mensajes de error y éxito
- Generar reportes detallados con colores y emojis
```

**Proceso de compilación**:

1. **Fase 1**: Análisis léxico → Lista de tokens
2. **Fase 2**: Análisis sintáctico → AST
3. **Fase 3**: Análisis semántico → Validaciones
4. **Resultado**: Código listo para ejecutar o lista de errores

#### `UI/ExecutionService.cs` - Servicio de Ejecución

```csharp
// Funcionalidades:
- Ejecutar código previamente compilado
- Manejar entrada y salida durante la ejecución
- Controlar el flujo de ejecución
- Reportar errores de runtime
```

#### `UI/ControlFactory.cs` - Factory de Controles

```csharp
// Funcionalidades:
- Crear botones con estilos consistentes
- Configurar colores y fuentes del tema
- Aplicar efectos visuales (hover, click)
- Mantener consistencia en toda la UI
```

#### `UI/MenuBuilder.cs` - Constructor de Menús

```csharp
// Funcionalidades:
- Crear menús dinámicos con ejemplos
- Organizar opciones por categorías
- Insertar código automáticamente en el editor
- Mostrar información del lenguaje
```

**Estructura del menú**:

```
📋 Estructuras del Lenguaje
├── 🔤 Palabras Reservadas
├── 📊 Tipos de Datos
│   ├── Simples (int, float, double, boolean, char)
│   └── Compuestos (string, array)
├── ⚙️ Sintaxis
│   ├── 🔄 Control de Flujo
│   ├── 📝 Funciones
│   └── 🧮 Operaciones
└── 📖 Semántica
```

#### `UI/UIConstants.cs` - Constantes de la Interfaz

```csharp
// Definiciones:
- Colores del tema (primario, secundario, errores, éxito)
- Fuentes y tamaños de texto
- Dimensiones de ventanas y controles
- Textos constantes y mensajes
- Iconos y emojis utilizados
```

---

## 🔄 FLUJO DE DATOS DEL SISTEMA

### 1. **Entrada del Usuario**

```
Usuario escribe código → MainForm.TextBox → CompilationService
```

### 2. **Proceso de Compilación**

```
CompilationService → Lexer → Parser → SymbolTable → Resultados
```

### 3. **Análisis Detallado**

```
📝 Código fuente
    ↓
🔍 Lexer.cs (Análisis Léxico)
    ↓ Lista de Tokens
🌳 Parser.cs (Análisis Sintáctico)
    ↓ Árbol AST
🧠 SymbolTable.cs (Análisis Semántico)
    ↓ Validaciones
✅ Código listo / ❌ Lista de errores
```

### 4. **Ejecución (si la compilación fue exitosa)**

```
AST válido → ExecutionService → Interpreter → Resultados
```

### 5. **Presentación de Resultados**

```
Resultados → MainForm → UI formateada → Usuario
```

---

## ⚙️ CONFIGURACIÓN TÉCNICA

### **Especificaciones del Proyecto**

```xml
<!-- KaizenLang.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>              <!-- Aplicación Windows -->
    <TargetFramework>net9.0-windows</TargetFramework>  <!-- .NET 9.0 -->
    <UseWindowsForms>true</UseWindowsForms>      <!-- Windows Forms UI -->
    <ImplicitUsings>enable</ImplicitUsings>      <!-- Using implícitos -->
    <Nullable>enable</Nullable>                  <!-- Referencias nulas -->
  </PropertyGroup>
</Project>
```

### **Dependencias**

- **.NET 9.0**: Framework base
- **Windows Forms**: Interfaz gráfica
- **System.Collections.Generic**: Estructuras de datos
- **System.Text.RegularExpressions**: Expresiones regulares para el lexer

### **Patrones de Diseño Utilizados**

1. **Factory Pattern**: `ControlFactory` para crear controles UI consistentes
2. **Service Pattern**: `CompilationService` y `ExecutionService` para separar responsabilidades
3. **Builder Pattern**: `MenuBuilder` para construir menús complejos
4. **Composite Pattern**: `Node` para representar el AST como estructura jerárquica

---

## 🎯 CARACTERÍSTICAS DESTACADAS

### ✨ **Innovaciones Implementadas**

1. **Interfaz Visual Moderna**
   - Colores y emojis para mejor experiencia de usuario
   - Menús automáticos que insertan código
   - Reportes detallados con estadísticas

2. **Análisis Robusto**
   - Tres fases de compilación bien diferenciadas
   - Detección precisa de errores con mensajes explicativos
   - Validación semántica avanzada (scope, tipos, compatibilidad)

3. **Ejemplos Integrados**
   - Menú con código automático
   - Documentación interactiva
   - Casos de prueba incluidos

### 🔧 **Manejo de Errores Avanzado**

```
📍 FASE 1: ANÁLISIS LÉXICO
❌ ERRORES LÉXICOS ENCONTRADOS:
   • Carácter no reconocido '@' en posición 15
   • String sin cerrar en línea 5

📍 FASE 2: ANÁLISIS SINTÁCTICO  
❌ ERRORES SINTÁCTICOS ENCONTRADOS:
   • Se esperaba ';' al final de la declaración en línea 8
   • Paréntesis no balanceados en expresión

📍 FASE 3: ANÁLISIS SEMÁNTICO
❌ ERRORES SEMÁNTICOS ENCONTRADOS:
   • Variable 'x' no está declarada en línea 12
   • Tipos incompatibles: no se puede asignar 'string' a 'int'
```

---

## 📊 MÉTRICAS DEL PROYECTO

### **Estadísticas de Código**

- **Archivos fuente**: 16 archivos `.cs`
- **Líneas de código**: ~2,000 líneas (estimado)
- **Clases principales**: 15+ clases
- **Métodos**: 100+ métodos
- **Fases de compilación**: 3 (Léxico, Sintáctico, Semántico)

### **Capacidades del Lenguaje**

- **Palabras reservadas**: 12
- **Tipos de datos**: 7 (5 simples + 2 compuestos)
- **Operadores**: 20+ operadores
- **Estructuras de control**: if/else, for, while, do-while
- **Funciones**: Con parámetros y valores de retorno

---

## 🚀 INSTRUCCIONES DE COMPILACIÓN

### **Compilar el Proyecto**

```bash
# Navegar al directorio del proyecto
cd KaizenLang

# Restaurar dependencias NuGet
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run
```

### **Limpiar Archivos Temporales**

```bash
# Limpiar archivos compilados
dotnet clean

# Eliminar carpetas bin y obj
rm -rf bin obj
```

---

## 📋 CUMPLIMIENTO DE REQUERIMIENTOS ACADÉMICOS

### ✅ **Estructuras del Lenguaje Implementadas**

| Requerimiento | Estado | Implementación |
|---------------|--------|----------------|
| Nombre del lenguaje | ✅ | **KaizenLang** |
| Palabras reservadas | ✅ | 12 palabras clave |
| Sintaxis de control | ✅ | if/else, for, while, do-while |
| Funciones | ✅ | Con parámetros y retorno |
| Operaciones aritméticas | ✅ | +, -, *, /, % |
| Operaciones lógicas | ✅ | &&, '||', ! |
| Entrada/salida | ✅ | input, output |
| Semántica explicada | ✅ | Documentación completa |
| 5 tipos simples | ✅ | int, float, double, boolean, char |
| 2 tipos compuestos | ✅ | string, array |

### ✅ **Reglas Adicionales Cumplidas**

| Requerimiento | Estado | Implementación |
|---------------|--------|----------------|
| Tipado estricto | ✅ | Sin conversiones implícitas |
| Sintaxis similar a C++ | ✅ | Estructura familiar |
| Validación de errores | ✅ | Todas las fases validan |
| Mensajes explicativos | ✅ | Errores detallados |

### ✅ **Pantalla de Programación**

| Componente | Estado | Descripción |
|------------|--------|-------------|
| Menú con opciones | ✅ | Menú completo con submenús |
| Área de output | ✅ | Errores y resultados formateados |
| Segmento para programar | ✅ | Editor de texto principal |
| Botón compilar | ✅ | Ejecuta las 3 fases |
| Botón ejecutar | ✅ | Interpreta código válido |

---

## 👨‍💻 INFORMACIÓN DE DESARROLLO

**Proyecto**: KaizenLang - Compilador e Intérprete Educativo  
**Curso**: Paradigmas de Programación  
**Institución**: Universidad Nacional - Sede Regional Brunca  
**Profesor**: MSc. Josías Ariel Chaves Murillo  
**Framework**: .NET 9.0 con Windows Forms  
**Lenguaje**: C#  

---

<p align="center">
  <strong>🌟 Documentación técnica completa del proyecto KaizenLang 🌟</strong><br>
  <em>Mejora continua en el aprendizaje de compiladores</em>
</p>
