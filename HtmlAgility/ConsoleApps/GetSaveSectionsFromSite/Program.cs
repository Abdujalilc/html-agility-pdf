using Microsoft.Extensions.DependencyInjection;
using Services;

ConsoleKeyInfo test1 = Console.ReadKey();
var mode = args[0].ToLower();

var services = new ServiceCollection();
services.AddHttpClient();
services.AddTransient<IMssqlTutorialService, MssqlTutorialService>();
services.AddTransient<IMvcTutorialService, MvcTutorialService>();
var provider = services.BuildServiceProvider();

if (args.Length == 0)
{
    Console.WriteLine("Please specify 'mssql' or 'mvc' and the tutorial URL.");
    return;
}


var url = args.Length > 1 ? args[1] : string.Empty;
var outputDir = args.Length > 2 ? args[2] : null;

switch (mode)
{
    case "mssql":
        var mssql = provider.GetRequiredService<IMssqlTutorialService>();
        await mssql.GetMssqlTutorials(url, outputDir);
        break;
    case "mvc":
        var mvc = provider.GetRequiredService<IMvcTutorialService>();
        await mvc.GetMvcTutorials(url, outputDir);
        break;
    default:
        Console.WriteLine("Unknown mode. Use 'mssql' or 'mvc'.");
        break;
}

// Add delay for debugging
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
