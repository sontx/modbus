# Core.Tests

Unit tests for the Modbus Core library.

## Running Tests

To run all tests:
```bash
dotnet test
```

To run tests with detailed output:
```bash
dotnet test --verbosity normal
```

To run tests and generate coverage:
```bash
dotnet test /p:CollectCoverage=true
```

## Test Coverage

This test suite provides comprehensive coverage of the Modbus Core library:

- **ChecksumTests**: CRC16 computation validation
- **BytesNumericTests**: Big-endian byte conversion utilities
- **BytesStructureTests**: Structure serialization with endianness support
- **RtuRequestTests**: RTU request builder and CRC validation
- **TcpRequestTests**: TCP request builder with MBAP header
- **ConstantsTests**: Constant value verification

## Requirements

- .NET SDK 8.0 or later
- xUnit test framework (automatically restored via NuGet)

## Test Statistics

Total Tests: 59
- ChecksumTests: 5 tests
- BytesNumericTests: 24 tests
- BytesStructureTests: 7 tests
- RtuRequestTests: 10 tests
- TcpRequestTests: 9 tests
- ConstantsTests: 4 tests

All tests validate both normal operation and edge cases to ensure the Modbus core library functions correctly and stably.
