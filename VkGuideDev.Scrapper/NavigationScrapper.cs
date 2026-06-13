using HtmlAgilityPack;

namespace VkGuideDev.Scrapper;

public static class NavigationScrapper
{
    public static List<SectionPlan> ScrapGuideNavigation(HtmlDocument doc)
    {
        var sections = new List<SectionPlan>();
        
        var navigationList = doc.DocumentNode.SelectSingleNode("//ul[@class='navigation-list']");

        var counter = 0;
        
        foreach (var section in navigationList.ChildNodes)
        {
            var mainSectionTitle = section.FirstChild.InnerText;
            if (mainSectionTitle.Contains("Korean") || mainSectionTitle.Contains("Legacy"))
            {
                continue;
            }
    
            Console.WriteLine(mainSectionTitle);
            var mainSectionLink = section.FirstChild.Attributes["href"].Value;
            mainSectionLink = $"https:{mainSectionLink}";
            
            var parsedSection = new SectionPlan(counter.ToString("D3") + " " + mainSectionTitle, mainSectionLink, null);
            counter++;
            
            var subsectionsPlan = new List<SectionPlan>();
            if (section.ChildNodes.Count > 1)
            {
                var nestedList = section.ChildNodes[1];
                foreach (var subsection in nestedList.ChildNodes)
                {
                    var subTitle = subsection.FirstChild.InnerText;
                    var subLink = subsection.FirstChild.Attributes["href"].Value;
                    subLink = $"https:{subLink}";
            
                    subsectionsPlan.Add(new (counter.ToString("D3") + " " + subTitle, subLink, null));
                    counter++;
                }
            }
            
            sections.Add(parsedSection with
            {
                SubSections = subsectionsPlan
            });
        }

        return sections;
    }
}