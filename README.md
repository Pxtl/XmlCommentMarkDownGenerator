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

When used as a nuget package, it will add a target to your project to automatically convert generated xml into markdown file stored 
in Docs at the solution level (peer to the project folder).

You must have XML documentation output enabled for your project in both debug and release configurations or it will warn that it can't find the file.
