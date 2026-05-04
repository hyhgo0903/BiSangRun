# Repository Guidelines

## Project Structure & Module Organization

This repository contains a .NET 8 Windows Forms application named `BiSangRun`.

- `BiSangRun.sln` and `BiSangRun.csproj` define the solution and single WinForms project.
- `Program.cs` is the application entry point.
- `BiSangRun.cs`, `BiSangRun.Designer.cs`, and `BiSangRun.resx` contain the main form logic, generated designer code, and form resources.
- `GameData/` stores app data definitions such as image metadata.
- `Utility/` contains small shared helpers and constants.
- `Resources/` stores PNG assets copied to the build output; keep filenames stable because code or project metadata may reference them.
- `Properties/` contains generated settings/resource files and project configuration.

Do not commit `bin/`, `obj/`, `.vs/`, or other generated local build output.

## Build, Test, and Development Commands

Run commands from the repository root:

```powershell
dotnet restore
dotnet build BiSangRun.sln
dotnet run --project BiSangRun.csproj
dotnet clean BiSangRun.sln
```

`dotnet restore` downloads NuGet packages, including `ImageFinder` and `System.Speech`. `dotnet build` compiles the WinForms executable. `dotnet run` launches the app locally on Windows. `dotnet clean` removes build artifacts.

## Coding Style & Naming Conventions

Follow `.editorconfig`: use 2-space indentation, CRLF line endings, and place `using` directives outside namespaces. Prefer explicit types over `var` unless existing nearby code clearly uses otherwise. Use PascalCase for classes, structs, enums, methods, properties, and events. Interfaces should start with `I`.

Keep handwritten logic out of `*.Designer.cs` files; edit the form class or designer through Visual Studio when possible. Generated files under `Properties/` should only change as a result of settings/resource updates.

## Testing Guidelines

There is currently no dedicated test project in the repository. For new automated coverage, add a separate test project such as `BiSangRun.Tests/` and name test files after the unit under test, for example `ImageGameDataTests.cs`. Prefer focused tests around non-UI logic in `GameData/` and `Utility/`.

Before opening a PR, at minimum run:

```powershell
dotnet build BiSangRun.sln
```

Also manually smoke-test UI changes with `dotnet run --project BiSangRun.csproj`.

## Commit & Pull Request Guidelines

Recent commits use short Korean summaries, for example `코드정리` or `성능향상 ...`. Keep commit messages concise and outcome-focused; mention the affected area when useful.

Pull requests should include a brief description, build/test results, and screenshots or short recordings for visible UI changes. Link related issues when available and call out any changes to image assets, settings, or generated designer files.
