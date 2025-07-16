# Copilot Instructions

## Code Formatting Guidelines

This repository uses an `.editorconfig` file to define coding standards. When making code changes, ensure compliance with the following formatting rules:

### General Rules
- **Trailing whitespace**: Remove all trailing whitespace from lines
- **Final newline**: Always insert a final newline at the end of files
- **Charset**: Use UTF-8 encoding
- **Line endings**: Use auto line endings

### C# Specific Rules
- **Indentation**: Use 4 spaces (no tabs) for indentation
- **Braces**: Place opening braces on new lines
- **Spacing**: Follow standard C# spacing conventions
- **Naming**: Use PascalCase for public members, camelCase with underscore prefix for private fields

### Before Committing
Always ensure your code passes the linting checks:
- Remove trailing whitespace from all lines
- Ensure files end with a single newline
- Verify proper indentation (4 spaces for C#)
- Check that formatting matches the `.editorconfig` specifications

### Linting Tools
This repository uses Super-Linter for automated code quality checks. Make sure your changes pass all linting rules before submitting.