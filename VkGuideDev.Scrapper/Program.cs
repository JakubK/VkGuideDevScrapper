using HtmlAgilityPack;
using VkGuideDev.Scrapper;

var outputPath = args[0];
var web = new HtmlWeb();
var mainDoc = await web.LoadFromWebAsync(Consts.MainUrl);
var plan = NavigationScrapper.ScrapGuideNavigation(mainDoc);
var maxIndex = await SectionScrapper.ScrapAsync(plan, web, outputPath, new HttpClient());
Console.WriteLine(maxIndex);
