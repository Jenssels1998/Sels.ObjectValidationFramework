# Sels.ObjectValidationFramework
As the name suggest this is a framework for easiliy implementing validation for poco classes.

It provides an AutoMapper like syntax in the form of validation profiles.

I made it mostly because I don't like creating validation services for each poco class.
Validation services for large objects also become hard to read quickly, the AutoMapper syntax should help with this.

## Examples of usage
The performance/testool both provide some easy to understand examples on how to use the validation profiles.

## Dependencies
- Sels.Core: https://github.com/Jenssels1998/Sels.Core

## Todo
- Write documentation
- Create unit tests
- Create nuget package

## Developer Notes
Projects in the solution folder 'Shared' are shared projects imported from another solution to make building easier.
### Sels.Core
Code can be cloned from here: https://github.com/Jenssels1998/Sels.Core

Folder structure should be
-> SomeFolder
--> Sels.Core
--> Sels.ObjectValidationFramework (this project)


