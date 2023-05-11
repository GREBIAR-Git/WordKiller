using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.Scripts.ReportHelper
{
    public static class ReportTable
    {
        public static void Create(WordprocessingDocument doc, TableData dataTable)
        {
            Table dTable = new();
            TableProperties props = new();
            dTable.AppendChild(props);

            for (int i = 0; i < dataTable.Rows; i++)
            {
                TableRow tr = new();
                for (int f = 0; f < dataTable.Columns; f++)
                {
                    DataCell(tr, dataTable.Columns, 0, dataTable.DataTable[i, f]);
                }
                dTable.Append(tr);
            }

            var tableWidth = new TableWidth()
            {
                Width = "5000",
                Type = TableWidthUnitValues.Pct
            };
            props.Append(tableWidth);

            EnumValue<BorderValues> borderValues = new(BorderValues.Single);
            TableBorders tableBorders = new(
                                 new TopBorder { Val = borderValues, Size = 4 },
                                 new BottomBorder { Val = borderValues, Size = 4 },
                                 new LeftBorder { Val = borderValues, Size = 4 },
                                 new RightBorder { Val = borderValues, Size = 4 },
                                 new InsideHorizontalBorder { Val = borderValues, Size = 4 },
                                 new InsideVerticalBorder { Val = borderValues, Size = 4 });

            props.Append(tableBorders);

            doc.MainDocumentPart.Document.Body.Append(dTable);
        }

        static void DataCell(TableRow tr, int numberOfСolumns, int idx, string text)
        {
            if (numberOfСolumns > idx)
            {
                TableCell tc = new();
                tc.Append(new Paragraph(new Run(new Text() { Text = text, Space = SpaceProcessingModeValues.Preserve }))
                {
                    ParagraphProperties = new ParagraphProperties()
                    {
                        ParagraphStyleId = new ParagraphStyleId() { Val = "Таблица" }
                    }
                });
                tc.Append(new TableCellProperties());
                tr.Append(tc);
            }
        }
    }
}
