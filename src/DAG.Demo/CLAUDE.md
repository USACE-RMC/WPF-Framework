# DAG.Demo

## Purpose
Demo application showcasing the DAG and DAGControls libraries with an interactive graph editor.

## Key Files
- `MainWindow.xaml.cs` - Main window hosting a `FlowGraphCanvas` with a sample `TestGraph`
- `TestGraph.cs` - Sample `Graph` subclass for demonstration
- `TestNode.cs` - Sample `NodeBase` subclass for demonstration
- `Resources/SampleGraph.xml` - Pre-built sample graph for loading

## How to Run
```
dotnet run --project src/DAG.Demo/DAG.Demo.csproj
```

## Dependencies
- **DAG** (project reference) - Graph model
- **DAGControls** (project reference) - FlowGraphCanvas UI control
