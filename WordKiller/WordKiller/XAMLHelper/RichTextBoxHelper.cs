using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace WordKiller.XAMLHelper;

public class RichTextBoxHelper : DependencyObject
{
    static readonly HashSet<Thread> _recursionProtection = [];

    public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
        "DocumentXaml",
        typeof(string),
        typeof(RichTextBoxHelper),
        new FrameworkPropertyMetadata(
            "",
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            (obj, e) =>
            {
                if (_recursionProtection.Contains(Thread.CurrentThread))
                    return;

                var richTextBox = (RichTextBox)obj;
                try
                {
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(richTextBox)));
                    var doc = (FlowDocument)XamlReader.Load(stream);

                    richTextBox.Document = doc;
                }
                catch (Exception)
                {
                    richTextBox.Document = new();
                }

                richTextBox.TextChanged += (obj2, e2) =>
                {
                    if (obj2 is RichTextBox richTextBox2)
                    {
                        SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox2.Document));
                    }
                };
            }
        )
    );

    public static string GetDocumentXaml(DependencyObject obj)
    {
        return (string)obj.GetValue(DocumentXamlProperty);
    }

    public static void SetDocumentXaml(DependencyObject obj, string value)
    {
        _recursionProtection.Add(Thread.CurrentThread);
        obj.SetValue(DocumentXamlProperty, value);
        _recursionProtection.Remove(Thread.CurrentThread);
    }
}