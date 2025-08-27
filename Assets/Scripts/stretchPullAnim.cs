using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class stretchPullAnim : MonoBehaviour
{

    public float maxScaleY;
    public float minScaleY;

    public float moveAmount;
    //private float originalPos;

    public float adjustmentAmount;

    public bool shrink;
    public bool moveDown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //originalPos = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localScale.y >= maxScaleY)
        {
            shrink = true;
        }
        else if (this.transform.localScale.y <= minScaleY)
        {
            shrink = false;
        }


        if (shrink == true)
        {
            this.transform.localScale -= new Vector3(0, adjustmentAmount, 0);
            this.transform.position -= new Vector3(0, moveAmount, 0);

        }
        else if (shrink == false)
        {
            this.transform.localScale += new Vector3(0, adjustmentAmount, 0);
            this.transform.position += new Vector3(0, moveAmount, 0);

        }


    }

}
