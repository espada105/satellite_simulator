using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject loadBar;
    public TMP_Text terminalText;
    public GameObject simulatorWindow;

    void Start( )
    {
        StartAnimation( );
    }

    private void StartAnimation( )
    {
        DOVirtual.DelayedCall( 5f, ( ) => {
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
}