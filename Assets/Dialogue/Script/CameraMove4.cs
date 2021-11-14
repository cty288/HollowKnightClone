using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove4 : MonoBehaviour
{
    public float TargetSize = 10f;
    public float Lerp = 10f;
    public Vector2 TargetRangeX;
    public Vector2 TargetRangeY;
    public GameObject Player;

    private void Update()
    {
        //Debug.Log(Lerp);

        Player = GameObject.FindWithTag("Player");

        Vector3 target = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);
        //transform.position = Vector3.Lerp(transform.position, target, Lerp * Time.deltaTime);
        //transform.position = new Vector2(Mathf.Clamp(transform.position.x, TargetRangeX.x + 16, TargetRangeX.y - 16), Mathf.Clamp(target.y, TargetRangeY.x + 10, TargetRangeY.y - 10));
        target = Vector3.Lerp(transform.position, target, 5 * Time.deltaTime);
        target.x = Mathf.Clamp(target.x, TargetRangeX.x + 16, TargetRangeX.y - 16);
        target.y = Mathf.Clamp(target.y, TargetRangeY.x + 10, TargetRangeY.y - 10);
        //transform.position = target;
        transform.position = Vector3.Lerp(transform.position, target, Lerp * Time.deltaTime);

        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, TargetSize, 1000 * Time.deltaTime);
    }
}
