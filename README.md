# XmlCommentMarkDownGenerator

Execute PxtlCa.XmlCommentMarkDownGenerator.exe -help for usage.

Generates Markdown from VS XML documentation file.  Forked from https://gist.github.com/lontivero/593fc51f1208555112e0 

Can be used as a stand-alone Markdown command-line tool, but is also available as a NuGet package.  

When used as a nuget package, it will add a target to your project to automatically convert generated xml into markdown file stored 
in Docs at the solution level (peer to the project folder).

TODO: error handling if the XML file does not exist.  Right now you *must* have XML document generation enabled for your project in 
both debug and release or this will error out as it can't find the file.