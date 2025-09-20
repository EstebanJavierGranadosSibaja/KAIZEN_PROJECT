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