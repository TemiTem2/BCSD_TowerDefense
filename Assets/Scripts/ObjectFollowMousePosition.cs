using System.Xml.Serialization;
using UnityEngine;

public class ObjectFollowMousePosition : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        //마우스 좌표를 게임 좌표로
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = mainCamera.ScreenToWorldPoint(position);
        //z위치 0으로
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
