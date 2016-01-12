# AML Studio

An integrated version of [AML Studio](http://amlstudio.codeplex.com) 
powers all of the AML inputs and can be run separately from the main 
screen.

![AML Studio](aml-studio.png)

This version of AML Studio includes a number of improvments.  They include:

- Improved display of AML in the collapsed state
  
  ![Collapsed AML](aml-studio-tree.png)

- Table display of AML that is editable and can be converted back to AML statements
  
  ![Editing a table](aml-studio-table-edit.png)

- Support for parameterized queries leveraging the `@` symbol. Parameters can be 
  either the entiretly of a property or attribute or a portion of a SQL segment
  within a property or attribute

  ![Parameter substitutions](aml-studio-param.png)

- Support for find/replace including a normal mode, extended mode (supports \r, \n, 
  etc.), Regex mode, and XPath mode.

  ![Find/replace](aml-studio-find-replace.png)

- Many useful commands (e.g. viewing items, invoking actions, running reports, etc.) 
  can be invoked directly from the output editor's context menu
  
  ![Context menu](aml-studio-context-menu.png)
  
- Go directly to items referenced in AML by Ctrl+clicking on the links.
  
  ![Links](aml-studio-links.png)
  
- Reference useful queries in the AML Cookbook folder of the Table of Contents
  
  ![Cookbook](aml-studio-cookbook.png)
  
- The query currently under the cursor can be executed with `Ctrl+Enter`
- Auto-completion uses single quotes for easier inclusion in C# multi-line strings