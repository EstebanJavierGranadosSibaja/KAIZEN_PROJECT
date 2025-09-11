# KaizenLang - Lenguaje de Programación Educativo

<p align="center">
  <img src="https://img.shields.io/badge/Version-1.0-blue.svg" alt="Version">
  <img src="https://img.shields.io/badge/.NET-9.0-purple.svg" alt=".NET Version">
  <img src="https://img.shields.io/badge/UI-WinForms-green.svg" alt="UI Framework">
  <img src="https://img.shields.io/badge/License-Educational-yellow.svg" alt="License">
</p>

## 📋 Descripción del Proyecto

**KaizenLang** es un lenguaje de programación educativo desarrollado como proyecto académico para la materia **Paradigmas de Programación**. Implementa un compilador completo con análisis léxico, sintáctico y semántico, junto con un intérprete básico y una interfaz gráfica moderna.

### 🎯 Objetivos Académicos

- **Comprensión práctica** de las fases de compilación
- **Implementación** de un analizador léxico, sintáctico y semántico
- **Desarrollo** de un entorno de programación visual
- **Aplicación** de principios de diseño de lenguajes

## 🚀 Características Principales

### ✨ Compilador Completo
- **Análisis Léxico**: Reconocimiento de tokens (palabras reservadas, identificadores, operadores, literales)
- **Análisis Sintáctico**: Verificación de gramática y construcción del AST
- **Análisis Semántico**: Validación de tipos, scope y coherencia lógica
- **Manejo de Errores**: Reportes detallados en cada fase

### 🎨 Interfaz Gráfica Moderna
- **Editor de código** con sintaxis highlighting visual
- **Menú interactivo** con ejemplos automáticos
- **Output formateado** con colores y iconos
- **Botones de acción** (Compilar y Ejecutar)

### 🔧 Características del Lenguaje

#### Palabras Reservadas
```
output, input, void, do, while, for, if, else, return, true, false, null
```

#### Tipos de Datos

**Simples (5):**
- `int` - Números enteros
- `float` - Números decimales
- `double` - Precisión doble
- `boolean` - Valores lógicos (true/false)
- `char` - Caracteres individuales

**Compuestos (2):**
- `string` - Cadenas de texto
- `array` - Arreglos de elementos

#### Operadores
- **Aritméticos**: `+`, `-`, `*`, `/`, `%`
- **Comparación**: `==`, `!=`, `<`, `>`, `<=`, `>=`
- **Lógicos**: `&&`, `||`, `!`
- **Asignación**: `=`, `+=`, `-=`, `*=`, `/=`

## 💻 Ejemplos de Código

### Declaraciones Básicas
```kaizen
int numero = 42;
string mensaje = "Hola KaizenLang";
boolean activo = true;
float precio = 19.99;
```

### Estructuras de Control
```kaizen
if (numero > 0) {
    // código cuando es positivo
} else {
    // código cuando es negativo
}

while (contador < 5) {
    contador = contador + 1;
}

for (int i = 0; i < 10; i++) {
    // código del bucle
}
```

### Funciones
```kaizen
int suma(int a, int b) {
    return a + b;
}

void saludar(string nombre) {
    // lógica de saludo
}
```

## 🏗️ Arquitectura del Proyecto

```
KaizenLang/
├── src/KaizenLang/
│   ├── Lexer.cs              # Análisis léxico
│   ├── Parser.cs             # Análisis sintáctico
│   ├── Interpreter.cs        # Intérprete
│   ├── ATS/
│   │   └── Node.cs           # Nodos del AST
│   ├── Semantic/
│   │   └── SymbolTable.cs    # Análisis semántico
│   ├── Tokens/
│   │   ├── Token.cs          # Definición de tokens
│   │   ├── ReservedWords.cs  # Palabras reservadas
│   │   ├── TypeWords.cs      # Tipos de datos
│   │   ├── OperatorWords.cs  # Operadores
│   │   └── DelimiterWords.cs # Delimitadores
│   └── UI/
│       └── MainForm.cs       # Interfaz gráfica
├── Examples/
│   └── example.txt           # Ejemplos de código
└── docs/
    └── descripción-proyecto.md
```

