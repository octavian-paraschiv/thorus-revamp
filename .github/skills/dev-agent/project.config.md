# Dev Agent — Project Configuration

The agent reads this file **first** before proceeding with any action.

## Project Identity

| Setting | Value |
|---------|-------|
| **Application Name** | Thorus Revamp |
| **Framework** | .NET 10 |
| **Repository** | `thorus-revamp` |
| **Test Framework** | xUnit 2.5.3 |
| **Test SDK** | Microsoft.NET.Test.Sdk 17.8.0 |

## How to review, analyze and update the project requirements

- Read the specifications from '.github/specs.md'
- Review and ellaborate them considering functional, non-functional and performance aspects
- If necessary, propose requirement updates to human
- If human agrees, write the updated requirements back to '.github/specs.md', otherwise ask human for suggestions and repeat the cycle
- Before writing the updated requirement specs, back up the already existing ones to '.github/archive/specs_v{versionNumber}_{date}.md', where {versionNumber} is a number that starts from 1 and increments for each backed up version, and {date} is the date when the specs were backed up, in format yyyy-MM-dd
- Should be possible to revert at any time to a backed up specs version if the human asks so by using prompt 'restore spec v{versionNumber}
- Should be possible to reiterate all this process whenever human asks so by using prompt 'review specs'

## How to create, review and update the implementation plans

- Read the specifications from '.github/specs.md', making sure they're comprehensible
- If potential pitfalls are spotted, flag them to the human and propose spec review
- If human agrees, move to the spec review phase; otherwise proceed with generating the implementation plan
- When generating the implementation plan, always consider implementation done in stages, not all in a single shot, and ask human to review each stage before proceeding to the next one
- When generating the implementation plan, consider all best architectural practices, such as:
  - Start with Business Domains, Not Technologies
  - Keep the Domain Independent
  - Depend Inward (Clean Architecture)
  - Separate Read and Write Paths
  - Keep components Thin
  - Design for Change
  - Explicit Boundaries Between Components
  - Treat External Systems as Unreliable
  - Security by Design
  - Observability from Day One
  - Prefer Simplicity Over Purity
  - Enforce Architecture Automatically
- When done, write the implementation plan to '.github/implementation-plan.md' and ask human to review
- Before writing the new implementation plans, back up the already existing one to '.github/archive/implementation-plan_v{versionNumber}_{date}.md', where {versionNumber} is a number that starts from 1 and increments for each backed up version, and {date} is the date when the implementation plan was backed up, in format yyyy-MM-dd
- Should be possible to revert at any time to a backed up implementation plan if the human asks so by using prompt 'restore plan v{versionNumber}

## How to generate code based on the implementation plans

- Read the implementation plan from '.github/implementation-plan.md', making sure it is comprehensible
- If potential pitfalls are spotted, flag them to the human and propose implementation plan review
- If human agrees, move to the implementation plan review phase; otherwise proceed with generating the application
- Never generate code that is not covered by the implementation plan, unless human explicitly asks for it (update the implementation plan first in this case)
- When generating code, follow the project structure, coding conventions, and guidelines for generating code across all application layers defined in this file
- When generating unit tests, follow the test project structure, naming conventions, and coverage targets defined in this file
- Always generate code for only one implementation plan stage at a time, and ask human to review it before proceeding to the next stage
- When done with generating code for a stage, and the human approved it, write a descriptive entry in changelog.md, including the implementation plan stage number, a brief description of the changes made, and the date of completion

## Project structure, coding conventions, and guidelines for generating code across all application layers

### Project structure

```
ThorusRevamp/
├── ThorusRevamp.slnx
├── src/
|   ├── ThorusViewer/
|   ├── ThorusSimulation/
|   ├── ThorusCommon/
|   ├── GribImporter/
|   └── NetCdfImporter/
└── tests/
    ├── ThorusViewerTests/
    ├── ThorusSimulationTests/
    ├── ThorusCommonTests/
    ├── GribImporterTests/
    └── NetCdfImporterTests/
```

ThorusViewer: .NET Desktop application that allows visualizing the simulation results
ThorusSimulation: .NET Console application that contains the simulation engine - that is, the engine that takes care of "advancing" the state of the planetary model to the next state
ThorusCommon: .NET assembly that contains the planetary model used by the simulation engine as well all the applicable models for the physics processes that intervene in the simulation.
GribImporter: .NET assembly that allows importing files in GRIB format, to be used as simulation initial data. The atmospheric data will be provided in this format.
NetCdfImporter: .NET assembly that allows importing files in NetCDF format, to be used as simulation initial data. The sea surface temperature (SST) will be provided in this format.

ThorusViewerTests: contains applicable unit tests for the ThorusViewer application
ThorusSimulationTests: contains applicable unit tests for the ThorusSimulation application
ThorusCommonTests: contains applicable unit tests for the ThorusCommon assembly
GribImporterTests: contains applicable unit tests for the GribImporter assembly
NetCdfImporterTests: contains applicable unit tests for the NetCdfImporter assembly

