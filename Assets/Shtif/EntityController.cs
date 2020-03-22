using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class EntityController : MonoBehaviour
{
    [HideInInspector]
    public EntityManager manager;

    private HealthState _state = HealthState.CLEAN;
    public bool stationary = false;
    public bool randomizeOrigin = true;
    public float maxDistance = 100;

    public Vector3 speed;

    public Vector3 origin;
    private RawImage _image;
    private Vector3 destination;
    private float speedScale = 10;
    private Rigidbody2D body;

    private int _percentage;
    private float _infectionTime = 0;

    private bool _deadCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        _percentage = Random.Range(0, 100);
        body = GetComponent<Rigidbody2D>();

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        circle.radius = manager.simulation.settings.infectionRadius * 6.5f + 6.5f;

        if (randomizeOrigin)
        {
            origin = new Vector3(Random.Range(manager.boundaryLeft, manager.boundaryRight),
                Random.Range(manager.boundaryTop, manager.boundaryBottom));
        }

        transform.position = origin;
        
        if (stationary)
        {
            speed = Vector3.zero;
            body.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            speed = new Vector3(Random.Range(-speedScale, speedScale), Random.Range(-speedScale, speedScale), 0);
            body.velocity = speed;
        }
    }

    public bool HasState(HealthState p_state)
    {
        return p_state == _state;
    }
    
    private void ChangeState(HealthState p_state)
    {
        if (_state == p_state) return;

        _state = p_state;
        
        if (_image == null)
        {
            _image = GetComponent<RawImage>();
        }
        
        switch (_state)
        {
            case HealthState.CLEAN:
                _image.DOColor(Color.white, .5f);
                //_image.color = Color.white;
                break;
            case HealthState.CURED:
                _image.DOColor(Color.green, .5f);
                //_image.color = Color.green;
                break;
            case HealthState.INFECTED:
                _image.DOColor(Color.red, .5f);
                //_image.color = Color.red;
                break;
            case HealthState.DEAD:
                stationary = true;
                body.bodyType = RigidbodyType2D.Static;
                _image.DOColor(Color.blue, .5f);
                //_image.color = new Color(158,255,255);
                break;
            case HealthState.QUARANTINED:
                stationary = true;
                body.bodyType = RigidbodyType2D.Static;
                _image.DOColor(Color.magenta, .5f);
                break;
        }
    }

    public void Stop()
    {
        body.bodyType = RigidbodyType2D.Static;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!manager.simulation.IsRunning()) return;
        
        if (_state == HealthState.INFECTED || _state == HealthState.QUARANTINED)
        {
            if (manager.simulation.time - _infectionTime > manager.simulation.settings.averageDeathTime && !_deadCheck)
            {
                _deadCheck = true;
                if (_percentage < 20 && Random.Range(0, 100) < 18*(manager.simulation.infected/10))
                {
                    ChangeState(HealthState.DEAD);
                }
            }

            if (manager.simulation.time - _infectionTime > manager.simulation.settings.averageRecoveryTime)
            {
                if (Random.Range(0, 100) < 50)
                {
                    ChangeState(HealthState.CURED);
                }
            }
        }

        transform.position += speed * Time.deltaTime*10;

        bool hitBoundary = false;
        speed = body.velocity;
        if (speed.magnitude > 10)
        {
            speed = speed.normalized * 10;
        }

        if (transform.position.x > manager.boundaryRight)
        {
            //transform.position = new Vector3(2*entityManager.boundaryRight - transform.position.x, transform.position.y, 0);
            speed.x = -speed.x;
            hitBoundary = true;
        }
        
        if (transform.position.x < manager.boundaryLeft)
        {
            //transform.position = new Vector3(2*entityManager.boundaryLeft - transform.position.x, transform.position.y, 0);
            speed.x = -speed.x;
            hitBoundary = true;
        }
        
        if (transform.position.y > manager.boundaryTop)
        {
            //transform.position = new Vector3(transform.position.x, 2*entityManager.boundaryTop - transform.position.y, 0);
            speed.y = -speed.y;
            hitBoundary = true;
        }
        
        if (transform.position.y < manager.boundaryBottom)
        {
            //transform.position = new Vector3(transform.position.x, 2*entityManager.boundaryBottom - transform.position.y, 0);
            speed.y = -speed.y;
            hitBoundary = true;
        }

        if (hitBoundary && !stationary)
        {
            body.velocity = speed;
        }
    }

    public bool Test()
    {
        if (_state == HealthState.INFECTED)
        {
            ChangeState(HealthState.QUARANTINED);
        }

        return _state == HealthState.QUARANTINED;
    }

    public void Infect(bool p_forceInfection)
    {
        if (_state != HealthState.CLEAN) return;

        if (Random.Range(0, 100) < manager.simulation.settings.infectionProbability || p_forceInfection)
        {
            ChangeState(HealthState.INFECTED);
            _infectionTime = manager.simulation.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EntityController entity = other.gameObject.GetComponent<EntityController>();
        if (entity != null)
        {
            if (_state == HealthState.INFECTED)
            {
                entity.Infect(false);
            }
        }
    }
    /*
    private void OnCollisionStay2D(Collision2D other)
    {
        EntityController entity = other.gameObject.GetComponent<EntityController>();
        if (entity != null)
        {
            if (_state == HealthState.INFECTED)
            {
                entity.Infect(false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        EntityController entity = other.gameObject.GetComponent<EntityController>();
        if (entity != null)
        {
            if (_state == HealthState.INFECTED)
            {
                entity.Infect(false);
            }
        }
    }
    /**/
}
