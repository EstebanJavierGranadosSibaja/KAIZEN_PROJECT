# DEV quickstart

Pequeñas instrucciones para desarrollar y testear KaizenLang en Windows (PowerShell).

1) Build principal:

```powershell
dotnet build KaizenLang.sln
```

2) Ejecutar el tester (muestra tokens/AST/semántica):

```powershell
dotnet run --project KaizenLang\tools\CompilationTester\CompilationTester.csproj
```

3) Ejecutar tests automatizados:

```powershell
dotnet test KaizenLang\tests\KaizenLang.Tests\KaizenLang.Tests.csproj
```

4) Si editas el proyecto principal, el `post-build` copia automáticamente `KaizenLang.dll` al bin del tester para que el tester pueda ejecutarse sin problemas.


Si quieres que agregue un script PowerShell `build-and-test.ps1` o un `tasks.json` para VS Code lo hago en el siguiente paso.