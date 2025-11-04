# UNIVERSIDAD NACIONAL

**SEDE REGIONAL BRUNCA - CAMPUS COTO**
**PROF. MSC. JOSÍAS ARIEL CHAVES MURILLO**
**CURSO PARADIGMAS DE PROGRAMACIÓN**

---

## PROYECTO #1

## Objetivo

El trabajo práctico de esta asignatura busca enfrentar al estudiantado con los problemas derivados de tener que programar cómo funciona un lenguaje de programación.

## Descripción

Los equipos de proyecto deberán desarrollar su propio lenguaje de programación. Para ello pueden usar el entorno de programación que deseen, pero se debe cumplir lo siguiente a nivel de requerimientos:

### Estructuras del lenguaje

Cada grupo de proyecto deberá crear las siguientes estructuras del lenguaje e incluirlas en la documentación:

- **Nombre**
- **Palabras reservadas**
- **Sintaxis**
  - Control: instrucciones, condicionales simples y múltiples, ciclos for y while.
  - Funciones: que retornen valores y funciones vacías.
  - Operaciones: aritméticas (sumas, restas, multiplicaciones, divisiones), lógicas (and, or, not).
  - Entrada y salida de datos: leer y escribir variables.
- **Semántica**
  - Explicación de cómo funciona el lenguaje (función principal, indentación obligatoria, etc.).
- **Tipos de datos**
  - 5 simples: enteros, flotantes, caracteres, booleanos y nulos.
  - 2 compuestos: arreglos, listas, matrices, etc.

### Reglas adicionales del lenguaje

- El tipado de datos debe ser **estricto**, no se permiten conversiones implícitas peligrosas.
- El lenguaje debe diseñarse con **sintaxis propia** (no debe ser idéntico ni pretender replicar exactamente C, C++ ni otro lenguaje existente). Debe priorizar claridad, consistencia y facilidad de análisis (esto facilita la implementación del compilador/interprete).
- El lenguaje debe **validar cualquier tipo de error** en tiempo de compilación y ejecución, mostrando mensajes explicativos al usuario.

### Pantalla de programación

El programa a desarrollar deberá tener:

- Un menú con opciones.
- Un área de output para errores.
- Un segmento para programar.
- Dos botones: **compilar** y **ejecutar**.

### Menú de opciones

El aplicativo deberá mostrar de forma clara y visualmente agradable:

- Palabras reservadas
- Sintaxis (con submenús: control, funciones, operaciones)
- Semántica
- Tipos de datos

Cada segmento deberá poder **generar código automático** en el área de escritura.

### Output

Debe mostrar errores de sintaxis detectados y, en caso de éxito, la salida del aplicativo.

### Botones

- **Compilar**: revisa el código según tablas y procesos de compilación.
- **Ejecutar**: ejecuta el código y le da semántica.

---

## Instrucciones Generales

1. Trabajo en equipos de máximo 2 personas.
2. Para la documentación se evaluará:
   - Planteamiento del problema y solución adoptada.
   - Descripción detallada del lenguaje construido.
3. Solo se reciben documentos en **PDF**.

**Fecha de entrega:** la indicada en el programa.

---

## RÚBRICA DE EVALUACIÓN DE PROYECTOS

| Rubros | Valor |
|--------|-------|
| Documentación | 10% |
| Planteamiento y solución adoptada | 10% |
| Funcionamiento del aplicativo | 70% |
| Defensa del proyecto | 10% |
| **TOTAL** | **100%** |

---

## Detalle de la Rúbrica

### A. Documentación (10 puntos)

1. Ortografía (3 pts)
2. Redacción (2 pts)
3. Formatos consistentes (5 pts)

### B. Planteamiento del problema y solución (20 puntos)

| Criterio | Puntos |
|----------|---------|
| Pobre planteamiento y solución | 0 – 5 |
| Planteamiento y solución presentes, pero insuficientes | 6 – 15 |
| Bien definidos ambos | 16 – 20 |

### C. Funcionamiento del aplicativo (60 puntos)

**1. Innovación en el desarrollo (10 pts)**

- Sin innovación ni esfuerzo: 0 – 3
- Innovación parcial: 4 – 7
- Buena innovación y dedicación: 7 – 10

**2. Manejo de validaciones (20 pts)**

- Sin validaciones: 0 – 5
- Validaciones parciales: 5 – 15
- Buen manejo de excepciones y mensajes: 15 – 20

**3. Funcionamiento integral (40 pts)**

- No desarrollado o con errores graves: 0 – 5
- Desarrollo parcial con errores: 6 – 10
- Mitad del aplicativo funcional: 11 – 20
- Desarrollo casi completo pero con faltantes: 21 – 30
- Aplicativo completo con detalles menores: 31 – 40

### D. Defensa del proyecto (10 puntos)

- No responde o desconoce su desarrollo: 0 – 3
- Explica con dudas: 4 – 7
- Explica claramente: 7 – 10

**Nota:** La defensa es **individual**.

---

