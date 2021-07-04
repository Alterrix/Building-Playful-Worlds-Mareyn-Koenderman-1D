using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

using UnityEngine;

public class IniFile
{
    //path is where file is saved
    private string Path;
    private string fileName;

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    public IniFile(string _fileName)
    {
        //make new path and get folder path from local low
        string newPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low";
        //create directory off defaultCompany
        Directory.CreateDirectory(newPath + "\\" + Application.companyName);
        //add newpath 
        newPath = newPath + "\\" + Application.companyName;
        //add save file to the same company folder, but not the same product folder.
        Directory.CreateDirectory(newPath + "\\" + Application.productName);
        newPath = newPath + "\\" + Application.productName + "\\";

        fileName = _fileName;
        
        Path = newPath + _fileName + ".ini";
    }

    public string Read(string Key, string DefaultValue, string Section = null)
    {
        var RetVal = new StringBuilder(255);
        GetPrivateProfileString(Section ?? fileName, Key, "", RetVal, 255, Path);
        // if the read returns nothing 
        if (RetVal.Length == 0)
        {
            // create a key with the default value
            WritePrivateProfileString(Section ?? fileName, Key, DefaultValue, Path);
            //and read that one instead
            GetPrivateProfileString(Section ?? fileName, Key, "", RetVal, 255, Path);
        }

        return RetVal.ToString();
    }

    public void Write(string Key, string Value, string Section = null)
    {
        WritePrivateProfileString(Section ?? fileName, Key, Value, Path);
    }

    public void DeleteKey(string Key, string DefaultValue, string Section = null)
    {
        //overwrite the current value with the default value
        WritePrivateProfileString(Section ?? fileName, Key, DefaultValue, Path);
    }

    public void DeleteSection(string Section = null)
    {
        Write(null, null, Section ?? fileName);
    }

    public bool KeyExists(string Key, string Section = null)
    {
        return Read(Key, Section).Length > 0;
    }
}
