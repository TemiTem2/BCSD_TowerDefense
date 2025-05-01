using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public bool IsBuildTower { set; get; }
    
    void Awake()
    {
        IsBuildTower = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
