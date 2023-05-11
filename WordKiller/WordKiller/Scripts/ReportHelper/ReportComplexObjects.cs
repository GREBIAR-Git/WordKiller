using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Models;
using WordKiller.ViewModels;

namespace WordKiller.Scripts.ReportHelper
{
    public static class ReportComplexObjects
    {
        public static void TitlePart(WordprocessingDocument doc, DataTypes.Enums.DocumentType typeDocument, ViewModelTitle title)
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
                Ministry(doc, title.Cathedra);
                switch (typeDocument)
                {
                    case DataTypes.Enums.DocumentType.LaboratoryWork:
                        LabPra(doc, "лабораторной", title);
                        break;
                    case DataTypes.Enums.DocumentType.PracticeWork:
                        LabPra(doc, "практической", title);
                        break;
                    case DataTypes.Enums.DocumentType.Coursework:
                        Coursework(doc, title);
                        break;
                    case DataTypes.Enums.DocumentType.ControlWork:
                        ControlWork(doc, title);
                        break;
                    case DataTypes.Enums.DocumentType.Referat:
                        Referat(doc, title);
                        break;
                    case DataTypes.Enums.DocumentType.VKR:
                        break;
                }
                Orel(doc);
            }
            ReportExtras.SectionBreak(doc);
        }

        static void LabPra(WordprocessingDocument doc, string type, ViewModelTitle title)
        {
            string text = "ОТЧЁТ";
            ReportText.Text(doc, text, 16, JustificationValues.Center, true);
            text = "По " + type + " работе №" + title.Number;
            ReportText.Text(doc, text, 16, JustificationValues.Center, after: 10);
            text = "на тему: «" + title.Theme + "»";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "по дисциплине: «" + title.Discipline + "»";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
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
            text = Properties.Settings.Default.Faculty;
            ReportText.Text(doc, text);
            text = "Направление: " + Properties.Settings.Default.Direction;
            ReportText.Text(doc, text);
            text = "Группа: " + Properties.Settings.Default.Group;
            ReportText.Text(doc, text);

            text = "Проверил: " + title.Professor;
            ReportText.Text(doc, text, after: 10);
            ReportExtras.EmptyLines(doc, 1);

            text = "Отметка о зачёте: ";
            ReportText.Text(doc, text);

            text = "Дата: «____» __________ " + SpaceForYear(Properties.Settings.Default.Year) + "г.";
            ReportText.Text(doc, text, justify: JustificationValues.Right);

            ReportExtras.EmptyLines(doc, 8);
        }

        static void Coursework(WordprocessingDocument doc, ViewModelTitle title)
        {
            string text = "Работа допущена к защите";
            ReportText.Text(doc, text, multiplier: 1.5f, left: 9.5f);
            text = "______________Руководитель";
            ReportText.Text(doc, text, multiplier: 1.5f, left: 9.5f);
            text = "«____»_____________" + SpaceForYear(Properties.Settings.Default.Year) + "г.";
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

            ReportText.Text(doc, text, justify: JustificationValues.Center, bold: true);

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
            text = Properties.Settings.Default.Faculty;
            ReportText.Text(doc, text, multiplier: 1.5f);
            text = "Направление: " + Properties.Settings.Default.Direction;
            ReportText.Text(doc, text, multiplier: 1.5f);
            text = "Группа: " + Properties.Settings.Default.Group;
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
            ReportText.Text(doc, text, 16, JustificationValues.Center, true);

            text = "по дисциплине: «" + title.Discipline + "»";
            ReportText.Text(doc, text, justify: JustificationValues.Center);

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
            text = Properties.Settings.Default.Faculty;
            ReportText.Text(doc, text);
            text = "Направление: " + Properties.Settings.Default.Direction;
            ReportText.Text(doc, text);
            text = "Группа: " + Properties.Settings.Default.Group;
            ReportText.Text(doc, text);

            text = "Проверил: " + title.Professor;
            ReportText.Text(doc, text, after: 10);

            ReportExtras.EmptyLines(doc, 1);

            text = "Отметка о зачёте: ";
            ReportText.Text(doc, text);

            text = "Дата: «____» __________ " + SpaceForYear(Properties.Settings.Default.Year) + "г.";
            ReportText.Text(doc, text, justify: JustificationValues.Right);

            ReportExtras.EmptyLines(doc, 9);
        }

        static void Referat(WordprocessingDocument doc, ViewModelTitle title)
        {
            ReportExtras.EmptyLines(doc, 1);

            string text = "Реферат по дисциплине: «" + title.Discipline + "»";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "Тема: «" + title.Theme + "»";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            ReportExtras.EmptyLines(doc, 16);

            if (title.OnePerformed())
            {
                text = "Выполнил: студент группы ";
            }
            else
            {
                text = "Выполнили: студенты группы ";
            }
            text += Properties.Settings.Default.Group;
            ReportText.Text(doc, text, justify: JustificationValues.Right);
            text = title.AllPerformed();
            ReportText.Text(doc, text, justify: JustificationValues.Right);
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
            ReportText.Text(doc, text, justify: JustificationValues.Right);
            text = title.Professor;
            ReportText.Text(doc, text, justify: JustificationValues.Right);

            ReportExtras.EmptyLines(doc, 6);
        }

        static void Ministry(WordprocessingDocument doc, string faculty)
        {
            string text = "МИНИСТЕРСТВО НАУКИ И ВЫСШЕГО ОБРАЗОВАНИЯ";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "РОССИЙСКОЙ ФЕДЕРАЦИИ";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "ФЕДЕРАЛЬНОЕ ГОСУДАРСТВЕННОЕ БЮДЖЕТНОЕ";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "ОБРАЗОВАТЕЛЬНОЕ УЧРЕЖДЕНИЕ ВЫСШЕГО ОБРАЗОВАНИЯ";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "«ОРЛОВСКИЙ ГОСУДАРСТВЕННЫЙ УНИВЕРСИТЕТ";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            text = "ИМЕНИ И.С.ТУРГЕНЕВА»";
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            ReportExtras.EmptyLines(doc, 2);

            text = faculty;
            ReportText.Text(doc, text, justify: JustificationValues.Center);
            ReportExtras.EmptyLines(doc, 3);
        }

        static void Orel(WordprocessingDocument doc)
        {
            string text = "Орел, " + SpaceForYear(Properties.Settings.Default.Year);
            ReportText.Text(doc, text, justify: JustificationValues.Center);
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
                text = "«___»_____________" + Properties.Settings.Default.Year + "г.";
                ReportText.Text(doc, text, left: 9.5f);
                ReportExtras.EmptyLines(doc, 2);
                text = "ЗАДАНИЕ";
                ReportText.Text(doc, text, 16, JustificationValues.Center, true);
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
                ReportText.Text(doc, text, 14, JustificationValues.Center, true, multiplier: 2);
                text = "по дисциплине «" + title.Discipline + "»";
                ReportText.Text(doc, text, multiplier: 2);
                ReportExtras.EmptyLines(doc, 1);
                User performed = title.FirstPerformed();
                text = "Студент    " + performed.Full + "                Шифр " + performed.Shifr;
                ReportText.Text(doc, text, multiplier: 1.5f);
                text = Properties.Settings.Default.Faculty;
                ReportText.Text(doc, text, multiplier: 1.5f);
                text = "Направление подготовки " + Properties.Settings.Default.Direction;
                ReportText.Text(doc, text, multiplier: 1.5f);
                text = "Группа " + Properties.Settings.Default.Group;
                ReportText.Text(doc, text, multiplier: 1.5f);
                ReportExtras.EmptyLines(doc, 1);
                text = "1 Тема " + type;
                ReportText.Text(doc, text, multiplier: 1.5f);
                text = title.Theme;
                ReportText.Text(doc, text, multiplier: 1.5f);
                ReportExtras.EmptyLines(doc, 1);
                text = "2 Срок сдачи студентом законченной работы «___» _____________ " + Properties.Settings.Default.Year;
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

                text = "Задание принял к исполнению: «___» _____________ " + Properties.Settings.Default.Year;
                ReportText.Text(doc, text, multiplier: 1.5f);
                ReportExtras.EmptyLines(doc, 1);

                text = "Подпись студента__________________ ";
                ReportText.Text(doc, text, multiplier: 1.5f);
                ReportExtras.EmptyLines(doc, 1);
                ReportExtras.SectionBreak(doc);

            }
        }

        public static void ListOfReferencesPart(WordprocessingDocument doc, ViewModelListOfReferences listOfReferences, bool on)
        {
            if (on)
            {
                if (listOfReferences.ListSourcesUsed)
                {
                    ReportText.Text(doc, "Список использованных источников", "Раздел");
                }
                else if (listOfReferences.Bibliography)
                {
                    ReportText.Text(doc, "Список литературы", "Раздел");
                }
                List<string> resours = new();
                foreach (Book book in listOfReferences.Books)
                {
                    resours.Add(book.Autors + " " + book.Name + ". " + book.Publication + ", " + book.Year + ". " + book.Page + " с.");
                }
                foreach (ElectronicResource electronicResource in listOfReferences.ElectronicResources)
                {
                    resours.Add(electronicResource.Name + " [Электронный ресурс]. URL: " + electronicResource.Url + " (дата обращения: " + electronicResource.CirculationDate + ").");
                }
                resours = resours.OrderBy(x => x).ToList();
                ReportList.ListOfReferences(doc, string.Join("\r\n", resours));
                ReportExtras.PageBreak(doc);
            }
        }

        public static void AppendixPart(WordprocessingDocument doc, ViewModelAppendix viewModelAppendix, bool on)
        {
            if (on)
            {
                char letter = 'А';
                foreach (IParagraphData appendix in viewModelAppendix.Appendix)
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
                        //Text(doc, "Рисунок " + p + " – " + picture.Description, "Картинка");
                    }
                    else if (appendix is ParagraphTable table)
                    {
                        //Text(doc, "Таблица " + t + " – " + table.Description, "ТекстКТаблице");
                        ReportTable.Create(doc, table.TableData);
                    }
                    else if (appendix is ParagraphCode code)
                    {
                        ReportText.Text(doc, code.Data, "Код");
                    }
                    ReportExtras.PageBreak(doc);
                    letter++;
                }
            }
        }

        static void TitleAppendix(WordprocessingDocument doc, string text)
        {
            MainDocumentPart mainPart = doc.MainDocumentPart;
            Body body = mainPart.Document.Body;

            Paragraph paragraph = body.AppendChild(new Paragraph());
            paragraph.ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId() { Val = "РазделПриложение" });
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
    }
}
