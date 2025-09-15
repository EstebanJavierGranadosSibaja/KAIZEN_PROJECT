<!-- markdownlint-disable MD033 -->

# 📚 DOCUMENTACIÓN KAIZENLANG

> **Compilador e Intérprete Educativo - Paradigmas de Programación**  
> Universidad Nacional - Sede Regional Brunca  
> Prof. MSc. Josías Ariel Chaves Murillo

---

## 🎯 **Acerca del Proyecto**

**KaizenLang** es un lenguaje de programación educativo desarrollado como proyecto académico. Implementa un compilador completo con análisis léxico, sintáctico y semántico, junto con un intérprete y una interfaz gráfica moderna.

### 🌟 **Filosofía del Nombre**

**"Kaizen"** (改善) es una palabra japonesa que significa "mejora continua". Refleja nuestro enfoque de aprendizaje iterativo en el desarrollo de compiladores.

---

## 📖 **Índice de Documentación**

### 🏗️ **Arquitectura y Estructura**

- **[📁 Estructura de Directorios](estructura-directorios.md)**  
  Árbol completo de carpetas y archivos del proyecto

- **[💻 Arquitectura del Código](arquitectura-codigo.md)**  
  Explicación detallada de cada módulo y componente

- **[🔄 Flujo de Compilación](flujo-compilacion.md)**  
  Proceso paso a paso desde código fuente hasta ejecución

### 🖥️ **Interfaz y Usuario**

- **[🎨 Interfaz de Usuario](interfaz-usuario.md)**  
  Descripción completa de la UI y sus componentes

- **[⚙️ Configuración Técnica](configuracion-tecnica.md)**  
  Especificaciones, dependencias y configuración del proyecto

### 📋 **Aspectos Académicos**

- **[🎓 Cumplimiento Académico](cumplimiento-academico.md)**  
  Requerimientos del curso y cómo se implementaron

- **[📄 Especificaciones del Curso](descripción-proyecto.md)**  
  Documento original con los requerimientos del profesor

---

## 🚀 **Inicio Rápido**

### **Prerrequisitos**

- .NET 9.0 SDK
- Windows 10/11
- Visual Studio Code (recomendado)

### **Comandos Básicos**

```bash
# Clonar el proyecto
git clone [repository-url]
cd KaizenLang

# Compilar
dotnet build

# Ejecutar
dotnet run
```

### **Primer Uso**

1. Ejecuta la aplicación
2. Usa el menú "Estructuras del Lenguaje" para ver ejemplos
3. Escribe código en el editor principal
4. Presiona "🛠 Compilar" para validar
5. Presiona "▶ Ejecutar" para interpretar

---

## 📊 **Estadísticas del Proyecto**

| Métrica | Valor |
|---------|-------|
| **Archivos de código** | 16 archivos .cs |
| **Líneas de código** | ~2,000 líneas |
| **Fases de compilación** | 3 (Léxico, Sintáctico, Semántico) |
| **Tipos de datos** | 7 (5 simples + 2 compuestos) |
| **Palabras reservadas** | 12 |
| **Operadores** | 20+ |

---

## 🎯 **Características Principales**

### ✨ **Compilador Completo**

- ✅ Análisis léxico con tokenización
- ✅ Análisis sintáctico con construcción de AST
- ✅ Análisis semántico con validación de tipos y scope
- ✅ Manejo robusto de errores en todas las fases

### 🎨 **Interfaz Moderna**

- ✅ Editor de código integrado
- ✅ Menús automáticos con ejemplos
- ✅ Output formateado con colores y emojis
- ✅ Botones de compilación y ejecución

### 🔧 **Lenguaje Rico**

- ✅ Tipado estricto similar a C++
- ✅ Estructuras de control (if, for, while)
- ✅ Funciones con parámetros y retorno
- ✅ Operaciones aritméticas y lógicas

---

## 📝 **Ejemplo de Código KaizenLang**

```kaizen
// Declaración de variables
int numero = 42;
string mensaje = "Hola KaizenLang";
boolean activo = true;

// Función con retorno
int suma(int a, int b) {
    return a + b;
}

// Estructura de control
if (numero > 0) {
    output("Número positivo");
} else {
    output("Número no positivo");
}

// Bucle
for (int i = 0; i < 5; i++) {
    output(i);
}
```

---

## 🔗 **Enlaces Útiles**

- **[Configuración VS Code](vs-config.md)** - Setup del entorno de desarrollo
- **[Ejemplos de Código](../Examples/example.txt)** - Casos de prueba del lenguaje
- **[Archivo Principal del Proyecto](../README.md)** - Documentación general

---

## 👨‍💻 **Información de Desarrollo**

**Desarrollador**: [Tu Nombre]  
**Curso**: Paradigmas de Programación  
**Institución**: Universidad Nacional - Sede Regional Brunca  
**Profesor**: MSc. Josías Ariel Chaves Murillo  
**Tecnologías**: .NET 9.0, C#, Windows Forms  

---

## 📞 **Soporte**

Si tienes preguntas sobre el proyecto:

- 📧 Email: [tu-email@universidad.ac.cr]
- 👨‍🏫 Profesor: MSc. Josías Ariel Chaves Murillo

---

<p align="center">
  <strong>🌟 KaizenLang - Mejora Continua en el Aprendizaje de Compiladores 🌟</strong>
</p>
