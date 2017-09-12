# PxtlCa.XmlCommentMarkDownGenerator.MSBuild #

## Type GenerateMarkdown

 A task that generates and optionally merges markdown 



---
#### Property GenerateMarkdown.InputXml

 The file(s) from which to generate markdown. This should be in XmlDocumentation format. 



---
#### Property GenerateMarkdown.DocumentationPath

 DocumentationPath is the top level directory in which to search for files. It is also the path where generated markdown files are created. 



---
#### Property GenerateMarkdown.MergeFiles

 Whether the generated markdown files should merge. Only valid if multiple markdown files exist. DocumentationPath is the top level directory in which to search for files. Both existing markdown files and the generated files are merged. 



---
#### Property GenerateMarkdown.OutputFile

 The file to be created by the merge. Unused if MergeFiles evaluates to false. 



---
#### Method GenerateMarkdown.Execute

 Runs the task as configured 

**Returns**: true if task has succeeded



---
#### Property GenerateMarkdown.GeneratedMDFiles

 The files generated during execution of the task 



---


