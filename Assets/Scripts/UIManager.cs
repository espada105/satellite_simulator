using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
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

    private Queue<GameObject> radarPool = new Queue<GameObject>();
    private const int INITIAL_POOL_SIZE = 10;

    void Start()
    {
        InitializeRadarPool();
        StartAnimation();
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
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            if (radarScreen.activeSelf)
                SetRadar(Satellite.instance.GetDistances());
            yield return wait;
        }
    }
}