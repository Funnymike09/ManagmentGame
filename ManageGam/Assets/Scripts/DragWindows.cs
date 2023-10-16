using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragWindows : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 offset;
    public GameObject selectedObject;

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Input received");
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            Debug.Log(targetObject);
            if (targetObject)
            {
                selectedObject = targetObject.transform.gameObject;
                offset = selectedObject.transform.position - mousePosition;
                //Debug.Log(this.gameObject.name + " should now be last");
                selectedObject.transform.SetAsLastSibling();
            }
        }

        if (selectedObject)
        {
            selectedObject.transform.position = mousePosition + offset;
        }

        if (Input.GetMouseButtonUp(0) && selectedObject)
        {
            //transform.SetAsFirstSibling();
            selectedObject = null;
        }
    }
}
