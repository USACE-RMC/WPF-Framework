# ExpressionParser.Tests

Unit tests for the ExpressionParser library.

## Framework

MSTest 3.5.2 (not xUnit).

## Key Test Areas

- `LexerTests.cs` - Tests tokenization of expressions into token lists
- `ParserTests.cs` - Tests parsing and evaluation of complete expressions
- `ArithmeticTests.cs` - Tests arithmetic operators (+, -, *, /, ^) and operator precedence
- `ComparisonAndLogicalTests.cs` - Tests comparison operators (=, !=, <, >, <=, >=) and logical functions (AND, OR, IF)
- `StringFunctionTests.cs` - Tests string functions (LEFT, RIGHT, LEN, SUBSTRING, CONTAINS, CONCATENATE, &)
- `TypeConverterTests.cs` - Tests type conversion functions (DBL, INT, STR, BOOL and aliases)
- `SimplifyAndVariableTests.cs` - Tests expression simplification and variable handling
- `ErrorHandlingTests.cs` - Tests parse error detection (missing parentheses, invalid syntax)

## How to Run

```
dotnet test src/ExpressionParser.Tests/ExpressionParser.Tests.csproj
```
