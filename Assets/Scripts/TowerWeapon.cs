using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using Unity.VisualScripting;

public enum WeaponState { SearchTarget = 0, AttackToTarget}

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate;    //타워 정보
    [SerializeField]
    private GameObject projectilePrefab; //발사체 프리펩
    [SerializeField]
    private Transform spawnpoint; //발사체 생성 위치
    private int level = 0;          //타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget;//타워ㅏ 무기의 상태
    private Transform attackTarget = null;//공격대상
    private SpriteRenderer spriteRenderer;  //타워 이미지 변경
    private EnemySpawner enemySpawner;//게임에 존재하는 적 정보 획득용
    private PlayerGold playerGold;  //골드 정보
    private Tile ownerTile;     //배치된 타일정보

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;

    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        ChangeState(WeaponState.SearchTarget); 
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
            float closestDistSqr = Mathf.Infinity;

            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

                if (distance <= towerTemplate.weapon[level].range &&distance <= closestDistSqr) 
                { 
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if (attackTarget != null) 
            {
                ChangeState(WeaponState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        while (true)
        {
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > towerTemplate.weapon[level].range)
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnpoint.position, Quaternion.identity);

        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
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
