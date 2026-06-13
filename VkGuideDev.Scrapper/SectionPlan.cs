namespace VkGuideDev.Scrapper;

public record SectionPlan(string Title, string Link, List<SectionPlan>? SubSections);
