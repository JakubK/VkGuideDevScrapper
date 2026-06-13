using HtmlAgilityPack;
using VkGuideDev.Scrapper;

var web = new HtmlWeb();
var mainDoc = await web.LoadFromWebAsync(Consts.MainUrl);
var plan = NavigationScrapper.ScrapGuideNavigation(mainDoc);
await SectionScrapper.ScrapAsync(plan, web, new HttpClient());
