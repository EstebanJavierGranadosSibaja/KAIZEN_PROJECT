<!-- markdownlint-disable MD033 -->

#

<div align="center" style="margin-top: 40px; margin-bottom: 40px;">

<img src="UNA Logo2.png" alt="UNA Logo" width="100" style="margin-right: 24px; vertical-align: middle;">
<img src="../Resources/icon.ico" alt="KaizenLang Logo" width="80" style="vertical-align: middle;">

<br><br>

<div align="center">

<strong style="font-size:2em;">Lenguaje de Programación Kaizen</strong>

<br><br>

<div style="font-size:1.3em;">
    <strong>Estudiantes:</strong><br>
    Juan Carlos Camacho Solano<br>
    Esteban Granados Sibaja
</div>

<br>

<div style="font-size:1.3em;">
    <strong>Universidad:</strong><br>
    Universidad Nacional (UNA)
</div>

<br>

<div style="font-size:1.3em;">
    <strong>Profesor:</strong><br>
    Josías Ariel Chaves Murillo
</div>

<br>

<div style="font-size:1.3em;">
    <strong>Curso:</strong><br>
    Paradigmas de Programación
</div>

<br>

<div style="font-size:1.3em;">
    <strong>Ciclo:</strong><br>
    II 2025
</div>

</div>

<div style="page-break-after: always;"></div>

## Sobre el lenguaje

### Problematica planteada

Se quiere desarrollar un lenguaje que se sienta diferente a otros, que tenga su propia esencia. Por eso se define el lenguaje bajo el concepto de tematica de cultura japonesa.

Se busca tambien se busca integrar la familiaridad de estructura de C++ para que sea mas sensilo de usar.

### Solucion propuesta

La solucion fue usar un lenguaje que tenga identidad propia usando palabras reservadas diferentes y elementos diferentes a C++ para que se sienta mas fresco. Ademas se integra con un ide con varias ejemplos de codigo para que se pueda iniciar a programar.

## Descripccion del lenguaje

**KaizenLang** es un lenguaje de programación educativo desarrollado como proyecto académico para el curso de Paradigmas de Programación de la Universidad Nacional de Costa Rica. El proyecto implementa un compilador e intérprete completo con IDE integrado.

### Filosofía del Proyecto

El nombre "Kaizen" (改善) proviene del japonés y significa "mejora continua". Este lenguaje fue diseñado para enseñar conceptos fundamentales de programación con una sintaxis clara y tipado estricto que previene errores comunes.

### Particularidades del Lenguaje

Otra de las cosas del lenguaje es el nivel de anidadcion que se permite, suporta un maximo de 512 anidaciones teoricas, pensado en que se pueda hacer codigo complejo sin problemas.

Usa la palabra reservada yin y yan para indica el scope de codigo, orientado en la cultura japonesa.

Usa nombres de tipos de datos orientadas en el anime de One piece:

- gear - Números enteros
- shikai - Números decimales de precisión simple
- bankai - Números decimales de doble precisión
- grimoire - Cadenas de texto
- shin - Valores booleanos (true/false)

- chainsaw - Array/Lista dinámica
- hogyoku - Matriz

El reso de se mantiene segun el entandar del ingles

## Problematicas de dearrollo

Además de mencionar que fue lo que más costó en el desarrollo, en este caso fueron los inputs y outputs, errores con límites no bien definidos para los bucles y la recursión infinita, problemas con la interfaz gráfica para que fuera agradable y se viera como queríamos.

Hubo problemas con el manejo de las referencias de vectores, ya que para un valor entero, de una matriz o vector entero, al intentar acceder a una posición realmente devolvía todo el arr y no arr[]. También se presentaron dificultades para lograr manejar adecuadamente las diferencias entre el float y double.

Se encontraron problemas con la semántica para lograr detectar errores (incluso puede ser que hayan errores por tratar), además de problemas con el entorno, ya que se tuvo que refactorizar la estructura del proyecto más de una vez para que fuera más manejable y entendible.

En cuanto al parser, al generar los lexemas (parte en la que no hubo mayor problema) se complicaba poder definir cómo se iban a manejar dichos lexemas para posteriormente hacer una revisión de la sintaxis completa del código. Al momento de pasarlo del parser a la semántica no hubo mayor complicación que la anteriormente mencionada.

