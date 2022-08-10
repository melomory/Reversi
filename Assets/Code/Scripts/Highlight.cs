using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField]
    private Color _normalColor;

    [SerializeField]
    private Color _mouseOverColor;

    private Material _material;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _material.color = _normalColor;
    }

    private void OnMouseEnter()
    {
        _material.color = _mouseOverColor;
    }

    private void OnMouseExit()
    {
        _material.color -= _normalColor;
    }

    private void OnDestroy()
    {
        Destroy(_material);
    }
}
