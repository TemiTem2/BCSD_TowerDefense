using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject enemyPrefab; //적 프리팹
    [SerializeField] 
    private GameObject enemyHPSliderPrefab; //적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private Transform canvasTransform;//UI canvas 오브젝트 transform
    //[SerializeField]
    //private float spawnTime;  //적 생성 주기
    [SerializeField]
    private Transform[] wayPoints; //현재 스테이지 이동경로
    [SerializeField]
    private PlayerHP playerHP; //플레이어 체력 컴포넌트
    [SerializeField]
    private PlayerGold playerGold; //골드 컴포넌트
    private Wave currentWave;     //현재 웨이브 정보
    private int currentEnemyCount;  //현재 웨이브에 남아있는 적 숫자(웨이브 시작시 max, 적 사망시 -1)
    private List<Enemy> enemyList;//모든 적의 정보

    //적의 생성과 삭제는 EnemySpawner에서 하기 때문에 Set은 필요X
    public List<Enemy> EnemyList => enemyList;
    //현재 웨이브의 남아있는 적, 최대 적 숫자
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        //적 리스트 메모리 할당
        enemyList = new List<Enemy>();
        //적 생성 코루틴 함수 호출
        //StartCoroutine("spawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        //매개변수로 받아온 웨이브 정보 저장
        currentWave = wave;
        //현재 웨이브의 최대 적 숫자를 저장
        currentEnemyCount = currentWave.maxEnemyCount;
        //현재 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        //현재 웨이브에서 생성한 적 숫자
        int spawnEnemyCount = 0;

        //while (true)
        //현재 웨이브에서 생성되어야 하는 적의 숫자만큼 적을 생성하고 코루틴 종료
        while ( spawnEnemyCount < currentWave.maxEnemyCount )
        {
            //GameObject clone = Instantiate(enemyPrefab); //적 오브젝트 생성
            //웨이브에 등장하는 적의 종류가 여러 종류일 때 임의의 적이 등장하도록 설정하고 적 오브젝트 생성
            int enemyIndex = Random.Range(0,currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();          //방금 생성된적의 ENMY컴포넌트

            //this는 자신의 EnemySpawner 정보
            enemy.Setup(this,wayPoints);//setup호출
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone); //적 체력을 나타내는 Slider 생성

            spawnEnemyCount++;

            //yield return new WaitForSeconds(spawnTime);
            yield return new WaitForSeconds(currentWave.spawnTime); //spawnTime만큼 대기
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy,int gold)
    {
        if (type == EnemyDestroyType.Arrive) //도착시
        {
            playerHP.TakeDamage(1);//-1 체력
        }

        else if (type == EnemyDestroyType.Kill) 
        {
            playerGold.CurrentGold += gold;
        }
        enemyList.Remove(enemy);

        //적 사망시 적 숫자 표시 감소
        currentEnemyCount--;
        //리스트에서 사망한 적 정보 삭제
        enemyList.Remove(enemy);
        //적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        //적 체력을 나타내는 Slider UI생성
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        //Slider UI 오브젝트를 parent("Canvas"오브젝트)의 자식으로 설정
        //tip.UI는 캔버스의 자식으로 설정되어있어야 보인다
        sliderClone.transform.SetParent(canvasTransform);
        //계층설정으로 바뀐 크기를 다시 (1,1,1)로 설정
        sliderClone.transform.localScale = Vector3.one;

        //Slider UI가 쫒아다닐 대상을 본인을 ㅗ설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        //Slider UI에 자신의 체력 정보를 표시하도록 설정
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
