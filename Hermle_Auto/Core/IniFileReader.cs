using System;
using System.Collections.Generic;
using System.IO;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;
namespace Hermle_Auto.Core
{
    public class IniFileReader
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data = new();

        public string FilePath { get; }


        private readonly List<string> _annotation = new();

        public IniFileReader(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified INI file was not found.", filePath);

            FilePath = filePath;

            ParseFile(filePath);
        }

        private void ParseFile(string filePath)
        {
            string currentSection = string.Empty;

            foreach (var line in File.ReadLines(filePath))
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                {
                    if(trimmedLine.StartsWith(";"))
                    {
                        _annotation.Add(trimmedLine);
                    }
                    continue; // Skip empty lines and comments
                }
                    

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    // Parse section header
                    currentSection = trimmedLine.Trim('[', ']');
                    if (!_data.ContainsKey(currentSection))
                        _data[currentSection] = new Dictionary<string, string>();
                }
                else if (trimmedLine.Contains("="))
                {
                    // Parse key-value pair
                    var keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    if (currentSection != string.Empty)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();
                        _data[currentSection][key] = value;
                    }
                }
            }
        }

        public bool TryGetValue(string section, string key, out string value)
        {
            value = null;
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
            {
                value = _data[section][key];
                return true;
            }

            return false;
        }

        public string TryGetValue(string section, string key)
        {
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
            {
                return _data[section][key];
            }

            return "";
        }

        // 특정 키의 값 설정 및 덮어쓰기
        public void SetValue(string section, string key, string value)
        {
            if (!_data.ContainsKey(section))
            {
                _data[section] = new Dictionary<string, string>();
            }

            _data[section][key] = value;
        }

        // INI 파일 다시 쓰기
        public void Save()
        {
            using (var writer = new StreamWriter(FilePath))
            {
                foreach (var section in _data)
                {
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var kvp in section.Value)
                    {
                        writer.WriteLine($"{kvp.Key}={kvp.Value}");
                    }
                    writer.WriteLine(); // Add a blank line between sections
                }

                foreach (var line in _annotation)
                {
                    writer.WriteLine($"{line}");
                }

            }
        }

        // INI 파일 내용 출력
        public void PrintToConsole()
        {
            foreach (var section in _data)
            {
                Console.WriteLine($"[{section.Key}]");
                foreach (var kvp in section.Value)
                {
                    Console.WriteLine($"{kvp.Key}={kvp.Value}");
                }
                Console.WriteLine(); // Add a blank line between sections

            }
            foreach (var line in _annotation)
            {
                Console.WriteLine($"{line}");
            }
        }

        public IEnumerable<string> GetSections() => _data.Keys;

        public IEnumerable<string> GetKeys(string section) =>
            _data.ContainsKey(section) ? _data[section].Keys : Array.Empty<string>();

    }
}