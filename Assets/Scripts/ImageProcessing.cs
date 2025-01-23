using System.Diagnostics;
using System.IO;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스 추가

public class SiftFeatureMatching : MonoBehaviour
{
    public string pythonFilePath = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/ImageProcessing.py";
    public string image1Path = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/Ralo.jpg";
    public string image2Path = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/Ralo3.jpg";
    public string outputPath = "C:/GitHubRepo/satellite_simulator/Assets/PythonScripts/output.json";

    public TextMeshProUGUI matchPercentageText; // TextMeshPro 텍스트 연결

    void Start()
    {
        RunPythonScript();
    }

    void RunPythonScript()
    {
        // 경로 확인용 디버깅 로그 출력
        Debug.Log($"Python File Path: {pythonFilePath}");
        Debug.Log($"Image1 Path: {image1Path}");
        Debug.Log($"Image2 Path: {image2Path}");
        Debug.Log($"Output Path: {outputPath}");

        // Python 실행 설정
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "C:/Python313/python.exe", // Python 실행 파일 경로
            Arguments = $"\"{pythonFilePath}\" \"{image1Path}\" \"{image2Path}\" \"{outputPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Debug.Log("Python Output: " + result);
                }

                using (StreamReader errorReader = process.StandardError)
                {
                    string errors = errorReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errors))
                    {
                        Debug.LogError("Python Errors: " + errors);
                    }
                }
            }

            // Python 결과 JSON 파일 읽기
            if (File.Exists(outputPath))
            {
                string jsonContent = File.ReadAllText(outputPath);
                SiftOutput result = JsonUtility.FromJson<SiftOutput>(jsonContent);

                Debug.Log($"Keypoints in Image1: {result.keypoints1}");
                Debug.Log($"Keypoints in Image2: {result.keypoints2}");
                Debug.Log($"Good Matches: {result.good_matches}");
                Debug.Log($"Match Percentage: {result.match_percentage}%");

                // UI 텍스트 업데이트
                if (matchPercentageText != null)
                {
                    matchPercentageText.text = $"Match Percentage: {result.match_percentage:F2}%";
                }
            }
            else
            {
                Debug.LogError("Python result file not found: " + outputPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error while running Python script: " + e.Message);
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
