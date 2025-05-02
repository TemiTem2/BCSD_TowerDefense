using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; //적 프리팹
    [SerializeField] 
    private GameObject enemyHPSliderPrefab; //적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private Transform canvasTransform;//UI canvas 오브젝트 transform
    [SerializeField]
    private float spawnTime;  //적 생성 주기
    [SerializeField]
    private Transform[] wayPoints; //현재 스테이지 이동경로
    private List<Enemy> enemyList;//모든 적의 경로


    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("spawnEnemy");
    }

    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab); //적 오브젝트 생성
            Enemy enemy = clone.GetComponent<Enemy>();//방금 생성된적의 ENMY컴포넌트

            enemy.Setup(this,wayPoints);//setup호출
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone); //적 체력을 나타내는 Slider 생성

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
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
