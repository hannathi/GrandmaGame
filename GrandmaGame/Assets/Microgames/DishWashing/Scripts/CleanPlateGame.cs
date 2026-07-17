using UnityEngine;

public class CleanPlateGame : MonoBehaviour
{
    [SerializeField]
    private Material cleanMaterial;
    private MeshRenderer meshRenderer;
    private bool isClean = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
    }

    void OnMouseDown(){
        if(!isClean){
            cleantThePlate();
        }
    }

    void cleantThePlate(){
        meshRenderer.material = cleanMaterial;
        isClean = true;
    }


}
