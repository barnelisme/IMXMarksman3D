using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextBlinkEffect : MonoBehaviour
{

    Text _lbl;
    public float blinkFadeInTime = 0.5f;
    public float blinkStayTime = 0.8f;
    public float blinkFadeOutTime = 0.7f;
    private float _timeChecker = 0;
    private Color _color;

    void Start()
    {
        _lbl = GetComponent<Text>();
        _color = _lbl.color;
    }

    // Update is called once per frame
    void Update()
    {
        _timeChecker += Time.deltaTime;
        if(_timeChecker < blinkFadeInTime)
        {
            _lbl.color = new Color(_color.r, _color.g, _color.b, _timeChecker / blinkFadeInTime);
        }
        else if(_timeChecker < blinkFadeInTime + blinkStayTime)
        {

        }
    }
}
