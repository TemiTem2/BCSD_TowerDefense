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
    private Transform hitTransform = null;  //마우스로 선택한 오브젝트 임시저장
    
    
    
    private void Awake()
    {
        //maincamera태그 달고 있으면 camera 컴포넌트 정보 전달
        mainCamera = Camera.main;
    }

    private void Update()
    {
        //마우스가 UI위에 있을 때 아래코드 실행 방지
        if ( EventSystem.current.IsPointerOverGameObject() == true )
        {
            return;
        }

        //마우스 버튼눌렀을때
        if (Input.GetMouseButtonDown(0))
        {
            //카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            //ray.origin :광선의 시작위치
            //ray.direction :광선의 진행방향
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //2D모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
            //광선에 부딪히는 오브젝트를 검출해서 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                //광선에 부딪힌 오브젝트의 태그가 "Tile"이면
                if (hit.transform.CompareTag("Tile"))
                {
                    //타워를 생성하는 SpanwTower() 호출
                    towerSpawner.SpawnTower(hit.transform);
                }
                //타워 선택시 타워 정보창 띄우기
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.Onpanel(hit.transform);
                }
            }
        }
        else if ( Input.GetMouseButtonUp(0))
        {
            //마우스를 눌렀을 때 오브젝트 X, 혹은 타워가 아니라면
            if (hitTransform == null || hitTransform.CompareTag("Tower") == false )
            {
                //타워 정보 끄기
                towerDataViewer.OffPanel();
            }

            hitTransform = null;
        }
    }
}
