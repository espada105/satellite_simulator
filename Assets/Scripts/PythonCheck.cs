using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PythonCheck : MonoBehaviour
{
    public TMP_Text checkText;

    void Start()
    {
        string check = PythonPathFinder.GetPythonPath( );

        checkText.DOText( "Checking Python System . . . ", 3f );
        DOVirtual.DelayedCall( 6f, ( ) => {
            if( check == "X" ) {
                checkText.DOText( "Python is not installed.\nYou must install Python to use this program.\nPlease install Python and restart this program.", 6f );
            } else {
                checkText.DOText( "Found Python file.\nStarting Simulator . . . ", 4f );
                DOVirtual.DelayedCall( 5f, ( ) => {
                    SceneManager.LoadScene( "UITest" );
                } );
            };
        } );
    }
}