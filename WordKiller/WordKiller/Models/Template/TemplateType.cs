﻿using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.ObjectModel;
using WordKiller.ViewModels;

namespace WordKiller.Models.Template
{
    [Serializable]
    public class TemplateType
    {
        public DataTypes.Enums.DocumentType Type { get; set; }

        public ObservableCollection<Template> Templates { get; set; }
        public TemplateType(DataTypes.Enums.DocumentType type)
        {
            Type = type;
            Templates = new()
            {
                new Template("Текст", justify: JustificationValues.Both, lineSpacing: 1.5f, firstLine: 1.25f),
                new Template("Раздел", justify: JustificationValues.Center, bold: true, after: 8, lineSpacing: 1.5f, firstLine: 1.5f),
                new Template("Подраздел", justify: JustificationValues.Center, bold: true, after: 8, lineSpacing: 1.5f, firstLine: 1.5f),
                new Template("Список", justify: JustificationValues.Both, lineSpacing: 1.5f, left: 1.25f),
                new Template("Картинка", justify: JustificationValues.Center, after: 8, lineSpacing: 1.5f),
                new Template("ТекстКТаблице", justify: JustificationValues.Both, before: 8, lineSpacing: 1.5f),
                new Template("Таблица", justify: JustificationValues.Both, after: 6, lineSpacing: 1f),
                new Template("Код", 12, JustificationValues.Left)
            };
        }

        public TemplateType()
        {
            Templates = new();
        }
    }
}