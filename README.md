# XmlCommentMarkDownGenerator

Usage: PxtlCa.XmlCommentMarkDownGenerator -i InputFileName.xml -o OutputFileName.md

  -i, --inputfile     Input xml file to read.

  --cin               Read input from console instead of file.

  -o, --outputfile    Output md file to write.

  --cout              Write output to console instead of file.

  --help              Display this help screen.

Execute PxtlCa.XmlCommentMarkDownGenerator.exe --help for usage if the above is out-of-date.

Generates Markdown from VS XML documentation file.  Forked from https://gist.github.com/lontivero/593fc51f1208555112e0 

Can be used as a stand-alone Markdown command-line tool, but is also available as a NuGet package.  

https://www.nuget.org/packages/PxtlCa.XmlCommentMarkDownGenerator

When used as a nuget package, it will add an MSBuild task to your project to automatically convert generated xml into markdown file stored in Docs at the project level.  It will also merge any existing markdown files in Docs with the converted markdown. Takes multiple input xml files. 

(note: the above nuget target was broken after 0.1.5977.1837 because I forgot to commit the nuspec line that does it.  Oops.  Fixed in 0.2.6130.564)

You must have XML documentation output enabled for your project in both debug and release configurations or it will warn that it can't find the file.
