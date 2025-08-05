# BigChungus.NET

BigChungus.NET is a work-in-progress minimal UI framework for Windows utilities.

## Features
- **NativeAOT-compatible** - the first .NET UI framework that lets you display a dialog box without requiring .NET to be installed, or bundling tens of megabytes of framework bloat with your application.
- **MVVM-first** - bind control properties to view model properties, and let the framework do the rest. No clunky control event infrastructure.
- **Reliability and functionality of Win32 dialogs** - including out-of-the-box accessibility, High-DPI support and native Windows control styles, as implemented in `user32.dll` and other native libraries that that ship with every Windows installation.
- **Fluent, low-code API** - create a `DialogBuilder<TViewModel>` and follow IntelliSense. See: [Program.cs](https://github.com/TheLeftExit/BigChungus.NET/blob/main/BigChungus.Demo/Program.cs).

## Development status

Core infrastructure/API implemented. `Button` feature-complete as a sample control.

Remaining tasks (not limited to):

- [ ] Dialog item & runtime control wrappers
    - [x] `CheckBox`
    - [ ] `CheckBoxThreeState`
    - [ ] `RadioButton` (including grouping & binding)
    - [x] `GroupBox`
    - [x] Dialog box itself
    - [x] `TextBox`
    - [ ] `Label`
    - [ ] `LineHorizontal`/`LineVertical`
    - [ ] `ProgressBar`
    - [ ] `TrackBar`
    - [ ] `ListView` (including list binding)
    - [ ] `TreeView` (including tree binding, somehow)
- [ ] Services/behaviors
    - [ ] `Dispatcher`/`SynchronizationContext`
    - [ ] Common dialogs (open/save, message box, etc.)
    - [ ] Tooltips
    - [ ] Validation (if there's a safe and viable solution)
    - [ ] Images (dialog icon, supported controls)
- [ ] IntelliSense docs
- [ ] Publish to NuGet (when viable for usage in real utilities)
     
I also plan to use this framework for my own utility projects, so the feature/task list will be driven by my own requirements.

## Getting started

Already? For now, clone the repository and use `BigChungus.Demo` as your working project.
