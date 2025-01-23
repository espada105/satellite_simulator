using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;  // UnityEngine.Debug를 Debug로 명시적 사용
using UnityEngine;

public class PythonPathFinder : MonoBehaviour
{
    public string path;

    void Start( )
    {
        path = GetPythonPath( );
        if( path == "X" )
            Debug.Log( "Python doesnt exist." );
    }

    public static string GetPythonPath()
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c where python";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                return output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else
            {
                return "X";
            }
        }
        catch (Exception ex)
        {
            return "X";
        }
    }
}