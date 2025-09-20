dotnet tool install -g dotnet-format

dotnet format KaizenLang.sln --verify-no-changes

dotnet format KaizenLang.sln

Nota: Antes de enviar o abrir una solicitud de integración (PR), ejecute `dotnet format KaizenLang.sln` localmente para aplicar los cambios de formato. El trabajo de integración continua (CI) en `.github/workflows/format.yml` ejecuta `dotnet format --verify-no-changes` y fallará si su rama contiene archivos sin formato.

Además: el proyecto utiliza las API de interfaz de usuario de WinForms que emiten advertencias de compatibilidad de plataforma CA1416 en plataformas distintas de Windows. Estas advertencias son esperadas para este repositorio y se suprimen en el archivo del proyecto para que los registros de integración continua se centren en problemas que requieren acción.
