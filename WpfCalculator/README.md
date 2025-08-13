# WPF Calculator (C#)

A simple Windows desktop calculator built with WPF and .NET 8.

## Requirements
- Windows 10/11
- .NET 8 SDK (or Visual Studio 2022 17.8+ with the ".NET Desktop Development" workload)

Note: WPF apps can only be built and run on Windows.

## Run (Command Line)
```bash
# From this repository root on Windows
dotnet run --project WpfCalculator/WpfCalculator.csproj
```

## Open in Visual Studio
- Open `WpfCalculator/WpfCalculator.csproj` (or create a solution and add the project)
- Set build configuration to `Debug` and press F5

## Features
- Digits 0-9, decimal
- Basic operators: +, −, ×, ÷
- Clear, backspace, sign toggle
- Culture-aware decimal separator and compact result formatting