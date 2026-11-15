using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace LMUChronoCoach.Core
{
    /// <summary>Loss-minimizing text wrapper. Unknown lines are preserved; no edits are applied.</summary>
    public sealed class SetupFileDocument
    {
        private readonly List<string> _lines;
        public string OriginalText { get; private set; }
        private SetupFileDocument(string text)
        {
            OriginalText = text ?? string.Empty;
            _lines = OriginalText.Replace("\r\n", "\n").Split('\n').ToList();
        }
        public static SetupFileDocument Parse(string text) { return new SetupFileDocument(text); }
        public static SetupFileDocument Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Setup path is required.", nameof(path));
            return Parse(File.ReadAllText(path));
        }
        public string ToText() { return string.Join(Environment.NewLine, _lines); }
        public bool TryGetValue(string key, out string value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(key)) return false;
            foreach (var line in _lines)
            {
                var separator = line.IndexOf('=');
                if (separator <= 0) continue;
                if (string.Equals(line.Substring(0, separator).Trim(), key.Trim(), StringComparison.OrdinalIgnoreCase))
                { value = line.Substring(separator + 1).Trim(); return true; }
            }
            return false;
        }
    }
}