### Coding conventions

### Guidelines for generating code


Always use the best practices for writing clean, maintainable and testable code, such as:

- Make Code Express Intent
- Make Architecture Visible in Code
- Small Methods
- One Level of Abstraction Per Method
- Avoid Deep Nesting
- Prefer Composition Over Inheritance
- Keep Classes Cohesive
- Minimize Dependencies
- Avoid Hidden Side Effects
- Make Invalid States Impossible
- Use Immutable Models Where Practical
- Log for Diagnostics, Not Curiosity
- Remove Duplication Carefully
- Refactor Continuously
- Use meaningful names for classes, methods, variables, and parameters
- Use PascalCase for class names and method names, and camelCase for variable and parameter names
- Use interfaces to define contracts for services and dependencies, and use dependency injection to inject them into classes
- Use async/await for asynchronous programming, and avoid blocking calls or synchronous code
- Use exception handling and logging to handle errors and unexpected situations, and avoid swallowing exceptions or returning null values
- Use unit tests to verify the functionality and behavior of classes and methods, and follow the Arrange-Act-Assert pattern for writing tests
- Use mocking frameworks to mock dependencies and isolate the system under test, and avoid testing external systems or databases
- Use code analysis tools to enforce coding standards and detect code smells, and avoid ignoring warnings or errors
- Use code reviews to get feedback and improve code quality, and avoid merging code without approval or testing
- Use version control to track changes and collaborate with others, and avoid committing large or unrelated changes
- Use documentation to explain the purpose and usage of classes and methods, and avoid leaving code undocumented or unclear
- Use design patterns to solve common problems and improve code structure, and avoid reinventing the wheel or using anti-patterns
- Use SOLID principles to design classes and methods that are single-responsibility, open-closed, Liskov-substitutable, interface-segregated, and dependency-inverted, and avoid violating these principles or creating tightly-coupled code
- Use refactoring to improve code readability and maintainability, and avoid leaving code messy or hard to understand


## Test project structure, naming conventions, and coverage targets for this project

### Test project structure

Mirror the folder structure of the application or assembly that gets tested. Example:

```
ThorusRevamp/
├── ThorusRevamp.slnx
├── src/
|   ├── ThorusViewer/
|   ├── ThorusSimulation/
|   ├── ThorusCommon/
|   |   └── FeatureName/ClassName.cs
|   ├── GribImporter/
|   └── NetCdfImporter/
└── tests/
    ├── ThorusViewerTests/
    ├── ThorusSimulationTests/
    ├── ThorusCommonTests/
    |   └── FeatureNameTests/ClassNameTests.cs
    ├── GribImporterTests/
    └── NetCdfImporterTests/
```

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| **Test file** | `{ClassName}Tests.cs` | `AccessTokenServiceTests.cs` |
| **Test class** | `{ClassName}Tests` | `AccessTokenServiceTests` |
| **Test method** | `MethodName_StateUnderTest_ExpectedBehavior` | `GetToken_WhenCacheIsEmpty_ShouldCallProvider` |
| **System under test variable** | `_sut` | `private readonly AccessTokenService _sut;` |
| **Mock variables** | `_mock{InterfaceName}` | `_mockAccessTokenProvider` |

## Coverage Targets

It is desirable to obtain coverage at least equal to 80% per all five test assemblies.
Only calculate code coverage for the apps and assemblies inside the src/ folder. 
Exclude from code coverage calculations everything that is inside the tests/ folder.

## Approach

### When creating unit tests:

1. **Read the project config** — determine which test project maps to which source project, and the naming/structure conventions
2. **Read the source code** — find the class to test, its constructor dependencies, public methods, and edge cases
3. **Identify dependencies** — list all interfaces/abstractions injected via constructor; these will be mocked
4. **Design test cases** — for each public method, create tests covering:
   - **Happy path** — valid inputs producing expected outputs
   - **Edge cases** — null inputs, empty collections, boundary values
   - **Error paths** — exceptions thrown for invalid state or failed dependencies
   - **Validation** — if FluentValidation validators exist, test valid and invalid inputs
5. **Write the test class** — follow the naming convention `{ClassName}Tests.cs`, using the Arrange-Act-Assert pattern
6. **Mirror the folder structure** — place the test file in a path that mirrors the source project structure

### When updating existing tests:

1. Read the existing test file to understand current coverage
2. Identify which new methods, parameters, or behaviors need test coverage
3. Add new test methods without modifying or removing existing passing tests
4. If a source method signature changed, update the affected test methods accordingly

### Guidelines for generating unit test code

- Avoid Mocking Everything
- Write Tests Against Behavior
- Follow the Test Pyramid
- Test Business Rules Thoroughly
- Use Mocks for External Dependencies
- Use Descriptive Test Names
- Keep Tests Independent
- Use Setup and Teardown Wisely