## 🔧 Tecnologías Utilizadas

- **.NET 9.0** - Framework de desarrollo
- **C#** - Lenguaje de programación
- **Windows Forms** - Interfaz gráfica
- **Visual Studio Code** - Entorno de desarrollo

## 📦 Instalación y Ejecución

### Prerrequisitos
- .NET 9.0 SDK
- Windows 10/11
- Git (opcional)

### Pasos de Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/KAIZEN_PROJECT.git
cd KAIZEN_PROJECT/KaizenLang
```

2. **Restaurar dependencias**
```bash
dotnet restore
```

3. **Compilar el proyecto**
```bash
dotnet build
```

4. **Ejecutar la aplicación**
```bash
dotnet run
```

## 🎮 Uso de la Aplicación

### 1. Escribir Código
- Utiliza el área de texto principal para escribir código en KaizenLang
- Usa el menú "Estructuras del Lenguaje" para insertar ejemplos automáticamente

### 2. Compilar
- Presiona el botón **"🛠 Compilar"** para ejecutar las tres fases:
  - Análisis léxico
  - Análisis sintáctico  
  - Análisis semántico
- Revisa los errores en el área de output

### 3. Ejecutar
- Presiona el botón **"▶ Ejecutar"** para interpretar el código
- Solo funciona si la compilación fue exitosa

## 🧪 Validaciones Implementadas

### ✅ Validaciones Léxicas
- Reconocimiento correcto de tokens
- Detección de caracteres inválidos
- Manejo de strings y comentarios

### ✅ Validaciones Sintácticas
- Verificación de gramática correcta
- Estructura de bloques balanceados
- Parámetros de funciones válidos

### ✅ Validaciones Semánticas
- **Tipado estricto**: Variables deben declararse con tipo
- **Scope de variables**: Variables deben existir en el contexto
- **Compatibilidad de tipos**: Operaciones entre tipos compatibles
- **Declaraciones únicas**: No redeclaración de variables

## 🐛 Manejo de Errores

El compilador proporciona mensajes de error detallados:

```
❌ ERRORES LÉXICOS ENCONTRADOS:
   • Carácter no reconocido '@' en posición 15

❌ ERRORES SINTÁCTICOS ENCONTRADOS:
   • Se esperaba ';' al final de la declaración

❌ ERRORES SEMÁNTICOS ENCONTRADOS:
   • Variable 'x' no está declarada
   • Tipos incompatibles en asignación
```

## 📚 Ejemplos de Prueba

### Código Válido
```kaizen
int x = 10;
int y = 20;
int suma = x + y;
```

### Errores Comunes
```kaizen
// Error léxico
string texto = "sin cerrar

// Error sintáctico  
int numero = 5  // falta ;

// Error semántico
z = 10; // variable no declarada
```

## 👥 Equipo de Desarrollo

- **Desarrollador Principal**: [Tu Nombre]
- **Curso**: Paradigmas de Programación
- **Institución**: Universidad Nacional - Sede Regional Brunca
- **Profesor**: MSc. Josías Ariel Chaves Murillo

## 📄 Documentación Adicional

- [`docs/descripción-proyecto.md`](docs/descripción-proyecto.md) - Especificaciones del proyecto
- [`Examples/example.txt`](Examples/example.txt) - Ejemplos de código completos

## 🎯 Evaluación Académica

Este proyecto cumple con los criterios de evaluación:

- ✅ **Documentación** (10%)
- ✅ **Planteamiento y solución** (20%) 
- ✅ **Funcionamiento del aplicativo** (60%)
- ✅ **Defensa del proyecto** (10%)

## 🔄 Próximas Mejoras

- [ ] Optimización del AST
- [ ] Más tipos de datos compuestos
- [ ] Funciones de entrada/salida reales
- [ ] Generación de código intermedio
- [ ] Debugger integrado

## 📞 Contacto

Para preguntas sobre el proyecto:
- **Email**: [tu-email@universidad.ac.cr]
- **GitHub**: [tu-usuario]

---

<p align="center">
  <strong>🌟 KaizenLang - Mejora Continua en el Aprendizaje de Compiladores 🌟</strong>
</p>
