using System.Collections; // IEnumerator를 사용하기 위한 네임스페이스
using System.Diagnostics;
using System.IO;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;  // UnityEngine.Debug를 Debug로 명시적 사용

public class SiftFeatureMatching : MonoBehaviour
{
    public string pythonFilePath;
    public string image1Path;
    public string image2Path;
    public string outputPath;

    public TextMeshProUGUI matchPercentageText;

    void Start()
    {
        // Python 스크립트를 먼저 실행하고 결과를 처리
        StartCoroutine(RunPythonAndUpdateUI());
    }

    IEnumerator RunPythonAndUpdateUI()
    {
        // Python 실행
        yield return StartCoroutine(RunPythonScript());

        // JSON 읽기 및 UI 업데이트
        if (File.Exists(Path.GetFullPath(outputPath)))
        {
            string jsonContent = File.ReadAllText(Path.GetFullPath(outputPath));
            SiftOutput result = JsonUtility.FromJson<SiftOutput>(jsonContent);

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

    IEnumerator RunPythonScript()
    {
        string fullPythonFilePath = Path.GetFullPath(pythonFilePath);
        string fullImage1Path = Path.GetFullPath(image1Path);
        string fullImage2Path = Path.GetFullPath(image2Path);
        string fullOutputPath = Path.GetFullPath(outputPath);

        string pythonPath = PythonPathFinder.GetPythonPath( );
        Debug.Log( "Current Python Path : " + pythonPath );

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"{fullPythonFilePath} {fullImage1Path} {fullImage2Path} {fullOutputPath}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        bool isCompleted = false;

        try
        {
            using (Process process = Process.Start(start))
            {
                process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log("Python Output: " + args.Data);
                process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError("Python Errors: " + args.Data);

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
                isCompleted = true;
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to run Python script: " + ex.Message);
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
