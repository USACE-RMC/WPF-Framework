# ExpressionParser

Mathematical expression parsing library with Excel-like syntax. Tokenizes and parses expressions into an AST for evaluation.

## Purpose

Provides a `Lexer` and `Parser` for evaluating mathematical, logical, and string expressions. Supports variables, type conversions, and operator precedence.

## Key Files

- `Lexer/Lexer.cs` - Tokenizes expression strings into `Token` lists via `TokenizeStringToList()`. Case-insensitive keyword lookup.
- `Lexer/Token.cs` - Token record with `TokenString`, `Type`, `TokenGroup`, `OperationOrder`, and `StartPosition`.
- `Lexer/TokenType.cs` - Enum of all token types (operators, functions, literals).
- `Parser/Parser.cs` - Static `Parse()` methods accepting strings or token lists. Builds AST using operator precedence climbing.
- `Parser/Nodes/IParserNode.cs` - Interface for all AST nodes: `Evaluate()`, `Simplify()`, `ContainsVariable()`, `GetVariableNodes()`.
- `Parser/Nodes/ParseNodeResult.cs` - Evaluation result wrapper with `Result` (object) and `Type` (ResultType).
- `Parser/Nodes/ResultType.cs` - Flags enum: `Double`, `Integer`, `String`, `Boolean`, `Number` (composite), `Error`, `UnDeclared`.

## Node Categories

- **Numerics**: `IntegerNode`, `DecimalNode`, `NumericBinaryNode`, `RoundNode`, `IncrementNode`, `RandomGeneratorNode`, `RandomBetweenNode`
- **Strings**: `StringNode`, `AmpersandNode`, `ConcatenateNode`, `LeftStringNode`, `RightStringNode`, `SubStringNode`, `InStringNode`, `StringLengthNode`
- **Comparisons**: `BooleanNode`, `BooleanBinaryNode`, `AndOrNode`, `ContainsNode`, `IfNode`
- **Converters**: `ConverterNode` (DBL, INT, STR, BOOL)
- **Variables**: `VariableNode` - references resolved at evaluation time

## Supported Functions

IF, AND, OR, RIGHT, LEFT, LEN, RAND, RANDBETWEEN, ROUND, ROUNDUP/ROUNDDOWN (FLOOR/CEILING), INCREMENT, INDEXOF/INSTRING, SUBSTRING, CONTAINS, CONCATENATE, type converters (DBL/CDBL, INT/CINT, STR/CSTR, BOOL/CBOOL).

## Usage

```csharp
var node = Parser.Parse("IF([x] > 10, 'big', 'small')", ignoreCase: true,
    availableVariables: new Dictionary<string, ResultType> { ["x"] = ResultType.Double });
// Set variable values on VariableNode instances before calling Evaluate()
var result = node.Evaluate(); // returns ParseNodeResult
```

## Dependencies

- `net9.0` (no Windows dependency)
- `System.Drawing.Common` 9.0.0

## Gotchas

- Leading `=` at position 0 is silently ignored (Excel copy-paste compatibility).
- Unary `-` and `+` are handled by inserting a zero left node.
- Keywords are case-insensitive; string comparisons respect the `ignoreCase` parameter.
- `ResultType` uses bitwise flags -- use `(node.OutputType & ResultType.Number) > 0` for numeric checks.
