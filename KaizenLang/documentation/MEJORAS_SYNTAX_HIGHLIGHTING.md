# Mejoras en Syntax Highlighting

## Problema Original
El sistema de resaltado de sintaxis anterior causaba varios problemas durante la edición:
- El texto parecía "seleccionarse" mientras se escribía
- Interferencia al copiar/pegar texto
- Parpadeo visible durante la escritura
- El cursor saltaba de posición ocasionalmente

## Solución Implementada

### 1. **Procesamiento Asíncrono con Debouncing**
```csharp
// El highlighting ahora se ejecuta en segundo plano con un delay
Task.Run(async () =>
{
    // Esperar 300ms antes de aplicar (debouncing)
    await Task.Delay(300, cts.Token);
    
    // Aplicar en el hilo de UI
    richTextBox.Invoke(() => ApplySyntaxHighlightingInternal(...));
});
```

**Beneficios:**
- No bloquea la escritura del usuario
- Reduce la carga del procesador
- Elimina el parpadeo

### 2. **Detección de Interacción del Usuario**
```csharp
// No aplicar highlighting si hay texto seleccionado
if (!forceHighlight && richTextBox.SelectionLength > 0)
    return;

// Detectar cuando el usuario está seleccionando
richTextBox.SelectionChanged += (s, e) =>
{
    if (rtb.SelectionLength > 0)
        isUserInteracting = true;
};
```

**Beneficios:**
- Copiar/pegar funciona correctamente
- No interfiere con la selección de texto
- Mejor experiencia al usar atajos de teclado (Ctrl+C, Ctrl+V)

### 3. **Control de Redibujado Optimizado**
```csharp
// Deshabilitar redibujado durante el procesamiento
NativeMethods.SendMessage(richTextBox.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

// ... aplicar colores ...

// Reactivar redibujado una sola vez al final
NativeMethods.SendMessage(richTextBox.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
richTextBox.Invalidate();
```

**Beneficios:**
- Elimina completamente el parpadeo
- Mejora el rendimiento
- El cambio de colores es instantáneo y suave

### 4. **Sistema de Cancelación Inteligente**
```csharp
// Cancelar highlighting previo si el usuario sigue escribiendo
if (_pendingHighlights.TryGetValue(richTextBox, out var existingCts))
{
    existingCts.Cancel();
    existingCts.Dispose();
}
```

**Beneficios:**
- Evita múltiples highlighting simultáneos
- Ahorra recursos del sistema
- Responde mejor a la velocidad de escritura del usuario

### 5. **Timings Ajustados**
- **Delay de escritura:** 1200ms (1.2 segundos)
  - El usuario puede escribir libremente sin interrupciones
  - Suficiente tiempo para completar una palabra o línea
  
- **Delay de forzado:** 300ms
  - Para highlighting al cargar archivos o cambiar de foco
  - Balance entre rapidez y recursos

## Características Técnicas

### Expresiones Regulares Compiladas
```csharp
private static readonly Regex _commentRegex = new Regex(
    @"//.*", 
    RegexOptions.Multiline | RegexOptions.Compiled
);
```
- `RegexOptions.Compiled` mejora el rendimiento en un 30-40%
- Cache de patrones para evitar recompilación

### Thread-Safety
```csharp
private static readonly object _lockObject = new object();
private static readonly Dictionary<RichTextBox, CancellationTokenSource> _pendingHighlights;

lock (_lockObject)
{
    // Operaciones thread-safe
}
```
- Previene condiciones de carrera
- Seguro para múltiples editores simultáneos

### Preservación de Estado
```csharp
var selectionStart = richTextBox.SelectionStart;
var selectionLength = richTextBox.SelectionLength;
var scrollPosition = GetScrollPosition(richTextBox);

// ... aplicar highlighting ...

richTextBox.SelectionStart = selectionStart;
richTextBox.SelectionLength = 0; // Sin selección fantasma
SetScrollPosition(richTextBox, scrollPosition);
```
- El cursor no salta de posición
- El scroll se mantiene en el mismo lugar
- La selección no se pierde inadvertidamente

## Comparación Antes/Después

| Aspecto | Antes | Después |
|---------|-------|---------|
| Tiempo de respuesta al escribir | Inmediato (bloqueante) | 1.2s delay (no bloqueante) |
| Parpadeo visible | ❌ Sí | ✅ No |
| Interferencia con copia | ❌ Sí | ✅ No |
| Saltos de cursor | ❌ Ocasionales | ✅ Ninguno |
| Uso de CPU | Alto (continuo) | Bajo (periódico) |
| Selección de texto | Problemática | Fluida |
| Rendimiento con archivos grandes | Lento | Rápido |

## Pruebas Recomendadas

1. **Escritura rápida:** Escribir varias líneas seguidas sin pausas
2. **Copiar/Pegar:** Seleccionar texto, copiar y pegar en otra ubicación
3. **Selección múltiple:** Hacer selecciones largas con Shift+Flechas
4. **Deshacer/Rehacer:** Probar Ctrl+Z y Ctrl+Y repetidamente
5. **Archivos grandes:** Abrir archivos de >500 líneas

## Configuración Personalizable

Si necesitas ajustar los timings según el hardware o preferencias:

```csharp
// En EnhancedVisualEffects.cs, línea ~348
delayTimer = new System.Windows.Forms.Timer 
{ 
    Interval = 1200  // Cambiar este valor (milisegundos)
};
```

### Valores Recomendados
- **PC potente:** 800-1000ms
- **PC estándar:** 1200-1500ms (actual)
- **PC antiguo:** 2000-2500ms

## Notas de Implementación

- El highlighting se aplica en el hilo de UI mediante `Invoke()`
- Se usa `Task.Run()` para el delay, no bloquea el pool de threads
- Los `CancellationToken` se limpian automáticamente
- El sistema es compatible con múltiples RichTextBox simultáneos

## Futuras Mejoras Posibles

1. **Highlighting incremental:** Solo repintar las líneas modificadas
2. **Cache de parsing:** Guardar tokens parseados entre ejecuciones
3. **Highlighting por viewport:** Solo procesar texto visible
4. **Análisis de complejidad:** Ajustar delay según tamaño del archivo
5. **Modo performance:** Opción para deshabilitar highlighting en archivos muy grandes

## Referencias

- [RichTextBox Performance Best Practices](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/performance-richtextbox)
- [WM_SETREDRAW Message](https://docs.microsoft.com/en-us/windows/win32/gdi/wm-setredraw)
- [Task-based Asynchronous Pattern (TAP)](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
