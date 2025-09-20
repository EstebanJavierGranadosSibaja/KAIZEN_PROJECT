dotnet tool install -g dotnet-format

dotnet format KaizenLang.sln --verify-no-changes

dotnet format KaizenLang.sln

Note: before pushing or opening a PR, run `dotnet format KaizenLang.sln` locally to apply formatting changes. The CI job in `.github/workflows/format.yml` runs `dotnet format --verify-no-changes` and will fail if your branch contains unformatted files.

Also: the project uses WinForms UI APIs which emit CA1416 platform-compatibility warnings on non-Windows platforms; these are expected for this repository and are suppressed in the project file to keep CI logs focused on actionable issues.
