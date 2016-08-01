

# This repository is forked from Pxtl  
https://github.com/Pxtl/XmlCommentMarkDownGenerator


# XmlCommentMarkDownGenerator

Usage: PxtlCa.XmlCommentMarkDownGenerator -i InputFileName.xml -o OutputFileName.md

  -i, --inputfile     Input xml file to read.

  --cin               Read input from console instead of file.

  -o, --outputfile    Output md file to write.

  --cout              Write output to console instead of file.

  --help              Display this help screen.

Execute PxtlCa.XmlCommentMarkDownGenerator.exe -help for usage if the above is out-of-date.

Generates Markdown from VS XML documentation file.  Forked from https://gist.github.com/lontivero/593fc51f1208555112e0 

Can be used as a stand-alone Markdown command-line tool, but is also available as a NuGet package.  

https://www.nuget.org/packages/PxtlCa.XmlCommentMarkDownGenerator

When used as a nuget package, it will add a target to your project to automatically convert generated xml into markdown file stored 
in Docs at the solution level (peer to the project folder).

TODO: error handling if the XML file does not exist.  Right now you *must* have XML document generation enabled for your project in 
both debug and release or this will error out as it can't find the file.

There is currently a bug that the --help is spat out twice, once with missing descriptions.  WIP.