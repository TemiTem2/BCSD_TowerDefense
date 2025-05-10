using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public enum WeaponType { Cannon = 0, Laser, Slow, }
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate towerTemplate;    //타워 정보
    [SerializeField]
    private Transform spawnPoint;   //발사체 생성 위치
    [SerializeField]
    private WeaponType weaponType;          //무기 속성

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab; //발사체 프리팹

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;      //레이저로 사용되는 선
    [SerializeField]
    private Transform hitEffect;            //타격 효과
    [SerializeField]
    private LayerMask targetLayer;          //광선에 부딫히는 레이어

    private int level = 0;                                              //타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget;         //타워 무기의 상태
    private Transform attackTarget = null;                              //공격대상
    private SpriteRenderer spriteRenderer;                              //타워 이미지 변경
    private EnemySpawner enemySpawner;                                  //게임에 존재하는 적 정보 획득용
    private PlayerGold playerGold;                                      //골드 정보
    private Tile ownerTile;                                             //배치된 타일정보

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public WeaponType WeaponType => weaponType;

    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        //무기가 캐논, 레이저일때만
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());

        weaponState = newState;

        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null) {
            RotateToTarget();
        }

    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        float degree = Mathf.Atan2(dy,dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            //가장 가까운 적 탐색
            attackTarget = FindClosestAttackTarget();

            if (attackTarget != null) 
            {
                if ( weaponType == WeaponType.Cannon )
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if ( weaponType == WeaponType.Laser )
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            //target 공격 가능한지 검사
            if ( IsPossibleToAttackTarget() == false )
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        //레이저 활성화
        EnableLaser();

        while (true)
        {
            //공격 불가능한 경우
            if (IsPossibleToAttackTarget() == false)
            {
                //레이저 비활
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            //공격
            SpawnLaser();

            yield return null;
        }
    }

    private Transform FindClosestAttackTarget()
    {
        //최초거리 크게 설정(최소거리 적 탐색)
        float closestDistSqr = Mathf.Infinity;
        //모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position) ;
            //적이 사정거리 내, 가장 가까울 시
            if ( distance <= towerTemplate.weapon[level].range && distance < closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        //타겟 유무 검사
        if ( attackTarget == null )
        {
            return false;
        }

        return true;
    }    

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        //target에 맞도록 광선 여러개 발사
        for ( int i = 0; i < hit.Length; i++ )
        {
            if (hit[i].transform == attackTarget)
            { 
                //선 시작
                lineRenderer.SetPosition(0, spawnPoint.position);
                //선 목표
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                //타격 효과 위치
                hitEffect.position = hit[i].point;
                //적 체력감소
                attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
            }
        }
    }
    
    public bool Upgrade()
    {
        //골드 충분한지 확인
        if ( playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost )
        {
            return false;
        }

        //타워 레벨업
        level++;
        //타워 외형 변경
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        //골드 차감
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        //무기가 레이저라면
        if ( weaponType == WeaponType.Laser )
        {
            //굵기 변화
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }    

        return true;
    }

    public void Sell()
    {
        //골드 지금
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        //건설 가능 타일로 변경
        ownerTile.IsBuildTower = false;
        //타워 삭제
        Destroy(gameObject);
    }
}
