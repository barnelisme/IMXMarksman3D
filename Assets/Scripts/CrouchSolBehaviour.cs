using System.Collections;
using UnityEngine;

public class CrouchSolBehaviour : MonoBehaviour
{
    [Header("VALUE")]
    [Space(5)]
    [SerializeField] public int rayDistance = 1000;
    [Space(5)]
    [SerializeField] public float hitTime = 0.5f;
    [Space(5)]
    [SerializeField] public  LayerMask layer;

    [Header("COMPONENTS")]
    [Space(5)]
    [SerializeField] public AudioClip audioClip;
    [Space(5)]
    [SerializeField] public GameObject hitUi;
    [Space(5)]

    new AudioSource audio;
    Animator _anim;
    Transform enemy;

    void Awake() => hitUi.SetActive(false);
    void Update() => ShootInput();

    void ShootInput()
    {
    }

    void PlayAudio(AudioClip tempAudioClip)
    {
        audio.clip = null;
        audio.clip = tempAudioClip;
        audio.Play();
    }

    void HitCheck(RaycastHit tempHit)
    {
        enemy = tempHit.collider.gameObject.transform;
        if (enemy.parent.GetComponent<Animator>() != null)
            _anim = enemy.parent.GetComponent<Animator>();

        else return;
        StartCoroutine(HitUI());
        _anim.SetInteger("crouchState", 1);
        // PlayAudio (audioClip);
    }

    IEnumerator HitUI()
    {
        hitUi.SetActive(true);
        hitUi.transform.position = new Vector3(enemy.position.x, enemy.position.y + 2.0f, enemy.position.z);
        yield return new WaitForSeconds(hitTime);
        hitUi.SetActive(false);
    }


}