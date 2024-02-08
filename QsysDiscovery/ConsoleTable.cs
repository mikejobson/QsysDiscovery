using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QsysDiscovery
{
    public class ConsoleTable
    {
        private readonly int[] _columnWidths;
        private readonly List<string> _headers;
        private readonly List<IEnumerable<string>> _rows = new List<IEnumerable<string>>();

        public ConsoleTable(params string[] headers)
        {
            _headers = new List<string>(headers);
            _columnWidths = new int[headers.Count()];

            var col = 0;
            foreach (var header in _headers)
            {
                _columnWidths[col] = header.Length;
                col++;
            }
        }

        public int TotalWidth
        {
            get { return _columnWidths.Aggregate(1, (current, width) => current + width + 3); }
        }

        public int Count => _rows.Count;

        public void AddRow(params object[] items)
        {
            var values = new List<string>();
            var col = 0;
            foreach (var item in items)
            {
                if (item == null)
                {
                    values.Add(string.Empty);
                    col++;
                    continue;
                }

                var s = item.ToString();
                values.Add(s);
                var replaced = Regex.Replace(s, @"\u001b.*?m", string.Empty);
                if (replaced.Length > _columnWidths[col])
                    _columnWidths[col] = replaced.Length;
                col++;
            }

            _rows.Add(values);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool useColor)
        {
            var sb = new StringBuilder();

            var colTag1 = useColor ? AnsiColors.Yellow : string.Empty;
            var colTag2 = useColor ? AnsiColors.Green : string.Empty;
            var colTagClose = useColor ? AnsiColors.Reset : string.Empty;

            sb.Append('|');
            var divLine = "|";

            var col = 0;
            foreach (var header in _headers)
            {
                var s = colTag1 + header.PadRight(_columnWidths[col]) + colTagClose;
                sb.Append(" " + s + " |");
                var dashes = string.Empty;
                for (var i = 0; i < _columnWidths[col]; i++) dashes = dashes + "-";

                divLine = divLine + " " + dashes + " |";
                col++;
            }

            sb.AppendLine();

            sb.AppendLine(divLine);

            foreach (var row in _rows)
            {
                col = 0;
                var items = row as string[] ?? row.ToArray();
                sb.Append('|');
                foreach (var item in items)
                {
                    var s = item.PadRight(_columnWidths[col]);
                    if (col == 0) s = colTag2 + s + colTagClose;

                    sb.Append(" " + s + " |");
                    col++;
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public static class AnsiColors
    {
        public const string Black = "\u001b[30m";
        public const string Red = "\u001b[31m";
        public const string Green = "\u001b[32m";
        public const string Yellow = "\u001b[33m";
        public const string Blue = "\u001b[34m";
        public const string Magenta = "\u001b[35m";
        public const string Cyan = "\u001b[36m";
        public const string White = "\u001b[37m";

        public const string BrightBlack = "\u001b[30;1m";
        public const string BrightRed = "\u001b[31;1m";
        public const string BrightGreen = "\u001b[32;1m";
        public const string BrightYellow = "\u001b[33;1m";
        public const string BrightBlue = "\u001b[34;1m";
        public const string BrightMagenta = "\u001b[35;1m";
        public const string BrightCyan = "\u001b[36;1m";
        public const string BrightWhite = "\u001b[37;1m";

        public const string BackgroundBlack = "\u001b[40m";
        public const string BackgroundRed = "\u001b[41m";
        public const string BackgroundGreen = "\u001b[42m";
        public const string BackgroundYellow = "\u001b[43m";
        public const string BackgroundBlue = "\u001b[44m";
        public const string BackgroundMagenta = "\u001b[45m";
        public const string BackgroundCyan = "\u001b[46m";
        public const string BackgroundWhite = "\u001b[47m";

        public static string BackgroundBrightBlack = "\u001b[40;1m";
        public static string BackgroundBrightRed = "\u001b[41;1m";
        public static string BackgroundBrightGreen = "\u001b[42;1m";
        public static string BackgroundBrightYellow = "\u001b[43;1m";
        public static string BackgroundBrightBlue = "\u001b[44;1m";
        public static string BackgroundBrightMagenta = "\u001b[45;1m";
        public static string BackgroundBrightCyan = "\u001b[46;1m";
        public static string BackgroundBrightWhite = "\u001b[47;1m";

        public static string Bold = "\u001b[1m";
        public static string Reversed = "\u001b[7m";
        public static string Underline = "\u001b[4m";

        public static string Reset = "\u001b[0m";
    }
}