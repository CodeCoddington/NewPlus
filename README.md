# NewPlus

NewPlus is a highly customized, local-first alternative to Microsoft PowerToys' "New+" feature. It integrates directly into the Windows 11 File Explorer context menu to provide a seamless, native-feeling way to deploy modular folder templates and boilerplate code files.

## Features
- **Context Menu Integration:** Right-click anywhere in File Explorer to launch the borderless template picker.
- **Pre-Deployment Intercept:** Automatically prompts for a project name and renames interior boilerplate files (like `.cs`, `.py`, `.ps1`) during deployment.
- **Template Categorization:** Supports robust categorization via `templates.json` to organize templates within a native Windows cascading Context Menu.
- **Icon Synchronization:** Includes a recursive icon sweeper to forcefully apply and fix custom `.ico` files to any directory structure using Windows P/Invoke calls.
- **State Memory:** Remembers the last-used target directories to save time during repetitive tasks.

## Tech Stack
- **Framework:** .NET 10
- **Platform:** Windows Forms
- **Language:** C#
- **Architecture:** Local-first JSON configuration management with COM automation (`Shell.Application`) for native Explorer interaction.

## Deployment
This application is designed to be published as a self-contained, single-file Windows executable utilizing ReadyToRun compilation for fast execution times.
