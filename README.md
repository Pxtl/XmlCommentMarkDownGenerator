# XmlCommentMarkDownGenerator

Generates Markdown from VS XML documentation file.  Forked from https://gist.github.com/lontivero/593fc51f1208555112e0 

XmlCommentMarkDownGenerator is available as both a command-line tool and as an MSBuild target.
There is not currently feature-parity between these two modes.  
Features involving merging a MarkDown document into the generated file are currently target-only.

## Command Line Tool

Usage: PxtlCa.XmlCommentMarkDownGenerator -i InputFileName.xml -o OutputFileName.md

  -i, --inputfile     Input xml file to read.

  --cin               Read input from console instead of file.

  -o, --outputfile    Output md file to write.

  --cout              Write output to console instead of file.

  --help              Display this help screen.

Execute PxtlCa.XmlCommentMarkDownGenerator.exe --help for usage if the above is out-of-date.

## Nuget Package/MsBuild Target

https://www.nuget.org/packages/PxtlCa.XmlCommentMarkDownGenerator

When used as a nuget package, it will add an MSBuild task to your project to automatically convert generated xml into markdown file stored in Docs at the project level.  It will also merge any existing markdown files in Docs with the converted markdown. Takes multiple input xml files. 

(note: the above nuget target was broken after 0.1.5977.1837 because I forgot to commit the nuspec line that does it.  Oops.  Fixed in 0.2.6130.564)

You must have XML documentation output enabled for your project in both debug and release configurations or it will warn that it can't find the file.

In the basic case, this is all you need.  However, if you need to merge with other markdown documents, XmlCommentMarkDownGenerator has additional features.

### Working Files and Directories

Assuming your project follows this layout

- root
    - docs
	- myApplication
		- program.cs
		- readme.md
	- myApplication.Lib
		- class1.cs
		- class2.cs
		- readme.md
	- myApplication.Tests
		- test1.cs
		- test2.cs
		- readme.md
	- myApplication.sln
	- readme.md

Given this typical layout, if we apply XmlCommentMarkDownGenerator to myApplication.Lib, 
the program will search for .md files in myApplication.Lib's folder.

The .md file will be considered an eligible markdown header if it matches one of the following criteria:

1) The file contains a YAML header block (more on this later)
2) The file is named "readme.md" (case-insensitive)
3) The file is named to match the assembly (in the case of this example, "myApplication.Lib.md")

In that order.  If the generator finds files with YAML header blocks in the project folder, 
it will not search by filename.

When it finds the file, it will generate the MD from the generated XML, 
append it to the end of the text in the found header file, and write out the result
at /docs/"assemblyName".md.  So in our example case, it would be /docs/myApplication.lib.md.

### YAML Header Blocks

For more precise control, you can modify your header file with a YAML header block.  Example below:

```yaml
---
UnexpectedTagAction: Error
OutputFile: myApplicationNameChanged.lib.md
MergeSequence: 1
---
```

This would be followed by normal MarkDown text.
Note that the YAML parser requires the three dashes to be the very first line in the file.

- **UnexpectedTagAction**: can be one of Error, Warn, or None.  Determines how the generator should respond to an unexpected XML tag.
- **OutputFile**: path including filename of the output file.  Relative to the ../Docs folder.
- **MergeSequence**: If merging multiple files into the same output file, this entry will be used to order them using a crude natural sort.

Note that the merge sequence is meaningless in typical workflow - each project generates one XML files, 
and each executes the markdown generator separately.

### MSBuild Options and Merging Multiple Files

If you wish to manually customize your proj files for msbuild, you can feed more complicated targets into the MSBuild processor.  
You can choose a better target to ../Docs, provide multiple XML files in a single execution (allowing your doc files to be concatenated together) 
and provide an alternate source documents folder.  See GenerateMarkdown.cs for details.