using HtmlAgilityPack;

namespace VkGuideDev.Scrapper;

public static class SectionScrapper
{
    public static async Task ScrapAsync(List<SectionPlan> plan, HtmlWeb web, HttpClient httpClient)
    {
        foreach (var sectionToExtract in plan)
        {
            await Task.Delay(500);
            var doc = web.Load(sectionToExtract.Link);
            var contentNode = doc.DocumentNode.SelectSingleNode("//div[@id='main-content']");
    
            // Remove all svg decorators
            var svgNodes = doc.DocumentNode.SelectNodes("//svg");
            if (svgNodes != null)
            {
                foreach (var svg in svgNodes)
                {
                    svg.ParentNode.RemoveChild(svg);
                }
            }
    
            // Detect the images, download them next to the html and update the paths
            var imgNodes = doc.DocumentNode.SelectNodes("//img");
            if (imgNodes != null)
            {
                foreach (var img in imgNodes)
                {
                    var imgPath = img.Attributes["src"].Value;
                    var downloadImgPath = $"{Consts.MainUrl}{imgPath}";

                    await Task.Delay(500);
                    var imgBytes = await httpClient.GetByteArrayAsync(downloadImgPath);
                    imgPath = imgPath.Substring(1, imgPath.Length - 1);

                    var directory = Path.GetDirectoryName(imgPath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    await File.WriteAllBytesAsync(imgPath, imgBytes);

                    img.Attributes["src"].Value = imgPath;
                }
            }
            
    
            Console.WriteLine(sectionToExtract.Title);
            await File.WriteAllTextAsync(sectionToExtract.Title + ".html", contentNode.InnerHtml);
            
            if (sectionToExtract.SubSections != null)
            {
                await ScrapAsync(sectionToExtract.SubSections, web, httpClient);
            }
        }
    }
}