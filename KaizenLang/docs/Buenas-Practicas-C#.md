# Buenas Prácticas en "C#"

## 1. Convenciones de Nombres

- **Clases, Propiedades y Métodos:** Usa PascalCase.  
    Ejemplo: `Cliente`, `ObtenerResultado()`
- **Variables locales y parámetros:** Usa camelCase.  
    Ejemplo: `valorTotal`, `nombreCompleto`
- **Campos privados estandar recomendado Microsoft:** Usa camelCase.  
    Ejemplo: `contador`, `conexionDb`
- **Campos privados estandar Proyectos grandes:** Usa camelCase con prefijo `_`.  
    Ejemplo: `_contador`, `_conexionDb`
- **Constantes:** Usa MAYÚSCULAS_CON_GUIONES.  
    Ejemplo: `MAXIMO_VALOR`
- **Interfaces:** Prefijo `I` seguido de PascalCase.  
    Ejemplo: `IDisposable`, `IRepositorioClientes`

## 2. Estructura del Código

- **Organiza los namespaces** jerárquicamente (Company.Product.Module.Feature).
- **Mantén un archivo por clase/interfaz**.
- **Evita el uso de regiones** (`#region`); prefiere clases más pequeñas.
- **Orden de miembros**: campos, propiedades, constructores, métodos.
- **Agrupa métodos** relacionados funcionalmente.

## 3. Buenas Prácticas Generales

- **Sigue los principios SOLID**:
  - Single Responsibility (Responsabilidad Única)
  - Open/Closed (Abierto/Cerrado)
  - Liskov Substitution (Sustitución de Liskov)
  - Interface Segregation (Segregación de Interfaces)
  - Dependency Inversion (Inversión de Dependencias)
- **Manejo de recursos**:
  - Usa bloques `using` para recursos IDisposable.
  - Implementa `IDisposable` cuando sea necesario.
- **Código limpio**:
  - Métodos concisos (<20 líneas idealmente).
  - Clases enfocadas (<200 líneas).
  - Nombres descriptivos y significativos.
- **Validación**:
  - Valida parámetros al inicio de los métodos públicos.
  - Usa `ArgumentNullException`, `ArgumentException` apropiadamente.

## 4. Ejemplo

```csharp
public class Calculadora
{
    private int resultadoActual;

    public int ResultadoActual => resultadoActual;

    public void Sumar(int valor)
    {
        resultadoActual += valor;
    }
}
```
