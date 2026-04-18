using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Levels/New Level List")]
public class LevelList : ScriptableObject
{
    public string mainMenu;
    public List<string> listScene = new List<string>();
}
