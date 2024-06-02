using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Models;
using WordKiller.Models.Template;
using WordKiller.ViewModels;
using DocumentType = WordKiller.DataTypes.Enums.DocumentType;
using Settings = WordKiller.Properties.Settings;

namespace WordKiller.Scripts.ReportHelper;

public static class ReportComplexObjects
{
    public static void TitlePart(WordprocessingDocument doc, DocumentType typeDocument, ViewModelTitle title)
    {
        if (title.Photo)
        {
            string id = ReportImage.Registration(doc, title.Picture);
            if (id != null && title.Picture.Bitmap != null)
            {
                ReportImage.FullScreen(doc, id);
            }
        }
        else
        {
            if (typeDocument == DocumentType.ProductionPractice)
            {
                ProductionPractice(doc, title);
            }
            else
            {
                Ministry(doc, title.Cathedra);
                switch (typeDocument)
                {
                    case DocumentType.LaboratoryWork:
                        LabPra(doc, "лабораторной", title);
                        break;
                    case DocumentType.PracticeWork:
                        LabPra(doc, "практической", title);
                        break;
                    case DocumentType.Coursework:
                        Coursework(doc, title);
                        break;
                    case DocumentType.ControlWork:
                        ControlWork(doc, title);
                        break;
                    case DocumentType.Referat:
                        Referat(doc, title);
                        break;
                    case DocumentType.VKR:
                        VKR(doc, title);
                        break;
                }
            }

            Orel(doc);
        }

        ReportExtras.SectionBreak(doc);
    }

    public static void Text(WordprocessingDocument doc, List<Line> lines, DocumentData data)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        Paragraph paragraph = new();
        int i1 = 0;
        TemplateType templateType = null;
        foreach (var item in Settings.Default.TemplateTypes)
        {
            if (data.Type == item.Type)
            {
                templateType = item;
                break;
            }
        }

