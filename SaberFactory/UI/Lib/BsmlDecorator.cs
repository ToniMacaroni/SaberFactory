using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class BsmlDecorator
    {
        private readonly Dictionary<string, string> _templates = new Dictionary<string, string>();

        private readonly Dictionary<string, Func<BsmlDecorator, string[], string>> _templateHandlers = new Dictionary<string, Func<BsmlDecorator, string[], string>>
        {
            {"color-template", (dec, args) => TryReplacingWithColor(args[0], out _) },
            {"template", (dec, args) => dec._templates.TryGetValue(args[0], out var template) ? template : "" },
            {"file", (dec, args) =>
            {
                var data = Readers.ReadResource(args[0]);
                var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
                return dec.Process(content);
            }}
        };

        public void AddTemplateHandler(string name, Func<BsmlDecorator, string[], string> action)
        {
            if (_templateHandlers.ContainsKey(name))
            {
                _templateHandlers[name] = action;
                return;
            }
            
            _templateHandlers.Add(name, action);
        }

        public void AddTemplate(string name, string template)
        {
            if (_templates.ContainsKey(name))
            {
                _templates[name] = template;
                return;
            }
            
            _templates.Add(name, template);
        }

        public async Task<BSMLParserParams> ParseFromResourceAsync(string resourceName, GameObject parent, object host)
        {
            var data = await Readers.ReadResourceAsync(resourceName);
            var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
            content = Process(content);
            return BSMLParser.instance.Parse(content, parent, host);
        }
        
        public BSMLParserParams ParseFromResource(string resourceName, GameObject parent, object host)
        {
            var data = Readers.ReadResource(resourceName);
            var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
            content = Process(content);
            return BSMLParser.instance.Parse(content, parent, host);
        }

        public string Process(string content)
        {
            var pos = 0;
            while (pos < content.Length)
            {
                if (content[pos] == '{')
                {
                    string charBuffer = "";
                    for (int j = pos+1; j < content.Length; j++)
                    {
                        if (content[j] == '}') break;
                        charBuffer += content[j];
                    }

                    content = content.Remove(pos, charBuffer.Length + 2);
                    var template = ProcessTemplate(charBuffer);
                    content = content.Insert(pos, template);
                    pos += template.Length;
                    continue;
                }
                pos++;
            }
            return content;
        }

        private string ProcessTemplate(string template)
        {
            var split = template.Split(';');
            if (_templateHandlers.TryGetValue(split[0], out var action))
            {
                return action(this, split.Skip(1).ToArray());
            }
            return "";
        }

        private static string TryReplacingWithColor(string input, out bool replaced)
        {
            replaced = false;
            if (input[0] != '$') return input;
            if (ThemeManager.GetDefinedColor(input.Substring(1), out var color))
            {
                replaced = true;
                return "#"+ColorUtility.ToHtmlStringRGBA(color);
            }

            replaced = true;
            return "#000";
        }
    }
}