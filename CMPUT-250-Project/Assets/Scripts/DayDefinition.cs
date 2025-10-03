using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UserDefinition
{
    public string filename;

    [Header("Events")]
    public UnitGameEvent[] before;
    public BoolGameEvent[] after;
}

[CreateAssetMenu(menuName = "Day Definition")]
public class DayDefinition : ScriptableObject
{
    public string Directory;
    public string Date;
    public List<UserDefinition> Users;

    public DayDefinition(string DirectoryIn)
    {
        Directory = DirectoryIn;
        Users = new List<UserDefinition>();
        Date = "";
    }

    public DayDefinition(string DirectoryIn, string DateIn)
    {
        Directory = DirectoryIn;
        Users = new List<UserDefinition>();
        Date = DateIn;
    }

    public DayDefinition(string DirectoryIn, string DateIn, List<UserDefinition> UserIn)
    {
        Directory = DirectoryIn;
        Users = UserIn;
        Date = DateIn;
    }
}
