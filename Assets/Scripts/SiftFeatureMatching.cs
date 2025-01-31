using System.Collections; // IEnumerator를 사용하기 위한 네임스페이스
using System.Diagnostics;
using System.IO;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;  // UnityEngine.Debug를 Debug로 명시적 사용
using UnityEngine.UI;

public class SiftFeatureMatching : MonoBehaviour
{
    public string pythonFilePath;
    public string image1Path;
    public string image2Path;
    public string outputPath;

    public TextMeshProUGUI matchPercentageText;

    public static SiftFeatureMatching instance;

    void Awake()
    {
        instance = this;
    }

    public void StartPython()
    {
        StartCoroutine(RunPythonAndUpdateUI());
    }

    private IEnumerator RunPythonAndUpdateUI()
    {
        yield return StartCoroutine(RunPythonScript());

        if (File.Exists(Path.GetFullPath(outputPath)))
        {
            string jsonContent = File.ReadAllText(Path.GetFullPath(outputPath));
            SiftOutput result = JsonUtility.FromJson<SiftOutput>(jsonContent);

            UIManager.instance.SetResult(result.match_percentage);

            UnityEngine.Debug.Log($"Keypoints in Image1: {result.keypoints1}");
            UnityEngine.Debug.Log($"Keypoints in Image2: {result.keypoints2}");
            UnityEngine.Debug.Log($"Good Matches: {result.good_matches}");
            UnityEngine.Debug.Log($"Match Percentage: {result.match_percentage}%");

            if (matchPercentageText != null)
            {
                matchPercentageText.text = $"Match Percentage: {result.match_percentage:F2}%";
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Python result file not found: " + Path.GetFullPath(outputPath));
        }
    }

    private IEnumerator RunPythonScript()
    {
        string pythonPath = PythonPathFinder.GetPythonPath( );
        
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        bool isCompleted = false;
        Process process = null;

        try
        {
            process = Process.Start(start);
            process.OutputDataReceived += (sender, args) => 
            {
                if (args.Data != null)
                {
                    UnityEngine.Debug.Log("Python Output: " + args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) => 
                UnityEngine.Debug.LogError("Python Errors: " + args.Data);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to run Python script: " + ex.Message);
        }

        while (process != null && !process.HasExited)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (process != null)
        {
            process.WaitForExit();
            isCompleted = true;
        }

        if (isCompleted)
        {
            UnityEngine.Debug.Log("Python script executed successfully.");
        }
        else
        {
            UnityEngine.Debug.LogError("Python script execution failed.");
        }

        yield return null;
    }

    [System.Serializable]
    public class SiftOutput
    {
        public int keypoints1;
        public int keypoints2;
        public int good_matches;
        public float match_percentage;
    }
}
