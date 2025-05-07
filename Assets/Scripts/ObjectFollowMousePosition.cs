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
        //���콺 ��ǥ�� ���� ��ǥ��
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = mainCamera.ScreenToWorldPoint(position);
        //z��ġ 0����
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
