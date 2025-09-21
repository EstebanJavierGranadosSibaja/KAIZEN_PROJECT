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