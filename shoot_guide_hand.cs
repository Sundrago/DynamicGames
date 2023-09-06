using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class shoot_guide_hand : MonoBehaviour
{
    [SerializeField]
    private float dist, speed;
    private Vector3 startPosition;
    private Image img;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = startPosition + new Vector3(Mathf.Sin(Time.time * speed) * dist, Mathf.Cos(Time.time * speed)* dist, 0);
    }

    public void Show()
    {
        Color color = Color.white;
        color.a = 0;

        img.color = color;
        img.DOFade(1, 2f);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        img.DOFade(0, 0.5f)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
    }
}
