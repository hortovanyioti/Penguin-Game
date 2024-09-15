using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponScript : MonoBehaviour
{
    [field: SerializeField] public float bulletSize { get; private set; }
    [field: SerializeField] public float bulletForce { get; private set; }
    [field: SerializeField] public float fireRate { get; private set; } //RPM

    [SerializeField] float bulletOffsetForward = 0.6f;
    [SerializeField] float bulletOffsetUp = 0.25f;
    [SerializeField] float bulletOffsetRight = 0.1f;
    [SerializeField] GameObject projectile;

    private bool isPrimaryFire;
    private float timeSinceFire;
    private AudioSource fireSound;

    public void OnFire(InputAction.CallbackContext context) //Implements HOLD functionality
    {
        if (context.started)
            isPrimaryFire = true;

        else if (context.canceled)
            isPrimaryFire = false;
    }
    void Start()
    {
        fireSound = GetComponent<AudioSource>();
    }
    void Update()
    {
        timeSinceFire += Time.deltaTime;    //Increment the time since the last shot
        if (isPrimaryFire)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (Time.timeScale == 0 || timeSinceFire < 1 / (fireRate / 60))    //Restrict gun to fire rate
            return;

		/*if (!GameManagerScript.Instance.IsGameOver)
            GameManagerScript.Instance.PlayerScripts[0].stats.ShotsFired++;*/

        var newProjectile = Instantiate(projectile);
        //newProjectile.transform.parent = GameManagerScript.Instance.BulletPool.transform;

        newProjectile.transform.position = this.transform.position +
            this.transform.forward * bulletOffsetForward +
            this.transform.up * bulletOffsetUp +
            this.transform.right * bulletOffsetRight;   //Spawn the projectile in front of the player

        newProjectile.transform.rotation = transform.rotation;  //Set the rotation of the projectile to the rotation of the player
        newProjectile.transform.Rotate(90f, 0f, 0f);

        newProjectile.transform.localScale *= bulletSize;
        newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(bulletSize, 3);
        newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce, ForceMode.Impulse);   //Add force to the projectile
        timeSinceFire = 0;

        fireSound.Play();
    }
}
