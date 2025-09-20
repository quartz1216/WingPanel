# WingPanel

WingPanel is a WinUI 3 packaged shell companion that surfaces quick access launchers, task switching, volume controls and notifications in a single, keyboard accessible panel. The solution is organised into three projects:

- **WingPanel.App** – the WinUI 3 packaged front-end that renders the interactive panel and hosts region controls.
- **WingPanel.Core** – cross-platform services, settings models, P/Invoke wrappers and logging infrastructure shared by the app and tests.
- **WingPanel.Tests** – xUnit based unit tests targeting the core library.

## Prerequisites

To build and debug the app you need a Windows 11 machine with the following tools installed:

1. [.NET SDK 8.0](https://dotnet.microsoft.com/download).
2. Visual Studio 2022 17.8 or later with the **Universal Windows Platform development** and **.NET desktop development** workloads.
3. The WinUI 3 workload for the .NET SDK:
   ```bash
   dotnet workload install winui
   ```
4. Windows App SDK runtime dependencies (installed automatically with the Visual Studio workload).

> **Note:** The WinUI 3 packaging project targets `net8.0-windows10.0.19041.0`, so building on non-Windows hosts is not supported.

## Building the solution

### From Visual Studio

1. Open `WingPanel.sln`.
2. When prompted, restore NuGet packages.
3. Set **WingPanel.App** as the startup project.
4. Choose the desired architecture (x64/x86/arm64) and configuration (Debug/Release).
5. Build with `Build > Build Solution`.

### From the command line

```bash
# Restore dependencies
msbuild WingPanel.sln -t:Restore

# Build the packaged app (Debug configuration)
msbuild WingPanel.sln -p:Configuration=Debug

# Run unit tests
dotnet test src/WingPanel.Tests/WingPanel.Tests.csproj
```

Because the packaged project produces an MSIX, the build output is generated under `src/WingPanel.App/bin/<Configuration>/<Architecture>/`. The MSIX bundle can be installed by double-clicking the generated package or via `Add-AppxPackage` in PowerShell.

## Packaging and deployment

To produce a signed MSIX package ready for sideloading:

1. Ensure you have a valid code signing certificate (.pfx). For local testing you can create one with the Windows SDK `makecert`/`New-SelfSignedCertificate` tools.
2. In Visual Studio right-click **WingPanel.App** and choose **Publish > Create App Packages...**. Follow the wizard to select sideloading, certificate and output location.
3. Alternatively, from the command line run:
   ```bash
   msbuild src/WingPanel.App/WingPanel.App.csproj -p:Configuration=Release -p:GenerateAppInstallerFile=false -p:UapAppxPackageBuildMode=StoreUpload
   ```
4. Install the produced MSIX by double-clicking it or using:
   ```powershell
   Add-AppxPackage .\WingPanel.App_1.0.0.0_x64.msixbundle
   ```

## Debugging

- **Visual Studio:** set breakpoints in the WinUI project, press `F5` to launch the packaged app. The `PanelWindow` will appear docked near the top of the primary monitor. Keyboard navigation (arrow keys) and drag-and-drop can be inspected live.
- **Logging:** runtime logs are written to `%LocalAppData%\WingPanel\Logs`. Each day creates a new `wingpanel-YYYYMMDD.log` file. This is useful when debugging hotkey registration or settings persistence.
- **Settings:** user settings are stored at `%LocalAppData%\WingPanel\Settings.json`. Delete the file to reset defaults or edit it while the app is closed to tweak configuration.
- **Unit tests:** run `dotnet test src/WingPanel.Tests/WingPanel.Tests.csproj` to verify settings migration, hotkey fallback logic and MRU tracking behaviour.

## Project structure

```
WingPanel.sln
├── src
│   ├── WingPanel.App          # WinUI 3 packaged application
│   ├── WingPanel.Core         # Shared services, models and logging
│   └── WingPanel.Tests        # xUnit test project targeting the core library
└── README.md
```

## Contributing

Issues and pull requests are welcome. Please ensure new code paths include appropriate unit tests and keep the solution building on supported Windows configurations.