Se presentaron problemas con las funciones, ya que en un principio no manejaba bien los retornos y los tipos. Al final se logró solucionar usando un enfoque simple y similar a C++.

Finalmente, hubo problemas al querer agregar la anidación hasta N, ya que se complicó lograr identificar realmente dónde termina y que los bloques de control dentro de otros fueran tomados como uno por aparte aunque estén dentro de un conjunto de bloques. Al final lo que se hizo fue buscar el último bloque donde dentro de él no se encontrara otro bloque, desde ese punto, se enlistaba el último anterior para ir sacando cada uno de los bloques en orden (se debe especificar cómo se solucionaron).

## Estructura del proyecto

### Características

| Característica                | Descripción                                                                                     |
| ----------------------------- | ----------------------------------------------------------------------------------------------- |
| **Tipado estricto**           | No conversiones implícitas peligrosas, todas las variables deben declararse explícitamente      |
| **Sintaxis única**            | Usa `ying` y `yang` como delimitadores de bloques, inspirados en el concepto de balance         |
| **Tipos de datos simples**    | `gear`, `shikai`, `bankai`, `shin`, `grimoire`                                                  |
| **Tipos de datos compuestos** | `chainsaw<T>`, `hogyoku<T>` (colecciones 1D y 2D)                                               |
| **Estructuras de control**    | `if-else`, `while`, `for`                                                                       |
| **Funciones**                 | Con retorno de valores y funciones `void`                                                       |
| **Operaciones**               | Aritméticas (`+`, `-`, `*`, `/`), lógicas (`&&`, `ll`, `!`), comparación (`>`, `<`, `==`, etc.) |
| **Entrada/Salida**            | Funciones builtin `input()` y `output()`                                                        |

### IDE Integrado

- **Editor con syntax highlighting**: Resalta palabras clave, tipos, operadores y literales
- **Compilación en tiempo real**: Detecta errores de sintaxis y semánticos
- **Ejecución interactiva**: Interpreta código directamente desde el IDE
- **Generación automática de código**: Menú con snippets para todas las estructuras
- **Tema oscuro moderno**: Inspirado en el logo Kaizen con paleta verde y dorado
- **Barra de estado inteligente**: Muestra línea/columna, estado de compilación y hora

### Arquitectura

#### Flujo de Compilación

| Etapa                    | Descripción                                     | Entrada             | Salida                     |
| ------------------------ | ----------------------------------------------- | ------------------- | -------------------------- |
| **Código Fuente**        | Código escrito por el usuario                   | -                   | Código fuente              |
| **Lexer (Tokenización)** | Convierte el código en tokens con línea/columna | Código fuente       | Tokens                     |
| **Parser (Sintáctico)**  | Analiza tokens y genera el AST                  | Tokens              | AST (Árbol Sintáctico)     |
| **Semantic Analyzer**    | Valida tipos y símbolos                         | AST                 | AST validado               |
| **Interpreter**          | Ejecuta el AST                                  | AST validado        | Resultado de ejecución     |
| **Output / Errores**     | Muestra resultados o errores                    | Resultado ejecución | Salida o mensajes de error |

## Particularidades del lenguaje

## Innovación del lenguaje

El lenguaje KaizenLang introduce varias innovaciones que lo distinguen de otros lenguajes de programación convencionales:

1. **Temática Cultural**: La integración de términos y conceptos de la cultura japonesa no solo aporta una identidad única al lenguaje, sino que también puede hacer que el aprendizaje de la programación sea más atractivo y memorable.

   - ying/yang - Define bloques de código (similar a llaves {})
   - gear - Números enteros
   - shikai - Números decimales de precisión simple
   - bankai - Números decimales de doble precisión
   - grimoire - Cadenas de texto
   - shin - Valores booleanos (true/false)

   - chainsaw - Array/Lista dinámica
   - hogyoku - Matriz

