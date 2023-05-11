using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordKiller.Models.Template;

namespace WordKiller.Scripts.ReportHelper
{
    public static class ReportStyles
    {
        public const byte pt_to_halfpt = 2;
        public static void Init(WordprocessingDocument doc, DataTypes.Enums.DocumentType typeDocument)
        {
            StyleDefinitionsPart styleDefinitions = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();

            Styles styles = new();

            styles.Save(styleDefinitions);
            styles = styleDefinitions.Styles;


            styles.Append(
                Init("EmptyLines", justify: JustificationValues.Center));

            foreach (TemplateType templateType in Properties.Settings.Default.TemplateTypes)
            {
                if (templateType.Type == typeDocument)
                {
                    foreach (Template template in templateType.Templates)
                    {
                        if (template.Name == "Раздел")
                        {
                            styles.Append(Init(template.Name, template.Size, template.Justify, template.Bold, template.Before, template.After, template.LineSpacing, template.Left, template.Right, template.FirstLine, true, outlineLevel: 1));
                            styles.Append(Init(template.Name + "Приложение", template.Size, template.Justify, template.Bold, template.Before, template.After, template.LineSpacing, template.Left, template.Right, 0f, true, outlineLevel: 1));
                        }
                        else if (template.Name == "Подраздел")
                        {
                            styles.Append(Init(template.Name, template.Size, template.Justify, template.Bold, template.Before, template.After, template.LineSpacing, template.Left, template.Right, template.FirstLine, outlineLevel: 2));
                        }
                        else if (template.Name == "Список")
                        {
                            styles.Append(Init(template.Name, template.Size, template.Justify, template.Bold, template.Before, template.After, template.LineSpacing, template.Left, template.Right, template.FirstLine, hanging: 0.63f));
                        }
                        else
                        {
                            styles.Append(Init(template.Name, template.Size, template.Justify, template.Bold, template.Before, template.After, template.LineSpacing, template.Left, template.Right, template.FirstLine));
                        }
                    }
                }
            }
        }

        public static Style Init(string name, int size = 14,
            JustificationValues justify = JustificationValues.Left, bool bold = false,
            int before = 0, int after = 0, float multiplier = 1, float left = 0, float right = 0, float firstLine = 0, bool caps = false, float hanging = 0, int outlineLevel = 0)
        {
            var style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = name,
                CustomStyle = true,
                Default = false
            };


            style.Append(new StyleName()
            {
                Val = name
            });

            var styleRunProperties = new StyleRunProperties();
            styleRunProperties.Append(new RunFonts()
            {

                Ascii = "Times New Roman",
                HighAnsi = "Times New Roman"
            });
            styleRunProperties.Append(new FontSize()
            {
                Val = (size * pt_to_halfpt).ToString()
            });
            styleRunProperties.Append(new Caps()
            {
                Val = caps
            });
            if (bold)
            {
                styleRunProperties.AddChild(new Bold());
            }

            ParagraphProperties paragraphProperties = new();
            paragraphProperties.AddChild(new Justification()
            {
                Val = justify
            });

            if (outlineLevel != 0)
            {
                paragraphProperties.AddChild(new OutlineLevel()
                {
                    Val = outlineLevel - 1
                });
            }

            paragraphProperties.AddChild(new SpacingBetweenLines()
            {
                After = (after * 20).ToString(),
                Before = (before * 20).ToString(),
                Line = (multiplier * 240).ToString(),
                LineRule = LineSpacingRuleValues.Auto
            });
            if (hanging == 0)
            {
                paragraphProperties.AddChild(new Indentation()
                {

                    Left = ((int)(left * ReportPageSettings.cm_to_pt)).ToString(),
                    Right = ((int)(right * ReportPageSettings.cm_to_pt)).ToString(),
                    FirstLine = ((int)(firstLine * ReportPageSettings.cm_to_pt)).ToString(),
                });
            }
            else
            {
                paragraphProperties.AddChild(new Indentation()
                {
                    Left = ((int)((left + hanging) * ReportPageSettings.cm_to_pt)).ToString(),
                    Right = ((int)(right * ReportPageSettings.cm_to_pt)).ToString(),
                    Hanging = ((int)(hanging * ReportPageSettings.cm_to_pt)).ToString(),
                });
            }
            style.Append(styleRunProperties);
            style.Append(paragraphProperties);
            return style;
        }
    }
}
