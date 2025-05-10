using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate towerTemplate;    //Ÿ�� ����
    [SerializeField]
    private Transform spawnPoint;   //�߻�ü ���� ��ġ
    [SerializeField]
    private WeaponType weaponType;          //���� �Ӽ�

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab; //�߻�ü ������

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;      //�������� ���Ǵ� ��
    [SerializeField]
    private Transform hitEffect;            //Ÿ�� ȿ��
    [SerializeField]
    private LayerMask targetLayer;          //������ �΋H���� ���̾�

    private int level = 0;                                              //Ÿ�� ����
    private WeaponState weaponState = WeaponState.SearchTarget;         //Ÿ�� ������ ����
    private Transform attackTarget = null;                              //���ݴ��
    private SpriteRenderer spriteRenderer;                              //Ÿ�� �̹��� ����
    private TowerSpawner towerSpawner;
    private EnemySpawner enemySpawner;                                  //���ӿ� �����ϴ� �� ���� ȹ���
    private PlayerGold playerGold;                                      //��� ����
    private Tile ownerTile;                                             //��ġ�� Ÿ������

    private float addedDamage;                                          //������ ���� �߰��Ǵ� ������
    private int buffLevel;                                              //���� ����

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level+1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        //���Ⱑ ĳ��, �������϶���
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
            //���� ����� �� Ž��
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
            //target ���� �������� �˻�
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
        //������ Ȱ��ȭ
        EnableLaser();

        while (true)
        {
            //���� �Ұ����� ���
            if (IsPossibleToAttackTarget() == false)
            {
                //������ ��Ȱ
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            //����
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        //��� Ÿ�� Ž��
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for ( int i = 0; i < towers.Length; i++ )
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            //�̹� ������ �ް� �ִٸ� �н�
            if ( weapon.BuffLevel > Level )
            {
                continue;
            }

            //���� ������ Ÿ���� �ִٸ�
            if ( Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range )
            {
                //����Ÿ���Ͻ�
                if ( weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser )
                {
                    //�� ����
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    //�����޴� Ÿ���� ��������
                    weapon.BuffLevel = Level;
                }
            }

        }

        
    }    

    private Transform FindClosestAttackTarget()
    {
        //���ʰŸ� ũ�� ����(�ּҰŸ� �� Ž��)
        float closestDistSqr = Mathf.Infinity;
        //��� �� �˻�
        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position) ;
            //���� �����Ÿ� ��, ���� ����� ��
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
        //Ÿ�� ���� �˻�
        if ( attackTarget == null )
        {
            return false;
        }

        return true;
    }    

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        //���� ��� ���ݷ� ���
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
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

        //target�� �µ��� ���� ������ �߻�
        for ( int i = 0; i < hit.Length; i++ )
        {
            if (hit[i].transform == attackTarget)
            { 
                //�� ����
                lineRenderer.SetPosition(0, spawnPoint.position);
                //�� ��ǥ
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                //Ÿ�� ȿ�� ��ġ
                hitEffect.position = hit[i].point;
                //�� ü�°���
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }
    
    public bool Upgrade()
    {
        //��� ������� Ȯ��
        if ( playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost )
        {
            return false;
        }

        //Ÿ�� ������
        level++;
        //Ÿ�� ���� ����
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        //��� ����
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        //���Ⱑ ���������
        if ( weaponType == WeaponType.Laser )
        {
            //���� ��ȭ
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        //���� ����
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        //��� ����
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        //�Ǽ� ���� Ÿ�Ϸ� ����
        ownerTile.IsBuildTower = false;
        //Ÿ�� ����
        Destroy(gameObject);
    }
}
