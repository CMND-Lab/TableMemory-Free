using UnityEngine;
using UXF;

public class RecordBounds : MonoBehaviour
{
    public string GetPosition() {
        Vector3 position = gameObject.transform.position;
            
        string str_position = position.ToString("F4");
            
        return str_position;
    }

    public string GetSize() {
        Vector3 size;
        if (gameObject.GetComponent<BoxCollider>()) {
            size = gameObject.GetComponent<BoxCollider>().bounds.size;
        } else if (gameObject.GetComponent<MeshRenderer>()) {
            size = gameObject.GetComponent<MeshRenderer>().bounds.size;
        } else
        {
            size = Vector3.zero;
        }

        string str_size = size.ToString("F4");

        return str_size;
    }

    public void Record(Trial trial) {
        bool gameObjectActive = gameObject.activeSelf;
        bool colliderActive = true;
        bool rendererActive = true;

        if (gameObject.GetComponent<BoxCollider>()) { colliderActive = gameObject.GetComponent<BoxCollider>().enabled; }
        if (gameObject.GetComponent<MeshRenderer>()) { rendererActive = gameObject.GetComponent<MeshRenderer>().enabled; }

        if (!gameObjectActive) { gameObject.SetActive(true); }
        if (!colliderActive) { gameObject.GetComponent<BoxCollider>().enabled = true; }
        if (!rendererActive) { gameObject.GetComponent<MeshRenderer>().enabled = true; }

        string name = gameObject.name;
        Debug.Log("Trial " + trial.number + " recording " + name + ":\n\t" + GetPosition().ToString() + "\n\t" + GetSize().ToString());

        trial.result[name + "__position"] = GetPosition();
        trial.result[name + "__size"] = GetSize();

        if (!colliderActive) { gameObject.GetComponent<BoxCollider>().enabled = false; }
        if (!rendererActive) { gameObject.GetComponent<MeshRenderer>().enabled = false; }
        if (!gameObjectActive) { gameObject.SetActive(false); }
    }
}