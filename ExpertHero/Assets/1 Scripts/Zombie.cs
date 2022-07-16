using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour,ITargetChange
{
    [SerializeField] int damage;
    [SerializeField] int coinCount;
    [SerializeField] int health;
    int currentHealth;
    [SerializeField] GameObject coinPrefab;
    public NavMeshAgent agent;

    [Range(0, 50)] [SerializeField] public float walkSpeed, rotSpeed;
    public bool gun;

    public enum States {  attack, followingPlayer, idle, dead }
    public States currentBehaviour;
   public GameObject player;
    public Animator anim;
    Rigidbody rigidbody;
    [SerializeField] float deadTime;
    float counter = 0f;
    public bool isSelected = false;
    PlayerControl playerParentControl;
    public FieldOfView eye;
    public SkinnedMeshRenderer enemyMesh;
    [SerializeField] Material deadMaterial;
   public bool deadActive = false;
    bool counterActive = true;
    [SerializeField] ParticleSystem deadParticlePrefab;
    [SerializeField] ParticleSystem deadParticlePrefab2;
    Vector3 forceDirect;
    public bool pushActive = false;
    int collisionCount = 2;
    float forceAmount = 1500;
    float speed = 11;
    void Start()
    {
        currentHealth = health;
        playerParentControl = player.transform.parent.GetComponent<PlayerControl>();
        agent = GetComponent<NavMeshAgent>();
        currentBehaviour = States.followingPlayer;
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetTrigger("walk");
        targetSelectManager.Instance.Add_ChangeObserver(this);
    }
    public void targetChange()
    {
        if (playerParentControl.players.Count > 0)
        {
            player = playerParentControl.players[Random.Range(0, playerParentControl.players.Count)];
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            currentBehaviour = States.idle;
            agent.enabled = false;
        }
    }
    public void changeSpeed()
    {
        speed = 19;
        agent.acceleration = 19;
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentBehaviour)
        {

            case States.followingPlayer:
                {
                    if (agent.enabled && player != null)
                    {
                        move2();
                    }
                }
                break;
            case States.attack:
                {
                }
                break;
            case States.dead:
                {

                    deadDest();
                }
                break;

            case States.idle:
                {
                    anim.SetTrigger("idle");
                    //agent.SetDestination(transform.position);
                }
                break;
        }
    }
    void deadDest()
    {
    
        GetComponent<Collider>().enabled = false;
        counter += Time.deltaTime;
        if(counter >= 0.5f)
        {
            var deadPart = Instantiate(deadParticlePrefab2,transform.GetChild(1).GetChild(0).transform.position, Quaternion.identity);
            deadPart.transform.localScale *= 3;

            Destroy(gameObject);
        }
    }
    public void thisSelected()
    {
        StartCoroutine(aliveCheck());
    }
    public void dead(int damage)
    {
        playerParentControl.killCheck();
        StartCoroutine(aliveCheck());
        currentHealth -= damage;
        if (!deadActive && currentHealth <= 0)
        {
            Globals.killedEnemy++;

            targetSelectManager.Instance.Remove_ChangeObserver(this);

            //deadParticlePrefab.Play();
            ZombiSpawner.Instance.enemyAll.Remove(gameObject);
            //GetComponent<Ragdoll>().RagdollActivate(true);
            enemyMesh.material = deadMaterial;
            anim.SetTrigger("fall");
            currentBehaviour = States.dead;
            gameObject.layer = LayerMask.GetMask("Default");
            agent.enabled = false;
            deadActive = true;
            for (int i = 0; i < coinCount; i++)
            {
                Instantiate(coinPrefab, transform.position + new Vector3(Random.Range(-0.5f,0.5f), 7, Random.Range(-0.5f, 0.5f)), Quaternion.identity);
            }
            if (eye != null)
            {
                eye.visibleTargets.Clear();
            }
        }
    }
    public void deadRagdoll(int damage,bool active, Vector3 forceDir)
    {
        StartCoroutine(aliveCheck());
        currentHealth -= damage;
        if (!deadActive && currentHealth <= 0)
        {
            Globals.killedEnemy++;

            targetSelectManager.Instance.Remove_ChangeObserver(this);

            //deadParticlePrefab.Play();

            ZombiSpawner.Instance.enemyAll.Remove(gameObject);

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            agent.enabled = false;
            enemyMesh.material = deadMaterial;
            GetComponent<Ragdoll>().RagdollActivateWithForce(active, forceDir);
                currentBehaviour = States.dead;
            gameObject.layer = LayerMask.GetMask("Default");
            agent.enabled = false;
            deadActive = true;
            for (int i = 0; i < coinCount; i++)
            {
                Instantiate(coinPrefab, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 7, Random.Range(-0.5f, 0.5f)), Quaternion.identity);
            }
            if (eye != null)
            {
                eye.visibleTargets.Clear();
            }
        }
    }
    IEnumerator aliveCheck()
    {
        counterActive = false;
        yield return new WaitForSeconds(0.1f);
        counterActive = true;

        float counter = 0f;
        while(counter < 0.5f && counterActive)
        {
            if (!counterActive)
            {
                break;
            }
            counter += Time.deltaTime;
            yield return null;
            if (!counterActive)
            {
                break;
            }
        }
        if (counterActive)
        {
            isSelected = false;
        }
    }
    void move2()
    {
       
     

        //if (Vector3.Distance(player.transform.position, transform.position) > 100)
        //{
        //    agent.speed = 20;
        //}
        //else
        //{
        //    agent.speed = 12;

        //}

        //transform.Translate(transform.forward * Time.deltaTime * walkSpeed);
        if (Vector3.Distance(player.transform.position, transform.position) > 5)
        {
            followingRotation(player.transform.position);
            anim.SetBool("hit", false);
            agent.SetDestination(player.transform.position);

            //rigidbody.velocity = (transform.forward * walkSpeed);
        }
        else
        {
            anim.SetBool("hit", true);
            //currentBehaviour = States.attack;
        }

        if (Vector3.Distance(player.transform.position, transform.position) > 150)
        {
            targetSelectManager.Instance.Remove_ChangeObserver(this);

            ZombiSpawner.Instance.enemyAll.Remove(gameObject);
            Destroy(gameObject);
        }
        if (Vector3.Distance(player.transform.position, transform.position) > 50)
        {
            agent.speed = 6;
        }
        else
        {
            agent.speed = speed;
        }
    }
    public void hit()
    {
        //player.transform.GetChild(0).GetComponent<Ragdoll>().RagdollActivate(true);
        //player.transform.parent.GetComponent<PlayerControl>().players.Remove(player);
      
    }

    private void followingRotation(Vector3 target)
    {

        Vector3 relativeVector = transform.InverseTransformPoint(target);
        relativeVector /= relativeVector.magnitude;
        float newSteer = (relativeVector.x / relativeVector.magnitude);
        transform.Rotate(0, newSteer * Time.deltaTime * 50 * rotSpeed, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<IEvolution>() != null)
        {
            //GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(colliderSet());
            collision.transform.GetComponent<IEvolution>().playerDead(damage);

            //GameObject deadPlayer = player;

            targetSelectManager.Instance.Notify_ChangeObservers();
            Vector3 forceDirection = (transform.position - collision.transform.position).normalized;
            forceDirect = forceDirection;
            StartCoroutine(_shotgunForce(forceDirection));
            StartCoroutine(collisionEnemy(forceDirection));

            //if ((collision.transform.localEulerAngles.y >= 315 || collision.transform.localEulerAngles.y < 45) && transform.position.z > collision.transform.position.z  )
            //{
            //    StartCoroutine(collisionEnemy(forceDirection));
            //}
            //if ((collision.transform.localEulerAngles.y >= 45 && collision.transform.localEulerAngles.y < 135) && transform.position.x > collision.transform.position.x)
            //{
            //    StartCoroutine(collisionEnemy(forceDirection));
            //}
            //if ((collision.transform.localEulerAngles.y >= 135 && collision.transform.localEulerAngles.y < 225) && transform.position.z < collision.transform.position.z)
            //{
            //    StartCoroutine(collisionEnemy(forceDirection));
            //}
            //if ((collision.transform.localEulerAngles.y >= 225 && collision.transform.localEulerAngles.y < 315) && transform.position.x < collision.transform.position.x)
            //{
            //    StartCoroutine(collisionEnemy(forceDirection));
            //}
            //Destroy(deadPlayer, 3);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<IEvolution>() != null)
        {
            agent.radius = 0.1f;
        }
        //if (other.transform.GetComponent<Zombie>() != null && pushActive && collisionCount>0)
        //{
        //    other.transform.GetComponent<Zombie>().otherCollision(other.transform.position - transform.position, collisionCount-1, forceAmount/2);

        //}
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<IEvolution>() != null)
        {
            agent.radius = 0.9f;
        }
    }
    IEnumerator colliderSet()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = true;
    }
    public void shotgunForce(Vector3 forceDirection)
    {
        StartCoroutine(_shotgunForce(forceDirection));
    }
    IEnumerator _shotgunForce(Vector3 forceDirection)
    {
        yield return null;
        if (!deadActive)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(forceDirection * 5000);
            yield return new WaitForSeconds(1f);
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    public void otherCollision( Vector3 _forceDirect, int collisionCountDown, float _forceAmount)
    {
        forceAmount = _forceAmount;
        collisionCount = collisionCountDown;
        StartCoroutine(collisionEnemy(_forceDirect));

    }
    IEnumerator collisionEnemy(Vector3 forceDirection)
    {
        forceDirect = forceDirection;
        yield return null;
        if (!deadActive)
        {
            pushActive = true;
          

            currentBehaviour = States.attack;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(forceDirection * forceAmount);
            GetComponent<CapsuleCollider>().isTrigger = true;
            //agent.enabled = false;

            yield return null;
    
            pushActive = false;

            yield return new WaitForSeconds(2f);
            collisionCount = 2;
            forceAmount = 1500;
            targetSelectManager.Instance.Notify_ChangeObservers();
            yield return new WaitForSeconds(0.1f);
            GetComponent<CapsuleCollider>().isTrigger = false;
            //agent.enabled = true;
            GetComponent<Rigidbody>().isKinematic = true;

            currentBehaviour = States.followingPlayer;

        }
    }
}
