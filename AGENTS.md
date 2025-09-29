# Repository Guidelines

## Project Structure & Module Organization

Work inside the solution `Svan.Monads.sln`, which loads the core library under `src/`. Each monad lives in its own directory (`src/Option`, `src/Result`, `src/Try`) with matching extension helpers. Usage samples reside in `examples/`, while automated checks target the test project in `tests/Svan.Monads.UnitTests.csproj`. Keep new runtime code in `src/` and mirror its surface with focused tests in `tests/`.

## Build, Test, and Development Commands

Restore dependencies with `dotnet restore Svan.Monads.sln`. Build the library using `dotnet build Svan.Monads.sln -c Release` before publishing. Run the full test suite via `dotnet test Svan.Monads.sln`, or target a single class with `dotnet test --filter FullyQualifiedName~OptionTests`. Use `dotnet pack src/Svan.Monads.csproj -c Release` when preparing a NuGet package.

## Coding Style & Naming Conventions

Follow standard C# conventions: four-space indentation, braces on their own lines, PascalCase for public types and members, and camelCase locals. Favor expression-bodied members only when they enhance clarity. Keep monad APIs fluent and immutable; leverage existing extension methods rather than duplicating logic. Align formatting with the solution defaults (`dotnet format` is acceptable when available) and avoid introducing compiler warnings.

## Testing Guidelines

All behavior changes must land with xUnit tests in `tests/`. Name fixture files `<Type>Tests.cs` and test methods in the form `Scenario_underTest_expectedOutcome`. Cover both success and error branches for Option, Result, and Try workflows. Execute `dotnet test` locally before opening a pull request and ensure sample code in `examples/` still compiles when relevant.

## Commit & Pull Request Guidelines

Write commits in present-tense imperative voice (e.g., `Add Result.Filter overload`). Group related changes together; use `[skip ci]` only for documentation-only commits, mirroring existing history. Pull requests should describe the change, reference any issues, and note testing done (`dotnet test`). Include screenshots or sample snippets if they help reviewers understand API additions or behavioral shifts.
