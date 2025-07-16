# GitHub Copilot Instructions

## General Guidelines

- Follow the existing code style and conventions in the repository
- Ensure all files end with a newline character
- Remove trailing whitespace from all lines
- Use consistent indentation as defined in .editorconfig

## C# Coding Standards

- Follow the naming conventions defined in .editorconfig
- Use PascalCase for public members and types
- Use camelCase with underscore prefix for private fields
- Prefer explicit types over var when type is not obvious

## Testing Guidelines

- **One test, one assert**: Each test method should verify a single behavior with a single assertion
- **Use parameterized tests**: When testing similar scenarios with different inputs, use `[TestCaseSource]` to eliminate code duplication
- **Test naming**: Use descriptive test method names that clearly indicate what is being tested and the expected outcome
- **Avoid code duplication**: If you find yourself copying test code, refactor to use parameterized tests instead

## Code Duplication Prevention

- Before creating similar classes or methods, consider if they can be generalized or parameterized
- Use inheritance or composition patterns when appropriate
- Extract common functionality into shared base classes or helper methods
- For test data, use test case sources rather than duplicating test methods

## EditorConfig Compliance

- Respect all settings defined in .editorconfig
- Let Git automatically manage line endings unless there are specific tool compatibility issues
- Follow indentation rules (spaces vs tabs as configured per file type)
- Maintain consistent formatting across all file types

## Code Duplication Detection (JSCPD)

- **Architectural patterns**: Similar structure across gates/blocks is expected and acceptable
- **Configuration**: JSCPD is configured with higher thresholds (10+ lines, 100+ tokens) to focus on true duplication issues
- **Focus areas**: Look for duplicated business logic, test patterns, or utility functions rather than architectural similarities
- **Regular review**: When JSCPD flags issues, evaluate if they represent true problems or acceptable patterns
