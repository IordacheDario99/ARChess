using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject _gameObject;
    [SerializeField] private Material hitMaterial;
    private Material _originalMaterial;
    [SerializeField] private Rigidbody rig;
    private MeshCollider _mesh;
    [SerializeField] private float _speed = 0.1f;
    private bool flag = true;
    [SerializeField] private ParticleSystem _selected;

    public float MaxDubbleTapTime;
    private float holdTime = 0.5f;
    private float acumTime = 0;

    void Start()
    {
        _selected.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {

            acumTime += Input.GetTouch(0).deltaTime;


            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                rig = hitInfo.collider.GetComponent<Rigidbody>();
                _mesh = hitInfo.collider.GetComponent<MeshCollider>();


                if (_mesh != null)
                {


                    //_mesh.GetComponent<MeshRenderer>().material = hitMaterial;
                    // if hold foo = hitinfo.trans.pos
                    //else if double tap destination = hitinfo.trans.pos
                    RaycastHit startPos = hitInfo;


                    if (acumTime >= holdTime)
                    {
                        _gameObject = hitInfo.transform.gameObject;
                        _selected.transform.position = _gameObject.transform.position;
                        _selected.Play();
                        Debug.Log(_gameObject.transform.name + " " + acumTime);
                    }

                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        acumTime = 0;
                    }

                    if (Input.GetTouch(0).tapCount == 2)
                    {
                        _gameObject.transform.position = Vector3.MoveTowards(_gameObject.transform.position, hitInfo.transform.position, _speed);
                        _selected.Stop();
                    }

                }
                else
                {

                    Debug.Log("No rigid body facing the ray");
                }
            }



        }
    }




}
