# ParadigmasLang

Este proyecto es la base para crear un lenguaje de programación propio en C#, siguiendo las reglas del documento 'Descripción del Proyecto Paradigmas'.

## Estructura

- `Lexer.cs`: Analizador léxico
- `Parser.cs`: Analizador sintáctico
- `Interpreter.cs`: Intérprete
- `Program.cs`: Punto de entrada

## Cómo empezar

1. Abre la solución en Visual Studio o VS Code.
2. Implementa las reglas del lenguaje en los archivos correspondientes.
3. Ejecuta y prueba con ejemplos en la carpeta `Examples`.

## Reglas

Las reglas y paradigmas a seguir están descritas en el documento PDF adjunto.

```txt
src/KaizenLang/
│
├── Lexer.cs           // Analizador léxico
├── Parser.cs          // Analizador sintáctico
├── Interpreter.cs     // Intérprete/Ejecutor
├── Program.cs         // Punto de entrada (UI)
│
├── AST/               // Representación del árbol de sintaxis abstracta
│   └── Node.cs
│
├── Tokens/            // Definiciones de tokens y tipos
│   └── Token.cs
│
├── Semantic/          // Análisis semántico y símbolos
│   └── SymbolTable.cs
│
└── UI/                // Interfaz gráfica o de línea de comandos
    └── ...
```
