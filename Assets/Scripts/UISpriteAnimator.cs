using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    public Sprite[] targets;
    public Image image;
    public int idx;
    private float timer;
    public float speed;

    void Start()
    {
        timer = 0;
        idx = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > speed)
        {
            idx = (idx + 1) % targets.Length;
            image.sprite = targets[idx];
            timer = 0;
        }
    }
}
