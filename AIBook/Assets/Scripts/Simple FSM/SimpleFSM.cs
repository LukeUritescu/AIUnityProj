using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleFSM : FSM
{
public enum FSMState { None, Patrol, Chase, Attack, Dead}
    //Current state that NPC is reaching 
    public FSMState curState;

    //Speed of the tank
    private float curSpeed;

    //Tank Rotation Speed
    private float curRotSpeed;

    //Bullet
    [SerializeField]
    private GameObject Bullet;

    //Whether the NPC is destroyed or not
    private bool bDead;
    private int health;

    //We overwrite the deprecated built-in 'rigidbody' veriable.
    new private Rigidbody rigidBody;

    protected override void Initialize()
    {
        curState = FSMState.Patrol;
        curSpeed = 150.0f;
        curRotSpeed = 2.0f;
        bDead = false;
        elapsedTime = 0.0f;
        shootRate = 3.0f;
        health = 100;

        //Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        //Set Raandom Destination point first
        FindNextPoint();

        //Get target enemy(player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        //Get the rigidbody
        rigidBody = GetComponent<Rigidbody>();


        if (!playerTransform)
            print("Player doesn't exist..Please add one" + "with Tag Named Player");

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;
    }

    //Update each frame
    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //Update the time

        elapsedTime += Time.deltaTime;

        //Go to dead state is no health hleft
        if (health <= 0)
            curState = FSMState.Dead;
    }

    protected void UpdatePatrolState()
    {
        //Find anotther random patrol point if the current point is reached
        if (Vector3.Distance(transform.position, destPos) <= 100.0f)
        {
            print("Reached to the destination point\n" + "calculating the next point");

            FindNextPoint();
        }

        //Check distance with player tank When distance is near, transtion to chase state
        else if (Vector3.Distance(transform.position, playerTransform.position) <= 300.0f)
        {
            print("Switch to Chase Position");
            curState = FSMState.Chase;
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);


        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        //Go Forward
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    protected void FindNextPoint()
    {
        print("finding next point");
        int rndIndex = Random.Range(0, pointList.Length);
        float rndRadius = 10.0f;
        Vector3 rndPosition = Vector3.zero;
        destPos = pointList[rndIndex].transform.position + rndPosition;

        //Cheeck rrange to decide random point as the same as before
        if (IsInCurrentRange(destPos))
        {
            rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0.0f, Random.Range(-rndRadius, rndRadius));
            destPos = pointList[rndIndex].transform.position + rndPosition;
        }
    }

    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50)
            return true;

        return false;
    }

    protected void UpdateChaseState()
    {
        //Set the target positionnn as the player position
        destPos = playerTransform.position;

        //Check distance with player tank when distaance is near, transition to attack state
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= 200.0f)
        {
            curState = FSMState.Attack;
        }
        //Go Bck to patrol if it becomes too far
        else if (dist>= 300.0f)
        {
            curState = FSMState.Patrol;
        }

        //Go Forward
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    protected void UpdateAttackState()
    {
        //set the target position as the player position
        destPos = playerTransform.position;

        //check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist >= 200.0f && dist < 300.0f)
        {
            //Rotate target point
            Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forrward
            transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            curState = FSMState.Attack;
        }

        //Transitionn to patrol is the tank become too far
        else if (dist >= 300.0f)
        {
            curState = FSMState.Patrol;
        }

        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);

        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        //Shoot the bullets
        ShootBullet();
    }
    private void ShootBullet()
    {
        if(elapsedTime >= shootRate)
        {
            //Shoot the bullet
            Instantiate(Bullet, bulletSpawnPoint.position,
                bulletSpawnPoint.rotation);
            elapsedTime = 0.0f;
        }
    }

    protected void UpdateDeadState()
    {
        //show the dead animation with some phsics effects
        if (!bDead)
        {
            bDead = true;
            Explode();
        }
    }

    protected void Explode()
    {
        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for(int i = 0; i <  3; i++) 
            {
            rigidBody.AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f,
                rndZ), 40.0f, 10.0f);
            rigidBody.velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
            }

        Destroy(gameObject, 1.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if(collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent
                <Bullet>().damage;
        }
    }
}