        int g = 0;
        foreach (Line line in lines)
        {
            g++;
            if (line.newPara)
            {
                if (line.ParagraphProperties != null)
                {
                    paragraph = body.AppendChild(new Paragraph());
                    paragraph.AppendChild(new ParagraphProperties(line.ParagraphProperties));
                }
                else
                {
                    paragraph = body.AppendChild(new Paragraph());
                }
            }
            else
            {
                string text = line.Text;

                Run run = paragraph.AppendChild(new Run());
                if (line.RunProperties != null)
                {
                    RunProperties runProperties = new(line.RunProperties);
                    if (runProperties.Highlight != null && runProperties.Highlight.Val == HighlightColorValues.Yellow &&
                        !string.IsNullOrEmpty(line.Text) && line.Text != "\n")
                    {
                        text = data.Title.GetData(templateType.YellowFragment[i1].Index);
                        runProperties.Highlight = new() { Val = HighlightColorValues.None };
                        run.PrependChild(runProperties);
                        ReportText.TextToRun(run, text);
                        i1++;
                    }
                    else
                    {
                        run.PrependChild(runProperties);
                        ReportText.TextToRun(run, line.Text);
                    }
                }
            }
        }
    }


    static void LabPra(WordprocessingDocument doc, string type, ViewModelTitle title)
    {
        string text = "ОТЧЁТ";
        ReportText.Text(doc, text, 16, "center", true);
        text = "По " + type + " работе №" + title.Number;
        ReportText.Text(doc, text, 16, "center", after: 10);
        text = "на тему: «" + title.Theme + "»";
        ReportText.Text(doc, text, justify: "center");
        text = "по дисциплине: «" + title.Discipline + "»";
        ReportText.Text(doc, text, justify: "center");
        ReportExtras.EmptyLines(doc, 8);
        if (title.OnePerformed())
        {
            text = "Выполнил: ";
        }
        else
        {
            text = "Выполнили: ";
        }

        text += title.AllPerformed();
        ReportText.Text(doc, text);
        text = Settings.Default.Faculty;
        ReportText.Text(doc, text);
        text = "Направление: " + Settings.Default.Direction;
        ReportText.Text(doc, text);
        text = "Группа: " + Settings.Default.Group;
        ReportText.Text(doc, text);

        text = "Проверил: " + title.Professor;
        ReportText.Text(doc, text, after: 10);
        ReportExtras.EmptyLines(doc, 1);

        text = "Отметка о зачёте: ";
        ReportText.Text(doc, text);

        text = "Дата: «____» __________ " + SpaceForYear(Settings.Default.Year) + "г.";
        ReportText.Text(doc, text, justify: "right");

        ReportExtras.EmptyLines(doc, 8);
    }

    static void Coursework(WordprocessingDocument doc, ViewModelTitle title)
    {
        string text = "Работа допущена к защите";
        ReportText.Text(doc, text, multiplier: 1.5f, left: 9.5f);
        text = "______________Руководитель";
        ReportText.Text(doc, text, multiplier: 1.5f, left: 9.5f);
        text = "«____»_____________" + SpaceForYear(Settings.Default.Year) + "г.";
        ReportText.Text(doc, text, multiplier: 1.5f, left: 9.5f);

        ReportExtras.EmptyLines(doc, 3);

        if (title.Work)
        {
            text = "КУРСОВАЯ РАБОТА";
        }
        else
        {
            text = "КУРСОВОЙ ПРОЕКТ";
        }

        ReportText.Text(doc, text, justify: "center", bold: true);

        ReportExtras.EmptyLines(doc, 1);

        text = "по дисциплине: «" + title.Discipline + "»";
        ReportText.Text(doc, text, multiplier: 2);

        text = "на тему: «" + title.Theme + "»";
        ReportText.Text(doc, text, multiplier: 1.5f);

        ReportExtras.EmptyLines(doc, 2);

        User performed = title.FirstPerformed();

        text = "Студент _________________" + performed.Full;
        ReportText.Text(doc, text, multiplier: 1.5f);
        text = "Шифр " + performed.Shifr;
        ReportText.Text(doc, text, multiplier: 1.5f);
        text = Settings.Default.Faculty;
        ReportText.Text(doc, text, multiplier: 1.5f);
        text = "Направление: " + Settings.Default.Direction;
        ReportText.Text(doc, text, multiplier: 1.5f);
        text = "Группа: " + Settings.Default.Group;
        ReportText.Text(doc, text, multiplier: 1.5f);

        text = "Руководитель __________________" + title.Professor;
        ReportText.Text(doc, text, multiplier: 1.5f, after: 12);

        text = "Оценка: «________________»               Дата ______________";
        ReportText.Text(doc, text);

        ReportExtras.EmptyLines(doc, 5);
    }

    static void ControlWork(WordprocessingDocument doc, ViewModelTitle title)
    {
        string text = "Контрольная работа";
        ReportText.Text(doc, text, 16, "center", true);

        text = "по дисциплине: «" + title.Discipline + "»";
        ReportText.Text(doc, text, justify: "center");

        ReportExtras.EmptyLines(doc, 10);

        if (title.OnePerformed())
        {
            text = "Выполнил: ";
        }
        else
        {
            text = "Выполнили: ";
        }

        text += title.AllPerformed();
        ReportText.Text(doc, text);
        text = Settings.Default.Faculty;
        ReportText.Text(doc, text);
        text = "Направление: " + Settings.Default.Direction;
        ReportText.Text(doc, text);
        text = "Группа: " + Settings.Default.Group;
        ReportText.Text(doc, text);

        text = "Проверил: " + title.Professor;
        ReportText.Text(doc, text, after: 10);

        ReportExtras.EmptyLines(doc, 1);

        text = "Отметка о зачёте: ";
        ReportText.Text(doc, text);

        text = "Дата: «____» __________ " + SpaceForYear(Settings.Default.Year) + "г.";
        ReportText.Text(doc, text, justify: "right");

        ReportExtras.EmptyLines(doc, 9);
    }

    static void Referat(WordprocessingDocument doc, ViewModelTitle title)
    {
        ReportExtras.EmptyLines(doc, 1);

        string text = "Реферат по дисциплине: «" + title.Discipline + "»";
        ReportText.Text(doc, text, justify: "center");
        text = "Тема: «" + title.Theme + "»";
        ReportText.Text(doc, text, justify: "center");
        ReportExtras.EmptyLines(doc, 16);

        if (title.OnePerformed())
        {
            text = "Выполнил: студент группы ";
        }
        else
        {
            text = "Выполнили: студенты группы ";
        }

        text += Settings.Default.Group;
        ReportText.Text(doc, text, justify: "right");
        text = title.AllPerformed();
        ReportText.Text(doc, text, "right");
        string cathedra = title.Cathedra;
        if (!string.IsNullOrWhiteSpace(cathedra))
        {
            string[] words = cathedra.Split(' ');
            if (words.Length > 0)
            {
                words = words.Skip(1).ToArray();
                cathedra = string.Join(" ", words);
            }
        }

        if (title.Rank == "и.о. зав. кафедрой")
        {
            text = "Проверил: " + title.Rank + " " + cathedra;
        }
        else
        {
            text = "Проверил: " + title.Rank + " кафедры " + cathedra;
        }

        ReportText.Text(doc, text, justify: "right");
        text = title.Professor;
        ReportText.Text(doc, text, justify: "right");

        ReportExtras.EmptyLines(doc, 6);
    }

    static void ProductionPractice(WordprocessingDocument doc, ViewModelTitle title)
    {
        string text = "Федеральное государственное бюджетное образовательное учреждение высшего образования";
        ReportText.Text(doc, text, 12, "center", true);

        ReportExtras.EmptyLines(doc, 1);

        text = "«ОРЛОВСКИЙ ГОСУДАРСТВЕННЫЙ УНИВЕРСИТЕТ ИМЕНИ И.С. ТУРГЕНЕВА»";
        ReportText.Text(doc, text, 12, "center", true, caps: true);

        ReportExtras.EmptyLines(doc, 1);

        text = title.Faculty;
        ReportText.Text(doc, text, 12, "center", true);

        ReportExtras.EmptyLines(doc, 1);

        text = title.Cathedra;
        ReportText.Text(doc, text, 12, "center", true);

        ReportExtras.EmptyLines(doc, 6);

        text = "ОТЧЁТ";
        ReportText.Text(doc, text, 14, "center", true);

        string type;
        if (title.Production)
        {
            type = "производственной";
        }
        else
        {
            type = "учебной";
        }

        text = "по " + type + " практике";
        ReportText.Text(doc, text, 14, "center");

        ReportExtras.EmptyLines(doc, 3);

        text = "На материалах " + title.PracticeLocation;
        ReportText.Text(doc, text, multiplier: 1.5f);

        ReportExtras.EmptyLines(doc, 4);

        User performed = title.FirstPerformed();

        text = "Студент     _________________ " + performed.Full;
        ReportText.Text(doc, text, multiplier: 1.5f);

        text = "Группа " + Settings.Default.Group;
        ReportText.Text(doc, text, before: 6, multiplier: 1.5f);

        text = "Направление " + Settings.Default.Direction;
        ReportText.Text(doc, text, before: 6, multiplier: 1.5f);

        ReportExtras.EmptyLines(doc, 1);

        text = "Руководитель практики";
        ReportText.Text(doc, text, before: 6, multiplier: 1.5f);

        text = "от университета \t\t\t\t\t________________ " + title.Professor;
        ReportText.Text(doc, text, multiplier: 1.5f, tabs: true);

        text = "Руководитель практики";
        ReportText.Text(doc, text, before: 6, multiplier: 1.5f);

        text = "от профильной организации \t\t\t________________ " + title.HeadOrganization;
        ReportText.Text(doc, text, multiplier: 1.5f, tabs: true);

        ReportExtras.EmptyLines(doc, 1);

        text = "Оценка защиты: ________________";
        ReportText.Text(doc, text, multiplier: 1.5f);

        ReportExtras.EmptyLines(doc, 1);
    }

    static void VKR(WordprocessingDocument doc, ViewModelTitle title)
    {
        ReportExtras.EmptyLines(doc, 2);
        string text = "ВЫПУСКНАЯ КВАЛИФИКАЦИОННАЯ РАБОТА";
        ReportText.Text(doc, text, justify: "center", caps: true, multiplier: 1.5f);

        text = "по направлению подготовки ";
        ReportText.Text(doc, text, justify: "center");

        text = Settings.Default.Direction;
        ReportText.Text(doc, text, justify: "center");

        text = "Направленность (профиль) " + title.Direction;
        ReportText.Text(doc, text, justify: "center");
        ReportExtras.EmptyLines(doc, 1);

        User performed = title.FirstPerformed();

        text = "Студента " + performed.LastName + " " + performed.FirstName + " " + performed.MiddleName + "\t шифр" +
               " " + performed.Shifr;
        ReportText.Text(doc, text, multiplier: 1.5f);

        text = title.Faculty;
        ReportText.Text(doc, text, multiplier: 1.5f);

        text = title.Cathedra;
        ReportText.Text(doc, text, multiplier: 1.5f);

        ReportExtras.EmptyLines(doc, 2);

        text = "Тема выпускной квалификационной работы";
        ReportText.Text(doc, text, justify: "center", after: 10);

        text = "«" + title.Theme + "»";
        ReportText.Text(doc, text, justify: "center", after: 10);

        ReportExtras.EmptyLines(doc, 2);

        text = "Студент \t" + performed.AlternateFull;
        ReportText.Text(doc, text, tabs: true);
        ReportExtras.EmptyLines(doc, 1);

        text = "Руководитель \t" + title.Professor;
        ReportText.Text(doc, text, tabs: true);
        ReportExtras.EmptyLines(doc, 2);

        text = "Нормоконтроль  \t" + title.Normocontrol;
        ReportText.Text(doc, text, tabs: true);
        ReportExtras.EmptyLines(doc, 2);

        text = "И.о. зав. кафедрой";
        ReportText.Text(doc, text);
        if (title.Cathedra.Length > 25)
        {
            int index = -1;
            for (int i = 0; i < title.Cathedra.Length; i++)
            {
                if (title.Cathedra.Length > 24)
                {
                    if (title.Cathedra[i] == ' ')
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index != -1)
            {
                text = title.Cathedra[..index];
                ReportText.Text(doc, text);
                text = title.Cathedra[index..] + "\t" + title.HeadCathedra;
                ReportText.Text(doc, text, tabs: true);
            }
            else
            {
                text = title.Cathedra + "\t" + title.HeadCathedra;
                ReportText.Text(doc, text, tabs: true);
            }
        }
        else
        {
            text = title.Cathedra + "\t" + title.HeadCathedra;
            ReportText.Text(doc, text, tabs: true);
        }

        ReportExtras.EmptyLines(doc, 1);
    }

    static void Ministry(WordprocessingDocument doc, string faculty)
    {
        string text = "МИНИСТЕРСТВО НАУКИ И ВЫСШЕГО ОБРАЗОВАНИЯ";
        ReportText.Text(doc, text, justify: "center");
        text = "РОССИЙСКОЙ ФЕДЕРАЦИИ";
        ReportText.Text(doc, text, justify: "center");
        text = "ФЕДЕРАЛЬНОЕ ГОСУДАРСТВЕННОЕ БЮДЖЕТНОЕ";
        ReportText.Text(doc, text, justify: "center");
        text = "ОБРАЗОВАТЕЛЬНОЕ УЧРЕЖДЕНИЕ ВЫСШЕГО ОБРАЗОВАНИЯ";
        ReportText.Text(doc, text, justify: "center");
        text = "«ОРЛОВСКИЙ ГОСУДАРСТВЕННЫЙ УНИВЕРСИТЕТ";
        ReportText.Text(doc, text, justify: "center");
        text = "ИМЕНИ И.С.ТУРГЕНЕВА»";
        ReportText.Text(doc, text, justify: "center");
        ReportExtras.EmptyLines(doc, 2);

        text = faculty;
        ReportText.Text(doc, text, justify: "center");
        ReportExtras.EmptyLines(doc, 3);
    }

    static void Orel(WordprocessingDocument doc)
    {
        string text = "Орел, " + SpaceForYear(Settings.Default.Year);
        ReportText.Text(doc, text, justify: "center");
    }

    static string SpaceForYear(string year, char spaceCharacter = '_')
    {
        for (int i = 0; i < 4 - year.Length; i++)
        {
            year += spaceCharacter;
        }

        return year;
    }

    public static void TaskSheet(WordprocessingDocument doc, ViewModelTitle title, ViewModelTaskSheet taskSheet)
    {
        if (taskSheet.Photo)
        {
            string id = ReportImage.Registration(doc, taskSheet.FirstPicture);
            if (id != null && taskSheet.FirstPicture.Bitmap != null)
            {
                ReportImage.FullScreen(doc, id);
            }

            ReportExtras.SectionBreak(doc);
            id = ReportImage.Registration(doc, taskSheet.SecondPicture);
            if (id != null && taskSheet.SecondPicture.Bitmap != null)
            {
                ReportImage.FullScreen(doc, id);
            }

            ReportExtras.SectionBreak(doc);
        }
        else
        {
            ReportPageSettings.PageSetup(doc.MainDocumentPart.Document.Body, title: true);
            Ministry(doc, title.Cathedra);

            string text = "УТВЕРЖДАЮ:";
            ReportText.Text(doc, text, left: 9.5f);
            text = "____________и.о. зав. кафедрой";
            ReportText.Text(doc, text, left: 9.5f);
            text = "«___»_____________" + Settings.Default.Year + "г.";
            ReportText.Text(doc, text, left: 9.5f);
            ReportExtras.EmptyLines(doc, 2);
            text = "ЗАДАНИЕ";
            ReportText.Text(doc, text, 16, "center", true);
            string type = string.Empty;
            if (title.Project)
            {
                text = "на курсовой проект";
                type = "курсового проекта";
            }
            else if (title.Work)
            {
                text = "на курсовую работу";
                type = "курсовой работы";
            }

            ReportText.Text(doc, text, 14, "center", true, multiplier: 2);
            text = "по дисциплине «" + title.Discipline + "»";
            ReportText.Text(doc, text, multiplier: 2);
            ReportExtras.EmptyLines(doc, 1);
            User performed = title.FirstPerformed();
            text = "Студент    " + performed.Full + "                Шифр " + performed.Shifr;
            ReportText.Text(doc, text, multiplier: 1.5f);
            text = Settings.Default.Faculty;
            ReportText.Text(doc, text, multiplier: 1.5f);
            text = "Направление подготовки " + Settings.Default.Direction;
            ReportText.Text(doc, text, multiplier: 1.5f);
            text = "Группа " + Settings.Default.Group;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);
            text = "1 Тема " + type;
            ReportText.Text(doc, text, multiplier: 1.5f);
            text = title.Theme;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);
            text = "2 Срок сдачи студентом законченной работы «___» _____________ " + Settings.Default.Year;
            ReportText.Text(doc, text, multiplier: 1.5f);

            ReportExtras.SectionBreak(doc);

            ReportPageSettings.PageSetup(doc.MainDocumentPart.Document.Body, 0.74f, 1.31f, 0.49f, 1.31f, true);
            text = "3 Исходные данные";
            ReportText.Text(doc, text, multiplier: 1.5f);

            text = taskSheet.SourceData;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);

            text = "4 Содержание " + type;
            ReportText.Text(doc, text, multiplier: 1.5f);

            text = taskSheet.TOC;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);

            text = "5 Отчетный материал " + type;
            ReportText.Text(doc, text, multiplier: 1.5f);

            text = taskSheet.ReportingMaterial;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);

            text = "Руководитель ________________________ " + title.Professor;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);

            text = "Задание принял к исполнению: «___» _____________ " + Settings.Default.Year;
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);

            text = "Подпись студента__________________ ";
            ReportText.Text(doc, text, multiplier: 1.5f);
            ReportExtras.EmptyLines(doc, 1);
            ReportExtras.SectionBreak(doc);
        }
    }

    public static void ListOfReferencesPart(WordprocessingDocument doc, ViewModelListOfReferences listOfReferences,
        bool on)
    {
        if (on)
        {
            if (listOfReferences.ListSourcesUsed)
            {
                ReportText.Text(doc, "СПИСОК ИСПОЛЬЗОВАННЫХ ИСТОЧНИКОВ", "Раздел");
            }
            else if (listOfReferences.Bibliography)
            {
                ReportText.Text(doc, "СПИСОК ЛИТЕРАТУРЫ", "Раздел");
            }

            List<string> resours = [];
            foreach (Book book in listOfReferences.Books)
            {
                resours.Add(book.Authors + " " + book.Name + ". " + book.Publication + ", " + book.Year + ". " +
                            book.Page + " с.");
            }

            foreach (ElectronicResource electronicResource in listOfReferences.ElectronicResources)
            {
                resours.Add(electronicResource.Name + " [Электронный ресурс]. URL: " + electronicResource.Url +
                            " (дата обращения: " + electronicResource.CirculationDate + ").");
            }

            if (listOfReferences.AlphabeticalOrder)
            {
                resours = [.. resours.OrderBy(x => x)];
            }

            ReportList.ListOfReferences(doc, string.Join("\r\n", resours));
            ReportExtras.PageBreak(doc);
        }
    }

    public static void AppendixPart(WordprocessingDocument doc, ViewModelAppendix viewModelAppendix, bool on,
        DocumentType type)
    {
        if (on)
        {
            if (type != DocumentType.VKR)
            {
                char letter = 'А';
                foreach (IParagraphData appendix in viewModelAppendix.Paragraphs)
                {
                    string name = "«";
                    if (appendix != null)
                    {
                        name += appendix.Description;
                    }

                    name += "»";
                    TitleAppendix(doc, "ПРИЛОЖЕНИЕ " + letter + "\n(обязательное)\n" + name);
                    if (appendix is ParagraphPicture picture)
                    {
                        string id = ReportImage.Registration(doc, picture);
                        if (id != null && picture.Bitmap != null)
                        {
                            ReportImage.Create(doc, id, picture.Bitmap);
                        }
                    }
                    else if (appendix is ParagraphTable table)
                    {
                        ReportTable.Create(doc, table.TableData);
                    }
                    else if (appendix is ParagraphCode code)
                    {
                        string data;
                        if (appendix.Data.Length > 1)
                        {
                            data = appendix.Data.Remove(appendix.Data.Length - 2, 2);
                        }
                        else
                        {
                            data = appendix.Data;
                        }

                        ReportText.Text(doc, data, "Код");
                    }

                    ReportExtras.PageBreak(doc);
                    letter++;
                }
            }
            else
            {
                int number = 1;
                foreach (IParagraphData appendix in viewModelAppendix.Paragraphs)
                {
                    TitleAppendixVKR(doc, "Приложение " + number, appendix.Description);
                    if (appendix is ParagraphPicture picture)
                    {
                        string id = ReportImage.Registration(doc, picture);
                        if (id != null && picture.Bitmap != null)
                        {
                            ReportImage.Create(doc, id, picture.Bitmap);
                        }
                    }
                    else if (appendix is ParagraphTable table)
                    {
                        ReportTable.Create(doc, table.TableData);
                    }
                    else if (appendix is ParagraphCode code)
                    {
                        string data;
                        if (appendix.Data.Length > 1)
                        {
                            data = appendix.Data.Remove(appendix.Data.Length - 2, 2);
                        }
                        else
                        {
                            data = appendix.Data;
                        }

                        ReportText.Text(doc, data, "Код");
                    }

                    ReportExtras.PageBreak(doc);
                    number++;
                }
            }
        }
    }

    static void TitleAppendix(WordprocessingDocument doc, string text)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;

        Paragraph paragraph = body.AppendChild(new Paragraph());
        paragraph.ParagraphProperties = new(
            new ParagraphStyleId { Val = "РазделПриложение" });
        foreach (string line in text.Split('\n'))
        {
            string[] words = line.Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                ReportText.TextIntoParagraph(doc, words[i] + ' ', paragraph);
            }

            if (words.Length - 1 >= 0)
            {
                ReportText.TextIntoParagraph(doc, words[^1], paragraph);
            }

            ReportExtras.NewLine(paragraph);
        }
    }

    static void TitleAppendixVKR(WordprocessingDocument doc, string main, string description)
    {
        ReportText.Text(doc, main, "РазделПриложениеВКР");
        ReportExtras.EmptyLines(doc, 1);
        ReportText.Text(doc, "Листинг файла «" + description + "»", "РазделПриложениеВКРНазвание");
        ReportExtras.EmptyLines(doc, 1);
    }
}