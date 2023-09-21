using System.Text.RegularExpressions;

namespace MemorizerBot.Helpers;

internal static class MarkdownHelpers
{
    private static readonly Regex regex = new Regex(@"[\.\\*`_{}\[\}\(\)#\+\-!]");
    public static string ToMarkdown(string text)
    {
        return regex.Replace(text, match => "\\" + match.Value);
    }
}
