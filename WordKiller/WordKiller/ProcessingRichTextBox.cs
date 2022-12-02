using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace WordKiller
{
    internal class ProcessingRichTextBox
    {
        public static void StartForSubstitutionPanel(RichTextBox richTextBox, KeyEventArgs e)
        {
            int line = RTBox.GetLineOfCursor(richTextBox);
            string[] lines = RTBox.GetText(richTextBox).Split('\n');
            int index = RTBox.GetCaretIndex(richTextBox);
            if (new TextRange(richTextBox.CaretPosition.DocumentStart, richTextBox.CaretPosition.DocumentEnd).Text == richTextBox.Selection.Text && (e.Key == Key.Back || e.Key == Key.Delete))
            {
                lines[1] = string.Empty;
                lines[3] = string.Empty;
                RTBox.SetText(richTextBox, lines[0] + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3]);
                RTBox.SetCaret(richTextBox, lines[0].Length + 3);
                e.Handled = true;
            }
            else if ((line == 1 || line == 3 || (line == 2 && richTextBox.Selection.Text.Contains('\n'))) && !(e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right))
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && line == 2 || e.Key == Key.Delete && EndSecond(lines, index) ||
            (e.Key == Key.Back || Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X) &&
                    (BeginningSecond(lines, index) || BeginningFourth(lines, index)) && richTextBox.Selection.Text.Length == 0)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Down && (line == 2 || BeginningSecond(lines, index) || EndSecond(lines, index)))
            {
                RTBox.SetCaret(richTextBox, index + lines[1].Length + lines[2].Length + 4);
                e.Handled = true;
            }
            else if (e.Key == Key.Up && (line == 4 || BeginningFourth(lines, index)))
            {
                RTBox.SetCaret(richTextBox, index - lines[1].Length - lines[2].Length);
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                if (line == 2)
                {
                    if (richTextBox.Selection.Text.Contains('\n'))
                    {
                        e.Handled = true;
                    }
                    else if (Clipboard.GetText().Contains('\n'))
                    {
                        Clipboard.SetText(Clipboard.GetText().Replace("\r", "").Replace('\n', ' '));
                    }
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D)
            {
                if (string.IsNullOrEmpty(lines[1]) && !string.IsNullOrEmpty(lines[3]))
                {
                    lines[1] = lines[3];
                    if (lines.Length > 4)
                    {
                        for (int i = 4; i < lines.Length; i++)
                        {
                            lines[3] += "\n" + lines[i];
                        }
                    }
                    RTBox.SetText(richTextBox, lines[0] + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3]);
                }
                else if (string.IsNullOrEmpty(lines[3]))
                {
                    lines[3] = lines[1];
                    RTBox.SetText(richTextBox, lines[0] + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3]);
                }
                RTBox.SetCaret(richTextBox, lines[0].Length + lines[1].Length + lines[2].Length + lines[3].Length + 6);
            }
        }

        public static void StartForTextPanel(RichTextBox richTextBox, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                Clipboard.SetText(Clipboard.GetText());
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.A && !CheckPressKey(e.Key, Key.Delete, Key.Back, Key.Enter, Key.Up, Key.Down, Key.Left, Key.Right) && (RTBox.GetLineAtCursor(richTextBox).Contains(Config.specialBefore) || RTBox.GetLineAtCursor(richTextBox).Contains(Config.specialAfter)) && !(Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)) // probably this is better than something above that does the same for line 0 and 2
            {
                e.Handled = true;
            }
        }

        static bool BeginningSecond(string[] lines, int index)
        {
            if (lines[0].Length == index - 1)
            {
                return true;
            }
            return false;
        }

        static bool CheckPressKey(Key press, params Key[] keys)
        {
            foreach (Key key in keys)
            {
                if (press == key)
                {
                    return true;
                }
            }
            return false;
        }

        static bool BeginningFourth(string[] lines, int index)
        {
            if (lines[0].Length + lines[1].Length + lines[2].Length == index - 3)
            {
                return true;
            }
            return false;
        }

        static bool EndSecond(string[] lines, int index)
        {
            if (lines[1].Length + lines[0].Length == index - 1)
            {
                return true;
            }
            return false;
        }
    }
}
