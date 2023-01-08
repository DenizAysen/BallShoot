using UnityEngine;
public class Top : MonoBehaviour
{
    Rigidbody rb;
    Renderer renk;
    public GameManager _GameManager;
    float zaman;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        renk = GetComponent<Renderer>();
        zaman = 0f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kova"))
        {
            TopuResetle();
            _GameManager.TopGirdi();
        }
        else if (other.CompareTag("Zemin"))
        {
            TopuResetle();
            _GameManager.TopGirmedi();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tahta"))
        {
            zaman += Time.deltaTime;
            if(zaman > 3f)
            {
                _GameManager.TopGirmedi();
                zaman = 0f;
            }
        }
    }
    void TopuResetle()
    {
        _GameManager.ParcEfektOrtayaCikart(transform.position, renk.material.color);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
