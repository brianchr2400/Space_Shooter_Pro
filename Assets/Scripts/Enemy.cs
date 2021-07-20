using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;
    private Animator _anim;
    private Collider2D _coll;
    private AudioSource _audioSource;
    private float _fireRate = 2.0f;
    private float _canFire = -1;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The animator is NULL!");
        }

        _coll = GetComponent<Collider2D>();
        if (_coll == null)
        {
            Debug.LogError("The enemy collider is NULL!");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The enemy _audioSource is NULL!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(5f, 8f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 8f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            //other.transform.GetComponent<Player>().Damage(); //This line is essentially the same as the following code except it fails if player doesn't exist, so the following is preferred
            _player = other.transform.GetComponent<Player>();
            if (_player != null)
            {
                _player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");
            StartCoroutine(EnemyDeathDelay());
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
        }

        else if (other.tag == "Laser")
        {

            Destroy(other);
            if (_player != null)
            {
                _player.AddScore(10);
            }

            _anim.SetTrigger("OnEnemyDeath");
            StartCoroutine(EnemyDeathDelay());
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }

        else if (other.tag == "Shield")
        {
            _player = other.transform.GetComponent<Player>();
            if (_player != null)
            {
                _player.Damage();
            }
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(other);
            Destroy(this.gameObject);
        }
    }

    IEnumerator EnemyDeathDelay()
    {
        yield return new WaitForSeconds(0.5f);
        _speed /= 2;
        yield return new WaitForSeconds(0.5f);
        _speed = 0;
    }

}
