using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class GMSManager : MonoBehaviour
{
    public static GMSManager instance;

    public string apiKey;
    public float latitude = 37.3641f;        // 위도
    public float longitude = 127.0020f;     // 경도
    public int zoom = 16;                    // 줌 레벨 (1~21)
    public int width = 800;                  // 이미지 너비 (px)
    public int height = 600;                 // 이미지 높이 (px)

    public TMP_Text progressText;                // 진행 상황을 표시할 UI 텍스트
    public Slider progressSlider;            // 진행 상황을 표시할 Slider
    public float artificialDelay = 2.0f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        apiKey = Resources.Load<TextAsset>("KEY").text;
    }

    public void CaptureImage(Image target)
    {
        StartCoroutine(GetGoogleImage(target));
    }

    IEnumerator GetGoogleImage(Image target)
    {
        string url = $"https://maps.googleapis.com/maps/api/staticmap?center={latitude},{longitude}&zoom={zoom}&size={width}x{height}&maptype=satellite&key={apiKey}";

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            www.SendWebRequest();

            float simulatedProgress = 0f;
            float elapsedTime = 0f;

            while (!www.isDone || simulatedProgress < 1f)
            {
                if (www.isDone)
                {
                    elapsedTime += Time.deltaTime;
                    simulatedProgress = Mathf.Min(1f, elapsedTime / artificialDelay);
                }
                else
                {
                    simulatedProgress = Mathf.Min(simulatedProgress + Time.deltaTime / artificialDelay, www.downloadProgress);
                }

                progressSlider.value = simulatedProgress;
                progressText.text = $"Processing... {simulatedProgress * 100f:F2}%";
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D mapTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                target.sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f));
                progressSlider.value = 1f;
                progressText.text = "Download Complete!";
            }
            else
            {
                progressSlider.value = 0f;
                progressText.text = "Download Failed!";
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}
