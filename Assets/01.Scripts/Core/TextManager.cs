using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager
{
    public static TextManager Instance;

    private string _currentLocale = "kr";
    private string _fallbackLocale = "eng";
    private Dictionary<string, Dictionary<string, string>> _localeDictionary = new Dictionary<string, Dictionary<string, string>>();

    //나중에 여기에서 파일로부터 읽어서 저장하거나, 웹에서 읽어서 저장하는 방식으로 변경한다.
    public void Init()
    {
        //invincible
        Dictionary<string, string> krDictionary = new Dictionary<string, string>();
        krDictionary.Add("invincible", "무적");
        _localeDictionary.Add("kr", krDictionary);

        Dictionary<string, string> engDictionary = new Dictionary<string, string>();
        engDictionary.Add("invincible", "Invincible");
        _localeDictionary.Add("eng", engDictionary);
    }  

    public string LocaleString(string key)
    {
        if(_localeDictionary[_currentLocale].ContainsKey(key)) {
            return _localeDictionary[_currentLocale][key];
        }
        //없으면 영어로 리턴
        return _localeDictionary[_fallbackLocale][key]; 
    }
}
