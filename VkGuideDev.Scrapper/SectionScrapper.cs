using HtmlAgilityPack;

namespace VkGuideDev.Scrapper;

public static class SectionScrapper
{
    private static int _counter;
    
    /// <summary>
    /// Returns total index of file written
    /// </summary>
    /// <param name="plan"></param>
    /// <param name="web"></param>
    /// <param name="outputPath"></param>
    /// <param name="httpClient"></param>
    /// <returns></returns>
    public static async Task<int> ScrapAsync(List<SectionPlan> plan, HtmlWeb web, string outputPath, HttpClient httpClient)
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


                    var savePath = Path.Combine(outputPath, imgPath);
                    var directory = Path.GetDirectoryName(savePath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    
                    await File.WriteAllBytesAsync(savePath, imgBytes);

                    img.Attributes["src"].Value = imgPath;
                }
            }
            
    
            await File.WriteAllTextAsync(Path.Combine(outputPath, _counter + ".html"), contentNode.InnerHtml);
            _counter++;
            
            if (sectionToExtract.SubSections != null)
            {
                await ScrapAsync(sectionToExtract.SubSections, web, outputPath, httpClient);
            }
        }

        return _counter;
    }
}