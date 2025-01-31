using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject mainCanvas;
    public GameObject loadBar;
    public TMP_Text terminalText;
    public GameObject simulatorWindow;

    public GameObject camScreen;
    public GameObject radarScreen;
    public TMP_Text modeText;

    private bool isCamMode = true;

    public GameObject radarPrefab;
    public GameObject[] radarInstances;
    public Transform radarSprite;
    public TMP_Text radarText;

    public Image captureResultImage;
    public GameObject captureLoadPanel;

    private Queue<GameObject> radarPool = new Queue<GameObject>();
    private const int INITIAL_POOL_SIZE = 10;

    public TMP_Text coordinateText;

    public TMP_Text zoomText;
    public Slider zoomSlider;

    public Image uploadedImage;
    public GameObject loadingPanel;

    public GameObject resultPanel;
    public Image resultImage;
    public TMP_Text resultText;

    private Vector2 targetCoord;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeRadarPool();
        // StartAnimation();
        StartCoroutine(UpdateRadar());
    }

    private void InitializeRadarPool()
    {
        for (int i = 0; i < INITIAL_POOL_SIZE; i++)
        {
            GameObject radar = Instantiate(radarPrefab, radarSprite);
            radar.SetActive(false);
            radarPool.Enqueue(radar);
        }
    }

    private void StartAnimation( )
    {
        DOVirtual.DelayedCall( 7f, ( ) => {
            mainCanvas.SetActive( true );
            DOVirtual.DelayedCall( 6f, ( ) => {
                FadeOutObject( loadBar );
                DOVirtual.DelayedCall( 3f, ( ) => {
                    Sequence textSequence = DOTween.Sequence( );
                    textSequence.Append( terminalText.DOText( "Initiating . . ." , 3f ) )
                        .AppendCallback( ( ) => terminalText.text = "" )
                        .Append( terminalText.DOText( "Checking Network Status . . .\nEstablishing Connection to Satellite . . .\nAuthenticating Secure Link . . .\nRetrieving Camera Feed . . .\nProcessing Data Stream . . .\nSystem Ready" , 8f ) )
                        .AppendCallback( ( ) => terminalText.gameObject.SetActive( false ) )
                        .AppendInterval( 1f )
                        .AppendCallback( ( ) => simulatorWindow.SetActive( true ) );
                } );
            } );
        } );
    }

    private void FadeOutObject( GameObject target )
    {
        Image[ ] images = target.GetComponentsInChildren<Image>( true );
        
        foreach( Image img in images )
            img.DOFade( 0f, 1f );
        
        DOVirtual.DelayedCall( 1f, ( ) => {
            target.SetActive( false );
        } );
    }

    public void SwitchToCamMode( )
    {
        if ( !isCamMode )
        {
            isCamMode = true;
            camScreen.SetActive( true );
            radarScreen.SetActive( false);
            modeText.text = "Camera Mode";
        }
    }

    public void SwitchToRadarMode( )
    {
        if ( isCamMode )
        {
            isCamMode = false;
            camScreen.SetActive( false );
            radarScreen.SetActive( true );
            modeText.text = "Radar Mode";
        }
    }

    public void SetRadar(Vector2[] targets)
    {
        if (radarInstances != null)
        {
            foreach (GameObject radar in radarInstances)
            {
                if (radar != null)
                {
                    radar.SetActive(false);
                    radarPool.Enqueue(radar);
                }
            }
        }

        radarInstances = new GameObject[targets.Length];
        radarText.text = targets.Length + " detected";

        while (radarPool.Count < targets.Length)
        {
            GameObject radar = Instantiate(radarPrefab, radarSprite);
            radar.SetActive(false);
            radarPool.Enqueue(radar);
        }

        for (int i = 0; i < targets.Length; i++)
        {
            GameObject radar = radarPool.Dequeue();
            radar.transform.localPosition = new Vector3(targets[i].x, targets[i].y, 0f);
            radar.SetActive(true);
            radarInstances[i] = radar;
        }
    }

    private IEnumerator UpdateRadar()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            if (radarScreen.activeSelf)
                SetRadar(Satellite.instance.GetDistances());
            yield return wait;
        }
    }

    public void CaptureImage()
    {
        targetCoord = new Vector2( GMSManager.instance.latitude, GMSManager.instance.longitude );
        captureLoadPanel.SetActive(true);
        GMSManager.instance.CaptureImage(captureResultImage);
    }

    public void UpdateCoordinate(Vector2 target)
    {
        coordinateText.text = $"Latitude\n{target.x:F6}\nLongitude\n{target.y:F6}";
    }

    public void UpdateZoomText()
    {
        zoomText.text = zoomSlider.value.ToString();
    }

    public void CallMatch()
    {
        if(uploadedImage.sprite == null)
            return;

        // 스프라이트를 텍스처로 변환하고 jpg 파일로 저장
        byte[] captureBytes = captureResultImage.sprite.texture.EncodeToJPG();
        byte[] uploadedBytes = uploadedImage.sprite.texture.EncodeToJPG();

        string image1 = Convert.ToBase64String(captureBytes);
        string image2 = Convert.ToBase64String(uploadedBytes);

        // base64 문자열을 파일로 저장
        try
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "image1.txt"), image1);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "image2.txt"), image2);
            Debug.Log($"Images saved to: {Application.persistentDataPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving images: {e.Message}");
            return;
        }

        loadingPanel.SetActive(true);
        SiftFeatureMatching.instance.StartPython();
    }

    public void SetResult(float percentage)
    {
        resultPanel.SetActive(true);

        // 텍스처 로드 및 설정
        string imagePath = Path.Combine(Application.dataPath, "Resources", "output_image.jpg");
        byte[] fileData = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        
        // 텍스처 설정 변경
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        // 스프라이트 생성
        Sprite sprite = Sprite.Create(texture, 
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        
        resultImage.sprite = sprite;

        string colorHex;
        if (percentage <= 30f)
            colorHex = "#FF0000"; // 빨간색
        else if (percentage <= 60f)
            colorHex = "#FFA500"; // 주황색
        else
            colorHex = "#00FF00"; // 초록색

        resultText.text = $"Latitude : {targetCoord.x}\nLongitude : {targetCoord.y}\n\n<color={colorHex}>{percentage:F2}%</color> Match";
    }
}