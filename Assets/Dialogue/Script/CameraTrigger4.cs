using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger4 : MonoBehaviour
{
    public Vector2 CamRangeX;
    public Vector2 CamRangeY;
    public float CamSize;
    public bool Disable = true;
    public AudioClip audioBG;
    public Vector3 RespawnOffset = new Vector3(-1, 0, 0);
    //public float CamLerp;

    private void Update()
    {
        if (Camera.main.GetComponent<CameraMove4>().Lerp == 10)
        {
            StopCoroutine(LerpBack());
            StartCoroutine(LerpBack());
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Camera.main.GetComponent<CameraMove4>().TargetRangeX == CamRangeX && Camera.main.GetComponent<CameraMove4>().TargetRangeY == CamRangeY)
            {
                //Camera.main.GetComponent<CameraMove4>().Lerp = 3000;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Camera.main.GetComponent<CameraMove4>().TargetRangeX != CamRangeX || Camera.main.GetComponent<CameraMove4>().TargetRangeY != CamRangeY)
            {
                if (Camera.main.GetComponent<AudioSource>().clip != audioBG)
                {
                    //if (audioBG )
                    Camera.main.GetComponent<AudioSource>().clip = audioBG;
                    Camera.main.GetComponent<AudioSource>().Play();
                }
                Camera.main.GetComponent<CameraMove4>().Lerp = 10;
                //collision.GetComponent<PlayerRespawn>().CheckPoint = transform.position + RespawnOffset;
                //collision.transform.position = new Vector2(transform.position.x + RespawnOffset.x/2, collision.transform.position.y);
                Camera.main.GetComponent<CameraMove4>().TargetRangeX = CamRangeX;
                Camera.main.GetComponent<CameraMove4>().TargetRangeY = CamRangeY;
                Camera.main.GetComponent<CameraMove4>().TargetSize = CamSize;
                StopCoroutine(LerpBack());
                Camera.main.GetComponent<CameraMove4>().Lerp = 10;
                Debug.Log("CamChange");

                if (Disable)
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Animation("Player_Idle");
                    StopCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().DisableMovement(0));
                    StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().DisableMovement(1.0f));
                }
                
            }
        }
    }

    IEnumerator LerpBack()
    {
        yield return new WaitForSeconds(0.5f);

        if (Camera.main.GetComponent<CameraMove4>().TargetRangeX == CamRangeX && Camera.main.GetComponent<CameraMove4>().TargetRangeY == CamRangeY)
        {
            Camera.main.GetComponent<CameraMove4>().Lerp = 3000;
        }
    }
}
