# Innovator Installer
The initial focus of the tool is on installing and exporting packages.  While 
tools already exist for package installs and exports, this tool attempts 
to improve the experience in the following key areas:

## Wizard Interface

The new tool features a wizard interface to make it easier for 
non-developers to be able to quickly install packages.

![Wizard interface](screenshot-pg01.png)

## Connection Manager

A connection manager saves all your commonly used connections locally so
you don't have to repetitively enter URLs and credentials when running
multiple exports or installs

![Connection manager](screenshot-pg02.png)

## Package Anything

No longer must all the items be in a pre-declared package in the database.
As you are creating your package, you can add items to it via a simple 
keyed_name search or through an advanced AML search. Previously saved
files can also be used as the basis for creating a package. This allows 
you to create packages which represent a unit of work as opposed to having
to create "canonical" packages and exporting pieces from multiple 
packages.

![Package creation](screenshot-pg03.png)

## Dependency Analysis

The tool will analyze the exported AML to find all dependencies. It will 
then prompt you for each dependency allowing you to alternatively include
that dependency in the package, remove the specific property reference,
or remove the entire Item with the reference.  This helps to ensure that 
you don't forget to include important items in the package.  In addition,
the tool will sort your package items in order of their dependencies to
help guarantee that they will import without any errors.

![Dependency analysis](screenshot-pg04.png)

## Integration with Install

No longer are the export and install two entirely separate tools.  After
creating your package, you can save off the export files, immediately 
install the package into a target database, or both.

![Export-install integration](screenshot-pg05.png)

## New Package Format

- The new tool defaults to a single-file package format with the extension 
  *.innpkg. This format is a zip file containing the relevant AML script 
  files and a manifest XML file defining the install order based on 
  dependencies.
- This file format attempts to generate files useful to developers where 
  possible.  For example, reports are exported as XSLT files with sidecar 
  XML data files allowing for easier offline development.
- The tool can also generate an unzipped version of the *.innpkg as well 
  as the previous import/export file directory structure (although the 
  latter is not fully tested)

## Install in Multiple Databases at Once

If you want to quickly install the same changes in multiple databases,
simply check off all the databases you want to install the package in.
The installs will run back-to-back for each database instance.

![Install multiple](screenshot-pg06.png)

## Clear Progress Indicator and Warning Dialog

A clear progress indicator tells you of the status of your install.  An
updated error dialog box will also give you a clear indication of any 
errors which may occur.  This dialog allows you to see the AML query
which caused the error and the SOAP error response.  It also gives you 
the option to either ignore the error, retry the query (e.g. after fixing
something in the database), or abort the entire install

![Install progress](screenshot-pg07.png)