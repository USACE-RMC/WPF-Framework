# ExpressionParserControls

## Purpose
WPF controls for editing and evaluating mathematical/logical expressions, providing a syntax-highlighted expression editor, calculator button panel, and function browser.

## Key Files
- `ExpressionControl.xaml.cs` - RichTextBox-based expression editor with real-time syntax highlighting, token parsing, color-coded parentheses, and function hyperlinks
- `CalculatorControl.xaml.cs` - Calculator-style button panel for arithmetic operations; integrates with ExpressionControl for expression building and parse tree generation
- `AvailableFunctions.xaml.cs` - Browsable list of parser functions with insert-into-expression and help navigation support

## Dependencies
- **ExpressionParser** (project reference) - Core tokenizer, parser, and function registry
- **System.Drawing.Common** (NuGet) - Color interop

## Patterns
- ExpressionControl exposes a `Text` dependency property for two-way binding to expression strings
- AvailableFunctions binds to an ExpressionControl via `ExpressionText` dependency property for function insertion
- CalculatorControl raises `ExpressionChanged` event when the expression is modified
- Token types from ExpressionParser drive syntax highlighting colors in ExpressionControl
- Namespace is `ExpressionParserControls`

## Gotchas
- ExpressionControl uses WPF RichTextBox internally -- setting Text triggers reformatting which re-parses tokens; guard against re-entrancy via `_formattingExpression` flag
- Parenthesis colors cycle through a fixed 7-color palette; deeply nested expressions repeat colors
- This project does NOT reference GenericControls or Themes -- it is intentionally lightweight with only ExpressionParser as a dependency
- Uses `Microsoft.NET.Sdk.WindowsDesktop` SDK (unlike other projects using `Microsoft.NET.Sdk`)