## Actualización técnica del repositorio (resumen breve)

Nota: esta sección documenta cambios recientes hechos al código fuente y herramientas de desarrollo — útil para entrega, mantenimiento y para el profesor.

- Fecha: Septiembre 2025
- Estado actual: desarrollo activo, UI funcional, y herramienta de pruebas automatizada integrada en `tools/CompilationTester`.

### Interfaz: nuevo tema oscuro

Se añadió un tema oscuro inspirado en el logo de KAIZEN (ver assets/logo). La nueva interfaz prioriza contraste suave para largas sesiones de edición y utiliza detalles en verde y dorado del logotipo para acentos (botones y highlights).

Características principales del IDE oscuro:
- Paleta oscura centralizada: fondos muy oscuros para el editor y paneles, texto claro para legibilidad.
- Acentos KAIZEN: botón de compilación en verde KAIZEN, botón de ejecución en dorado; pequeños degradados y sombras sutiles para profundidad.
- Paneles redondeados y separación visual entre editor, salida y menú.
- Inputs no bloqueantes: la función builtin `input()` se implementa vía `ExecutionService.InputProvider` y se muestra con un diálogo no bloqueante en la UI.
- Soporte visual para numeración de líneas y autocompletado básico en el editor.

Se agregó `docs/theme.css` con tokens de color para documentación y mockups web, y `src/KaizenLang.UI/UIConstants.cs` contiene los tokens de color y fuentes usados por la aplicación.

Cambios e implementaciones importantes
- Reorganización del código
  - El compilador/interpretador fue reestructurado en carpetas claras: `Lexeme` (lexer/charstream/tokenizer), `Syntax` (parser), `Semantic` (análisis semántico y tabla de símbolos) y `Interpreter` (ejecutor). Esto mejora la mantenibilidad y la separación de responsabilidades.

- Correcciones en el análisis léxico y sintáctico
  - Se corrigió la tokenización de cadenas (strings) y se añadió trazabilidad de posición: `CharStream` y `Token` ahora registran `Line` y `Column` para todos los tokens, lo que permite errores y warnings con ubicación exacta.
  - El parser ahora construye nodos AST con posición (Line/Column) cuando corresponde.

- Mejoras en análisis semántico
  - El `SemanticAnalyzer` ahora propaga ubicaciones (línea/columna) en mensajes de error y reconoce correctamente llamadas builtin como `input()` y `output()` sin requerir declaraciones previas.
  - Se mejoró la inferencia de tipos en expresiones y se añadió una excepción controlada para inicializaciones provenientes de `input()` (la conversión se delega al runtime/interpreter).

- Runtime / Interpreter
  - El `Interpreter` realiza conversiones de token a tipo declarado en tiempo de ejecución (por ejemplo, convertir la cadena leída por `input()` a `gear` si la variable es `gear`).
  - Se mantiene manejo de buffer de entrada y proveedor de entradas desde UI para compatibilidad con la interfaz gráfica.

- Herramienta de pruebas E2E (console)
  - Se añadió `tools/CompilationTester` (aplicación de consola) para ejecutar el pipeline (lex → parse → semantic) en snippets y obtener:
    - Lista de tokens con `Line:Column`
    - AST completo (`ToTreeString()`)
    - Mensajes semánticos enriquecidos
  - Uso rápido (desde PowerShell):

```powershell
dotnet build "tools/CompilationTester/CompilationTester.csproj"
dotnet run --project "tools/CompilationTester/CompilationTester.csproj" --no-build
```

Notas operativas y decisiones temporales
- Para permitir que la herramienta de pruebas referencie el proyecto principal sin conflicto de puntos de entrada, se aplicó un cambio temporal al `KaizenLang.csproj` (OutputType cambiado a `Library`).
  - Recomendación: revertir ese cambio antes de publicar la versión final de la aplicación (dejar `WinExe` para la UI) y, en su lugar, hacer que `tools/CompilationTester` use la DLL compilada o un mecanismo de artefacto local (evita tener dos mains en proyectos referenciados).

Próximos pasos recomendados (opciones)
- Revertir `KaizenLang.csproj` a ejecutable y actualizar `tools/CompilationTester` para usar la DLL de salida o un local package/reference.
- Normalizar la representación de llamadas a funciones en el parser (usar un solo tipo `FunctionCall`) para simplificar semántica futura.
- Añadir pruebas automatizadas (xUnit) que verifiquen los snippets problemáticos (por ejemplo, el caso `input()` + asignación + `output()`), y que aseguren que tokens/AST/errores se mantengan estables.
- Actualizar README y documentación en `docs/` con instrucciones de desarrollo y ejecución (cómo compilar, cómo ejecutar la UI y la tester, y cómo ejecutar las pruebas).

Si quieren, puedo aplicar alguno de los pasos siguientes automáticamente (revertir `OutputType` y actualizar la referencia del tester para usar la DLL, o bien añadir una prueba xUnit para el snippet). Dime cuál prefieres y lo hago.

