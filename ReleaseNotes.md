# Boilerplate Code Generator Release Notes

### [Download from Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Strongbytes.boilerplate-code-generator)

## Release 1.11
* Add "Class Properties" Panel, that can be extended to change different properties for an already defined model on the fly (eg. which Base Class to inherit) -- for now, this panel is used for "Response Domain Entity" only.

## Release 1.10
* Normalize white space for class members;

## Release 1.9
* Keep existing code formatting when merging generated classes with existing ones;

## Release 1.8
* Enable code merge between existing CQRS classes and generated CQRS classes;

## Release 1.7
* Register UnitOfWork and Repository to Dependency Container (Autofac is used as the IoC Container);

## Release 1.6
* Generate Unit of Work and Repository;
* Add a new DbSet for the selected entity to the existing application DbContext (if not already present);
* When Unit of Work is not used, all CQRS operations use the existing application DbContext;
* The user now has the option to not generate AutoMapper configuration mappings;

## Release 1.5
* Generate hierarchy properties from inner classes referenced by main entity;

## Release 1.4
* Generate GetPaginated Query;
* Generate specific Operations, Commands and Queries, based on selected options;

## Release 1.0
* Generate Queries: GetAll, GetById;
* Generate Commands: Create, Update, Delete;
* Generate Controller containing all operations;
* Merge AutoMapper configuration mappings to existing bindings (if present) in the Target Module
* Export generated files to Solution Projects.
