using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;
    [SerializeField]
    private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null;  //���콺�� ������ ������Ʈ �ӽ�����
    
    
    
    private void Awake()
    {
        //maincamera�±� �ް� ������ camera ������Ʈ ���� ����
        mainCamera = Camera.main;
    }

    private void Update()
    {
        //���콺�� UI���� ���� �� �Ʒ��ڵ� ���� ����
        if ( EventSystem.current.IsPointerOverGameObject() == true )
        {
            return;
        }

        //���콺 ��ư��������
        if (Input.GetMouseButtonDown(0))
        {
            //ī�޶� ��ġ���� ȭ���� ���콺 ��ġ�� �����ϴ� ���� ����
            //ray.origin :������ ������ġ
            //ray.direction :������ �������
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //2D����͸� ���� 3D������ ������Ʈ�� ���콺�� �����ϴ� ���
            //������ �ε����� ������Ʈ�� �����ؼ� hit�� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                //������ �ε��� ������Ʈ�� �±װ� "Tile"�̸�
                if (hit.transform.CompareTag("Tile"))
                {
                    //Ÿ���� �����ϴ� SpanwTower() ȣ��
                    towerSpawner.SpawnTower(hit.transform);
                }
                //Ÿ�� ���ý� Ÿ�� ����â ����
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.Onpanel(hit.transform);
                }
            }
        }
        else if ( Input.GetMouseButtonUp(0))
        {
            //���콺�� ������ �� ������Ʈ X, Ȥ�� Ÿ���� �ƴ϶��
            if (hitTransform == null || hitTransform.CompareTag("Tower") == false )
            {
                //Ÿ�� ���� ����
                towerDataViewer.OffPanel();
            }

            hitTransform = null;
        }
    }
}
