using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // UnityEngine.UI 추가
using TMPro; // TextMeshPro 사용 시 추가

public class SiftFeatureMatching : MonoBehaviour
{
    public string pythonFilePath = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/ImageProcessing.py";
    public string image1Path = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/Ralo.jpg";
    public string image2Path = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/Ralo5.png";
    public string outputPath = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/output.json";

    public TextMeshProUGUI matchPercentageText; // TextMeshPro 텍스트 연결
    // 또는 기본 UI 텍스트를 사용할 경우:
    // public Text matchPercentageText;

    void Start()
    {
        RunPythonScript();
    }

    void RunPythonScript()
    {
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "C:/Python313/python.exe", //파이썬 경로
            Arguments = $"{pythonFilePath} {image1Path} {image2Path} {outputPath}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                UnityEngine.Debug.Log("Python Output: " + result);
            }

            using (StreamReader errorReader = process.StandardError)
            {
                string errors = errorReader.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                {
                    UnityEngine.Debug.LogError("Python Errors: " + errors);
                }
            }
        }

        if (File.Exists(outputPath))
        {
            string jsonContent = File.ReadAllText(outputPath);
            SiftOutput result = JsonUtility.FromJson<SiftOutput>(jsonContent);

            UnityEngine.Debug.Log($"Keypoints in Image1: {result.keypoints1}");
            UnityEngine.Debug.Log($"Keypoints in Image2: {result.keypoints2}");
            UnityEngine.Debug.Log($"Good Matches: {result.good_matches}");
            UnityEngine.Debug.Log($"Match Percentage: {result.match_percentage}%");

            // UI 텍스트에 매칭 퍼센트 표시
            if (matchPercentageText != null)
            {
                matchPercentageText.text = $"Match Percentage: {result.match_percentage:F2}%";
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Python result file not found: " + outputPath);
        }
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
