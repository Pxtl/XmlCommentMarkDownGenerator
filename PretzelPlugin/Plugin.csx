using Pretzel.Logic.Templating;
using Pretzel.Logic.Templating.Context;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;


[SiteEngineInfo(Engine="customxmlmd")]
public class Processor : ISiteEngine
{

	public string AllowableCustomTags {get; set;}
	public void Initialize()
	{
	
	}
	
	public bool CanProcess(SiteContext context)
	{
		var readmePage = context.Posts.Where(p => p.File.ToLower().EndsWith("readme.md")).FirstOrDefault();
		if(null == readmePage)
		{
		//if no readme.md then return false
			return false;
		}
		if(readmePage.Bag.ContainsKey("mergexmlcomments"))
		{
			Console.WriteLine("about to check 'mergexmlcomments' a bool");
			if(!(bool)(readmePage.Bag["mergexmlcomments"]))
			{
				Console.WriteLine("as boolean 'mergexmlcomments' is false");
			//if there is a mergexmlcomments value in the front matter
			//but it is false
				return false;
			}
			Console.WriteLine("as boolean 'mergexmlcomments' is true");
			AllowableCustomTags = (string)(readmePage.Bag["allowedcustomtags"]);
			return true;
		}
		else
		{
			Console.WriteLine("Bag doesn't contain 'mergexmlcomments'");
		//no mergexmlcomments value
			return false;
		}
	}
	
	public void Process(SiteContext context, bool skipFileOnError = false)
	{
			Console.WriteLine($@"About to check CanProcess(context) again");
			if(CanProcess(context))
			{
			Console.WriteLine($@"Custom Tag to allow: {AllowableCustomTags}");
			}
			else
			{
				Console.WriteLine($@"CanProcess(context) is false this time");
			}
	}
}
