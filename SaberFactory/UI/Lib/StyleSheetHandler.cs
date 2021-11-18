using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BeatSaberMarkupLanguage;
using SaberFactory.Helpers;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class StyleSheetHandler
    {
        private readonly Dictionary<string, StyleSelector> _selectors = new Dictionary<string, StyleSelector>();

        private readonly Regex SelectorRegex = new Regex(@"(\S+?)\s*{([\s\S]*?)}");
        private readonly Regex RuleRegex = new Regex(@"\s*(\S*?): *([\s\S]*)");
        
        public void LoadStyleSheet(string resourceName)
        {
            var content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), resourceName).Replace("\r\n", "\n");
            LoadStyle(content);
        }
        
        public void LoadStyle(string content)
        {
            var matches = SelectorRegex.Matches(content);
            foreach (Match match in matches)
            {
                var selector = new StyleSelector(match.Groups[1].Value);
                
                foreach (var rule in match.Groups[2].Value.Replace("\n", "").Split(';'))
                {
                    if (rule.Length < 1)
                    {
                        continue;
                    }
                    
                    var ruleMatches = RuleRegex.Match(rule);
                    var ruleName = ruleMatches.Groups[1].Value;
                    var ruleValue = ruleMatches.Groups[2].Value;
                    
                    selector.AddRule(new StyleRule(ruleName, ruleValue));
                }

                if (_selectors.ContainsKey(selector.Name))
                {
                    _selectors[selector.Name] = selector;
                }
                else
                {
                    _selectors.Add(selector.Name, selector);
                }
            }
        }

        public IEnumerable<StyleSelector> GetAllSelectors()
        {
            return _selectors.Values;
        }

        public bool GetSelector(string name, out StyleSelector selector)
        {
            return _selectors.TryGetValue(name, out selector);
        }

        public List<StyleRule> CollectRules(params string[] selectors)
        {
            var output = new Dictionary<string, StyleRule>();
            foreach (var selectorName in selectors)
            {
                if (_selectors.TryGetValue(selectorName, out var selector))
                {
                    foreach (var rule in selector.GetRules())
                    {
                        if (output.ContainsKey(rule.Name))
                        {
                            output[rule.Name] = rule;
                        }
                        else
                        {
                            output.Add(rule.Name, rule);
                        }
                    }
                }
            }

            return output.Values.ToList();
        }

        internal class StyleRule
        {
            public readonly string Name;
            public readonly string Value;

            public StyleRule(string name, string value)
            {
                Name = name;

                Value = value = value.Replace("\"", "");

                if (value.Length > 1 && value[0] == '-' && value[1] == '-')
                {
                    ThemeManager.GetDefinedColor(value.Substring(2), out var color);
                    Value = "#" + ColorUtility.ToHtmlStringRGBA(color);
                }
            }
        }
        
        internal class StyleSelector
        {
            public readonly string Name;
            
            private readonly Dictionary<string, StyleRule> _rules = new Dictionary<string, StyleRule>();

            public StyleSelector(string name)
            {
                Name = name;
            }

            public void AddRule(StyleRule rule)
            {
                if (_rules.ContainsKey(rule.Name))
                {
                    _rules[rule.Name] = rule;
                    return;
                }
                
                _rules.Add(rule.Name, rule);
            }

            public void RemoveRule(string rule)
            {
                _rules.Remove(rule);
            }

            public IEnumerable<StyleRule> GetRules()
            {
                return _rules.Values;
            }
        }
    }
}