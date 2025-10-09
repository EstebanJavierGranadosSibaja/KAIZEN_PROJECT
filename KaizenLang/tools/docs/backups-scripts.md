# Backup helper scripts

These were local helper scripts previously under `tools/backups/`.

build-and-test.ps1

```powershell
# Build, copy and run tests for KaizenLang (Windows PowerShell)

param(
    [string]$Configuration = "Debug"
)

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host "Building solution..."
dotnet build "$root\KaizenLang.sln" -c $Configuration

Write-Host "Running tests..."
dotnet test "$root\tests\KaizenLang.Tests\KaizenLang.Tests.csproj" -c $Configuration

Write-Host "Running CompilationTester..."
dotnet run --project "$root\tools\CompilationTester\CompilationTester.csproj" --no-build -c $Configuration
```

run-single-test.ps1

```powershell
param(
    [string]$testName
)
if (-not $testName) { Write-Host "Usage: .\run-single-test.ps1 <FullyQualifiedTestName>"; exit 1 }

$project = "tests\KaizenLang.Tests\KaizenLang.Tests.csproj"
$assembly = "tests\KaizenLang.Tests\bin\Debug\net9.0-windows7.0\KaizenLang.Tests.dll"
Write-Host "Running single test: $testName"

if (Test-Path $assembly) {
    Write-Host "Found compiled assembly. Running vstest for speed..."
    dotnet vstest $assembly --Tests:$testName
} else {
    Write-Host "Assembly not found, falling back to dotnet test (will build)..."
    dotnet test $project --filter "FullyQualifiedName~$testName"
}
```

If you prefer the original scripts restored, copy these into `tools/backups` or let me move them elsewhere.
