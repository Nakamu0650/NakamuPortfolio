using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TrackRecord", menuName = "Hanadayori/TrackRecord")]
public class G_TrackRecord : ScriptableObject
{
    public string trackName;
    public LanguageValue<string> title;
    public LanguageValue<string> content;
    public Sprite sprite;
    public G_TrackRecord(string _name, LanguageValue<string> _title, LanguageValue<string> _content, Sprite _sprite)
    {
        title = _title;
        trackName = _name;
        content = _content;
        sprite = _sprite;
    }

    private void OnValidate()
    {
        title.SetAll();
        content.SetAll();
    }
}
