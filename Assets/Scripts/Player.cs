using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    // Powerups
    [SerializeField]
    private bool isTripleShotActive = false;
    [SerializeField]
    private bool isShieldActive = false;

    [SerializeField]
    private GameObject _shieldVisualizer;

    [SerializeField]
    private int _score;

    [SerializeField]
    private GameObject _rightThrusterVisualizer;

    [SerializeField]
    private GameObject _leftThrusterVisualizer;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Set the current position to center, or new position (0, 0, 0)
        transform.position = new Vector3(0, -3, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.right * 5 * Time.deltaTime);
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        //transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        //transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

            transform.Translate(direction * _speed * Time.deltaTime);

        // Up and down boundary or y boundary
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        // The following causes the player to wrap around
        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }

    }

    void FireLaser()
    {


        _canFire = Time.time + _fireRate;
        
        //Instantiate(_laserPrefab, new Vector3(transform.position.x, transform.position.y + 0.8f,0), Quaternion.identity);

        //if space key press,
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if isTripleShotActive is true
            if (isTripleShotActive)
            {
                //fire 3 lasers (triple shot prefab)
                Instantiate(_tripleLaserPrefab, transform.position, Quaternion.identity);
            }
            //else fire 1 laser
            else
            {
                //Instantiate 3 lasers (triple shot prefab
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
            }
        }

        _audioSource.Play();

    }

    public void Damage()
    {
        //if shields is active
        //do nothing. . . .
        //deactivate
        //return
        if (isShieldActive)
        {
            DeactivateShield();
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _rightThrusterVisualizer.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftThrusterVisualizer.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void DeactivateShield()
    {
        isShieldActive = false;
        _shieldVisualizer.SetActive(false);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
