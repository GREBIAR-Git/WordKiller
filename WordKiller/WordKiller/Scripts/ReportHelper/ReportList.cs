using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace WordKiller.Scripts.ReportHelper
{
    public static class ReportList
    {
        public static void ListOfReferences(WordprocessingDocument doc, string list)
        {
            NumberingDefinitionsPart numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingPart == null)
            {
                numberingPart = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
                Numbering element = new();
                element.Save(numberingPart);
            }

            int abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
            Level[] levels = new Level[9];
            string levelText = string.Empty;

            levelText += "%" + (1);
            levels[0] = new()
            {
                NumberingFormat = new NumberingFormat() { Val = NumberFormatValues.Decimal },
                StartNumberingValue = new StartNumberingValue() { Val = 1 },
                LevelText = new LevelText() { Val = levelText + "." },
                LevelIndex = 0,
                LevelSuffix = new LevelSuffix()
                {
                    Val = LevelSuffixValues.Space
                },
                PreviousParagraphProperties = new PreviousParagraphProperties()
                {
                    Indentation = new Indentation()
                    {
                        Start = (0).ToString(),
                        Hanging = (-(int)(1.25 * 1 * ReportPageSettings.cm_to_pt)).ToString(),
                    }
                }
            };

            AbstractNum abstractNum1 = new(levels) { AbstractNumberId = abstractNumberId, MultiLevelType = new MultiLevelType() { Val = MultiLevelValues.HybridMultilevel } };
            if (abstractNumberId == 1)
            {
                numberingPart.Numbering.Append(abstractNum1);
            }
            else
            {
                AbstractNum lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
                numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
            }

            int numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
            NumberingInstance numberingInstance1 = new() { NumberID = numberId };
            AbstractNumId abstractNumId1 = new() { Val = abstractNumberId };
            numberingInstance1.Append(abstractNumId1);

            if (numberId == 1)
            {
                numberingPart.Numbering.Append(numberingInstance1);
            }
            else
            {
                var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
                numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
            }
            Body body = doc.MainDocumentPart.Document.Body;

            string[] items = list.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            int level = 0;
            if (items.Length > 0)
            {
                level = Level(items[0]);
            }
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(items[i]))
                {
                    string itemText = items[i][StartLine(items[i], Level(items[i]))..].Trim();
                    string item = itemText[..1];
                    if (itemText.Length > 1)
                    {
                        if (itemText[1] == char.ToUpper(itemText[1]))
                        {
                            item = itemText[..1];
                        }
                        item += itemText[1..];
                    }
                    level = Level(items[i]);
                    Paragraph paragraph = body.AppendChild(new Paragraph());

                    paragraph.ParagraphProperties = new ParagraphProperties(
                        new NumberingProperties(
                            new NumberingLevelReference() { Val = Level(items[i]) },
                            new NumberingId() { Val = numberId }),
                        new ParagraphStyleId() { Val = "Список" }
                        );

                    Run run = paragraph.AppendChild(new Run());
                    run.AppendChild(new Text() { Text = item, Space = SpaceProcessingModeValues.Preserve });
                }
            }
        }

        public static void Create(WordprocessingDocument doc, string list)
        {
            NumberingDefinitionsPart numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingPart == null)
            {
                numberingPart = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
                Numbering element = new();
                element.Save(numberingPart);
            }

            int abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
            Level[] levels = new Level[9];
            string levelText = string.Empty;

            levelText += "%" + (1);
            levels[0] = new()
            {
                NumberingFormat = new NumberingFormat() { Val = NumberFormatValues.Decimal },
                StartNumberingValue = new StartNumberingValue() { Val = 1 },
                LevelText = new LevelText() { Val = levelText + ")" },
                LevelIndex = 0,
                LevelSuffix = new LevelSuffix()
                {
                    Val = LevelSuffixValues.Space
                },
                PreviousParagraphProperties = new PreviousParagraphProperties()
                {
                    Indentation = new Indentation()
                    {
                        Start = (0).ToString(),
                        Hanging = (-(int)(1.25 * 1 * ReportPageSettings.cm_to_pt)).ToString(),
                    }
                }
            };
            levelText += ".";

            for (int i = 1; i < 9; i++)
            {
                levelText += "%" + (i + 1);
                levels[i] = new()
                {
                    NumberingFormat = new NumberingFormat() { Val = NumberFormatValues.Decimal },
                    StartNumberingValue = new StartNumberingValue() { Val = 1 },
                    LevelText = new LevelText() { Val = levelText + ")" },
                    LevelIndex = i,
                    LevelSuffix = new LevelSuffix()
                    {
                        Val = LevelSuffixValues.Space
                    },
                    PreviousParagraphProperties = new PreviousParagraphProperties()
                    {
                        Indentation = new Indentation()
                        {
                            Start = ((int)(0.63f * (i) * 2 * ReportPageSettings.cm_to_pt)).ToString(),
                            Hanging = (-(int)(0.63f * 1 * ReportPageSettings.cm_to_pt)).ToString(),
                        }
                    }
                };
                levelText += ".";
            }

            AbstractNum abstractNum1 = new(levels) { AbstractNumberId = abstractNumberId, MultiLevelType = new MultiLevelType() { Val = MultiLevelValues.HybridMultilevel } };
            if (abstractNumberId == 1)
            {
                numberingPart.Numbering.Append(abstractNum1);
            }
            else
            {
                AbstractNum lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
                numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
            }

            int numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
            NumberingInstance numberingInstance1 = new() { NumberID = numberId };
            AbstractNumId abstractNumId1 = new() { Val = abstractNumberId };
            numberingInstance1.Append(abstractNumId1);

            if (numberId == 1)
            {
                numberingPart.Numbering.Append(numberingInstance1);
            }
            else
            {
                var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
                numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
            }
            Body body = doc.MainDocumentPart.Document.Body;

            string[] items = list.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            int level = 0;
            if (items.Length > 0)
            {
                level = Level(items[0]);
            }
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(items[i]))
                {
                    string itemText = items[i][StartLine(items[i], Level(items[i]))..].Trim();
                    string item = itemText[..1].ToLower();
                    if (itemText.Length > 1)
                    {
                        if (itemText[1] == char.ToUpper(itemText[1]))
                        {
                            item = itemText[..1];
                        }
                        item += itemText[1..];
                    }
                    string end;
                    if (i + 1 < items.Length)
                    {
                        if (Level(items[i]) < Level(items[i + 1]))
                        {
                            end = ":";
                        }
                        else
                        {
                            end = ";";
                        }
                    }
                    else
                    {
                        end = ".";
                    }
                    level = Level(items[i]);
                    Paragraph paragraph = body.AppendChild(new Paragraph());

                    paragraph.ParagraphProperties = new ParagraphProperties(
                        new NumberingProperties(
                            new NumberingLevelReference() { Val = Level(items[i]) },
                            new NumberingId() { Val = numberId }),
                        new ParagraphStyleId() { Val = "Список" }
                        );

                    Run run = paragraph.AppendChild(new Run());
                    run.AppendChild(new Text() { Text = item + end, Space = SpaceProcessingModeValues.Preserve });
                }
            }
        }

        static int Level(string str)
        {
            int level = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '!')
                {
                    level++;
                }
                else
                {
                    break;
                }
            }
            return level;
        }

        static int StartLine(string line, int current)
        {
            int start = 1;
            if (line.Length < current)
            {
                start += current;
            }
            else
            {
                start = current;
            }
            return start;
        }

    }
}
