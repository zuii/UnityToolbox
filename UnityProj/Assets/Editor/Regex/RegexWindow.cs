//================================================================
// author: harperzhang
// date:   2017.5.20
//================================================================
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace Toolbox
{
    public class RegexWindow : EditorWindow
    {
        string _testString;
        string _resultString;
        string _expression;

        RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        [MenuItem("Toolbox/Regex Tool")]
        public static void Open()
        {
            RegexWindow window = GetWindow<RegexWindow>();
            window.title = "Regex";
        }

        void OnEnable()
        {
            _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                // test string area
                OnTestStringArea();

                // matching result area
                OnResultArea();

                // options
                OnOptions();

                // regular expression field
                OnExpressionArea();
            }
            EditorGUILayout.EndVertical();
        }

        void OnTestStringArea()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("TEST STRING");
            string test = EditorGUILayout.TextArea(_testString, GUILayout.Height(100));
            if (!string.IsNullOrEmpty(test) && !test.Equals(_testString))
            {
                _testString = test;
                Do();
            }
            else if (string.IsNullOrEmpty(test))
            {
                _testString = "";
                _resultString = "";
            }
        }

        void OnResultArea()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("MATCH INFOMATION");
            EditorGUILayout.TextArea(_resultString, GUILayout.Height(100));
        }

        void OnOptions()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                // ignore case
                bool check = (_options & RegexOptions.IgnoreCase) > 0;
                bool toggle = EditorGUILayout.ToggleLeft("IGNORE CASE", check, GUILayout.Width(100));
                if (toggle != check)
                {
                    _options ^= RegexOptions.IgnoreCase;
                    Do();
                }

                // multi line
                check = (_options & RegexOptions.Multiline) > 0;
                toggle = EditorGUILayout.ToggleLeft("MULTI LINE", check, GUILayout.Width(100));
                if (toggle != check)
                {
                    _options ^= RegexOptions.Multiline;
                    Do();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnExpressionArea()
        {
            EditorGUILayout.Space();

            GUIStyle style = GUI.skin.GetStyle("LargeTextField");
            style.alignment = TextAnchor.MiddleLeft;
            string exp = EditorGUILayout.TextField(_expression, style, GUILayout.Height(30));
            if (!string.IsNullOrEmpty(exp) && !exp.Equals(_expression))
            {
                _expression = exp;
                Do();
            }
        }

        void Do()
        {
            if (string.IsNullOrEmpty(_testString)
                || string.IsNullOrEmpty(_expression))
            {
                return;
            }
            
            _resultString = "";
            MatchCollection matches = Regex.Matches(_testString, _expression, _options);
            foreach (Match mach in matches)
            {
                _resultString += mach.Value + "\n";
            }
        }
    }
}