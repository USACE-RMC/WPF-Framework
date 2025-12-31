# Getting Started

This guide walks you through creating your first application using the ProjectControls framework.

## Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8.1
- Basic knowledge of WPF and C#

## Step 1: Create a New WPF Project

1. Open Visual Studio
2. Create a new **WPF App (.NET Framework)** project
3. Target **.NET Framework 4.8.1**
4. Name your project (e.g., `MyProjectApp`)

## Step 2: Add Project References

Add references to the ProjectControls libraries:

1. Right-click your project > **Add** > **Reference**
2. Browse to the ProjectControls build output and add:
   - `FrameworkInterfaces.dll`
   - `FrameworkUI.dll`
   - `Themes.dll`
   - `GenericControls.dll`
3. Also add the NuGet package:
   - `Xceed.Wpf.AvalonDock` (v3.5.3)

Or add project references if you have the source:

```xml
<ProjectReference Include="..\FrameworkInterfaces\FrameworkInterfaces.csproj" />
<ProjectReference Include="..\FrameworkUI\FrameworkUI.csproj" />
<ProjectReference Include="..\Themes\Themes.csproj" />
```

## Step 3: Create Your Project Class

Create a class that implements `IProject`:

```csharp
using FrameworkInterfaces;
using System.Collections.ObjectModel;

namespace MyProjectApp
{
    public class MyProject : IProject
    {
        public MyProject()
        {
            Name = "New Project";
            Elements = new MyElementCollection(this);
        }

        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool IsDirty { get; set; }

        public MyElementCollection Elements { get; }

        // IProject requires a collection of IElementCollection
        public ReadOnlyCollection<IElementCollection> ElementCollections =>
            new ReadOnlyCollection<IElementCollection>(
                new IElementCollection[] { Elements });
    }
}
```

## Step 4: Create Your Element Classes

Create element classes that extend `ElementBase`:

```csharp
using FrameworkInterfaces;
using System;

namespace MyProjectApp
{
    public class MyElement : ElementBase
    {
        private string _customProperty;

        public MyElement(string name, IElementCollection parentCollection)
            : base(name, parentCollection)
        {
            _creationDate = DateTime.Now;
            _lastModified = DateTime.Now;
        }

        public override string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(Name));
                    IsDirty = true;
                }
            }
        }

        public override string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(nameof(Description));
                    IsDirty = true;
                }
            }
        }

        public string CustomProperty
        {
            get => _customProperty;
            set
            {
                if (_customProperty != value)
                {
                    _customProperty = value;
                    NotifyPropertyChanged(nameof(CustomProperty));
                    IsDirty = true;
                }
            }
        }

        public override DateTime CreationDate => _creationDate;
        public override DateTime LastModified => _lastModified;

        // Implement other required abstract members...
    }
}
```

## Step 5: Create Your Element Collection

```csharp
using FrameworkInterfaces;

namespace MyProjectApp
{
    public class MyElementCollection : ElementCollectionBase<MyElement>
    {
        public MyElementCollection(IProject project) : base(project)
        {
            Name = "Elements";
        }

        public override string Name { get; set; }
    }
}
```

## Step 6: Create Your Project Controller

The controller connects your project model to the UI:

```csharp
using FrameworkInterfaces;
using FrameworkUI;
using FrameworkUI.ProjectExplorer;
using System.Windows;
using System.Windows.Controls;

namespace MyProjectApp
{
    public class MyProjectController : FrameworkUIController
    {
        public MyProjectController(IProject project) : base(project)
        {
        }

        public override bool CanMultiSelect => true;

        protected override void DefineProjectMenuItems()
        {
            // Add custom Project menu items here
        }

        protected override void DefineToolsMenuItems()
        {
            // Add custom Tools menu items here
        }

        protected override void DefineHelpMenuItems()
        {
            // Add custom Help menu items here
        }

        protected override void DefineProjectExplorerMenuItems()
        {
            // Add custom context menu items here
        }

        public override Control GetDocumentControl(IElement element)
        {
            // Return the editor control for an element
            return new MyElementEditor();
        }

        public override Control GetPropertiesControl(IElement element)
        {
            // Return the properties panel for an element
            return new MyElementProperties();
        }

        public override Control GetPropertiesControl(UIElement documentControl)
        {
            return new MyElementProperties();
        }

        public override void DocumentClosed(UIElement documentControl)
        {
            // Handle document close
        }

        public override void PropertiesClosed(UIElement documentControl)
        {
            // Handle properties panel close
        }

        public override IElement GetControlElement(UIElement control)
        {
            // Return the element associated with a control
            if (control is MyElementEditor editor)
                return editor.Element;
            return null;
        }
    }
}
```

## Step 7: Set Up App.xaml

Update your `App.xaml` to include necessary resources:

```xml
<Application x:Class="MyProjectApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Startup="Application_Startup">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- Icon resources from FrameworkUI -->
                <ResourceDictionary Source="pack://application:,,,/FrameworkUI;component/Icons/IconDictionary.xaml"/>

                <!-- Theme resources are loaded by ThemeService.Initialize() -->
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## Step 8: Initialize the Application

In `App.xaml.cs`:

```csharp
using FrameworkUI;
using System.Windows;

namespace MyProjectApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize theme system FIRST (before creating any UI)
            ThemeManager.SetTheme(ThemeColor.Light);

            // Configure application settings
            ShellPublicVariables.SoftwareVersionDate = "December 2025";
            ShellPublicVariables.SoftwareExtension = ".myproj";

            // Create the project model
            var project = new MyProject();

            // Create the controller
            var controller = new MyProjectController(project);

            // Create and show the main window
            var mainWindow = new MainWindow();
            mainWindow.ProjectNode = controller;
            mainWindow.Show();
        }
    }
}
```

## Step 9: Run Your Application

Press F5 to run. You should see:

- The main window with menu bar and toolbar
- Project Explorer on the left
- Message Window at the bottom
- Document area in the center

## Next Steps

- [Architecture Overview](architecture.md) - Understand the system design
- [Themes Guide](themes.md) - Customize application appearance
- [Undo/Redo Guide](undo-redo.md) - Add undo support to your elements
- See `Demo_ProjectUI` for a complete working example

## Common Issues

### Theme not applying

Make sure you call `ThemeManager.SetTheme()` **before** creating any UI elements:

```csharp
// CORRECT - theme initialized first
ThemeManager.SetTheme(ThemeColor.Light);
var mainWindow = new MainWindow();

// WRONG - window created before theme
var mainWindow = new MainWindow();
ThemeManager.SetTheme(ThemeColor.Light);
```

### Missing resources

Ensure all required resource dictionaries are merged in `App.xaml` and that theme initialization happens in `Application_Startup`.

### Undo/Redo not working

Verify that your element implements `IUndoableElement` and uses `SetPropertyWithUndo` for property changes. See the [Undo/Redo Guide](undo-redo.md) for details.
