/********************************************************************
	created:	2017/08/02
	author:		harperzhang
	purpose:	Find asset by MD5/GUID or other keywords.
*********************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class FindAsset : EditorWindow
{
    enum SearchType : int
    {
        MD5,
        GUID
    };

    string[] SearchTypeString = Enum.GetNames(typeof(SearchType));

    int _searchType;
    string _keyword;

    string[] _results;

    [MenuItem("Toolbox/Find Asset")]
    public static void HereYoure()
    {
        GetWindow<FindAsset>();
    }
    
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();

            // search bar
            OnDrawSearch();

            // result
            OnDrawResult();
        }
        EditorGUILayout.EndVertical();
    }

    void OnDrawSearch()
    {
        EditorGUILayout.BeginHorizontal();
        {
            // label
            EditorGUILayout.LabelField("Keyword：", GUILayout.Width(60));

            // input
            _keyword = EditorGUILayout.TextField(_keyword);

            // search type
            _searchType = EditorGUILayout.Popup(_searchType, SearchTypeString, GUILayout.Width(60));

            // button
            if (GUILayout.Button("Find", GUILayout.Width(80)))
            {
                OnFind();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void OnDrawResult()
    {
        if (_results == null || _results.Length == 0)
        {
            return;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Results：");

            // results
            foreach (string asset in _results)
            {
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(asset);
                if (obj != null)
                {
                    EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), true);
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    void OnFind()
    {
        if (string.IsNullOrEmpty(_keyword))
        {
            return;
        }

        SearchType type = (SearchType)_searchType;
        switch (type)
        {
            case SearchType.GUID:
                FindWithGUID(_keyword);
                break;
            case SearchType.MD5:
                FindWithMD5(_keyword);
                break;
        }
    }

    void FindWithMD5(string md5)
    {
        List<string> results = new List<string>();

        System.Security.Cryptography.MD5 md5Tool = System.Security.Cryptography.MD5.Create();

        string[] assets = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(f => !f.EndsWith(".meta")).ToArray();
        int total = assets.Length;
        int current = 0;
        foreach (string asset in assets)
        {
            FileStream file = new FileStream(asset, FileMode.Open);
            byte[] retVal = md5Tool.ComputeHash(file);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            string hash = sb.ToString();
            if (md5.Equals(hash, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(asset.Substring(Application.dataPath.Length - "Assets".Length).Replace("\\", "/"));
                break;
            }

            EditorUtility.DisplayProgressBar("Searching", string.Format("Checking：{0}", Path.GetFileName(asset)), (float)current++ / total);
        }

        EditorUtility.ClearProgressBar();

        _results = results.ToArray();
    }

    void FindWithGUID(string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!string.IsNullOrEmpty(path))
        {
            _results = new string[] { path };
        }
    }
}