2. **Sintaxis Clara y Concisa**: La sintaxis de KaizenLang está diseñada para ser intuitiva y fácil de entender, lo que facilita la curva de aprendizaje para los nuevos programadores. Usa pocas palabras reservasdas, unas 20, para la facildidad de uso con el ide integrado que indica como usarlas desde la interfaz principal.

3. **Integra un ide personalizado**: El IDE integrado proporciona un entorno de desarrollo amigable con características como resaltado de sintaxis, autocompletado y depuración, lo que mejora la experiencia del usuario y facilita el proceso de codificación.

4. **Recurcion y Anidacion avanzada**: Soporta un alto nivel de anidación (hasta 512 niveles teóricos), permitiendo a los desarrolladores crear estructuras de código complejas sin restricciones significativas.

5. **IDE Temático**: El IDE no solo es funcional, sino que también está diseñado con una estética que refleja la filosofía de Kaizen, utilizando una paleta de colores inspirada en la cultura japonesa. Ademas de integrar comandos facilitadores.

6. **Tiene su propio logotipo**: El logotipo de KaizenLang es una representación visual de su filosofía de mejora continua y su identidad cultural. El diseño incorpora elementos tradicionales japoneses, como el símbolo del yin-yang, para reflejar el equilibrio y la armonía en la programación.

<p align="center">
    <img src="../Resources/icon.ico" alt="KaizenLang Logo" width="60">
</p>

## GUIA DE USO

### 💻 Uso

#### Ejecutar el IDE

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

#### Herramientas de Desarrollo

1. **CompilationTester** - Testing E2E desde consola

   ```bash
   dotnet run --project tools/CompilationTester/CompilationTester.csproj
   ```

   Muestra tokens, AST completo y errores semánticos para snippets de código.

2. **AstDump** - Volcado de AST para debugging

   ```bash
   dotnet run --project tools/AstDump/AstDump.csproj
   ```

3. **IDERunner** - Ejecución de archivos `.kaizen`

   ```bash
   dotnet run --project tools/IDERunner/IDERunner.csproj -- sample.kaizen
   ```

4. **QuickRunner** - Ejecución rápida de snippets

   ```bash
   dotnet run --project tools/QuickRunner/QuickRunner.csproj
   ```

## Sintaxis

### Declaración de Variables

```kaizen
// Tipado explícito obligatorio
gear edad = 25;
shikai altura = 1.75;
bankai temperatura = 36.6;
shin esEstudiante = true;
grimoire nombre = "Ana";
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
gear contador = 0;
while (contador < 5) ying
    output("Iteración: " + contador);
    contador = contador + 1;
yang

// Bucle for
for (gear i = 0; i < 10; i = i + 1) ying
    output("Número: " + i);
yang
```

### Funciones

```kaizen
// Función con retorno
gear suma(gear a, gear b) ying
    return a + b;
yang

// Función void
void saludar(grimoire nombre) ying
    output("Hola " + nombre + "!");
yang

// Llamadas a funciones
gear resultado = suma(10, 5);
saludar("KaizenLang");
```

### Chainsaw y Hogyoku

```kaizen
// Chainsaw con tipo genérico
chainsaw<gear> numeros = [1, 2, 3, 4, 5];
chainsaw<grimoire> nombres = ["Ana", "Luis", "Pedro"];

// Acceso por índice
gear primero = numeros[0];
grimoire persona = nombres[1];

// Hogyoku (colecciones 2D)
hogyoku<gear> tabla = [
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9]
];

gear elemento = tabla[0][1];  // Valor: 2
```

### Entrada y Salida

```kaizen
// Solicitar entrada del usuario
grimoire nombre = input();
output("Hola " + nombre);

// Obtener longitud de un chainsaw
chainsaw<gear> datos = [10, 20, 30];
gear tam = length(datos);
output("Tamaño del chainsaw: " + tam);
```

### Operaciones

```kaizen
// Aritméticas
gear x = 10 + 5 * 2;     // 20
shikai y = 10.0 / 3.0;        // 3.333...

// Comparación
shin mayorQue = (x > 15);    // true
shin igual = (y == 3.33);    // false

// Lógicas
shin resultado = (x > 10) && (y < 5);  // true
shin negacion = !resultado;             // false
```

## Ejemplos de Código
