using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterMovement : MonoBehaviour
{
    Vector3 screenPoint;
    Vector3 offset;
    Vector3 realPoint;
    Vector3 realPosition;
    Vector3 oldPos;

    public float MoveRange = 4.5f;
    public float Speed = 5f;
    public float Swipe1;
    public float Swipe2;
    public float SwipeZ1;
    public float SwipeZ2;
    public float Rail1Position;
    public float Rail2Position;
    public float Fly1Position;
    public float Fly2Position;
    public ParticleSystem Wind;


    RaycastHit crashed;
    public GameObject RailFinished;
    public GameObject RailFinished2;
    public GameObject FlyFinished;
    public GameObject FlyFinished2;
    public GameObject Empty;
    public Animator PlayerAnimator;
    public string PositionNow;
    public string OnThe;
    bool JustOne = false;
    public float Size;
    public float RotationSize;
    public GameObject Shield;
 
    private Quaternion _targetRotation = Quaternion.identity;

    public float turningRate = 3f;
    private void Start()
    {
        PositionNow = "Running";
        OnThe = "Ground";
        Size = Screen.height / 4;
        RotationSize = 90f / Screen.width;


        Shield = GameObject.Find("Shield");

    }
    private void FixedUpdate()
    {
        if (PositionNow != "Sleeping")
        {
            
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turningRate * Time.deltaTime);
            

            GameObject.FindGameObjectWithTag("StickMan").transform.rotation = Quaternion.RotateTowards(GameObject.FindGameObjectWithTag("StickMan").transform.rotation, _targetRotation, turningRate * Time.deltaTime);
            
            if (PositionNow=="OnAir")
            {
                 Shield.transform.rotation = Quaternion.RotateTowards(Shield.transform.rotation, Quaternion.Euler(-184,0,90), turningRate * Time.deltaTime);
            }

        }



    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            SwipeZ1 = Input.mousePosition.x;
        }
        if (Input.GetMouseButton(0))
        {

            SwipeZ2 = Input.mousePosition.x;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, transform.position.y + 14.7f, transform.position.z - 16.7f), Time.deltaTime * 10);

        transform.Translate(transform.forward * Time.deltaTime * Speed, Space.World);


        if (PlayerAnimator.GetBool("IsFall") == false && PlayerAnimator.GetBool("IsFallBack") == false)
        {

            if (transform.position.x > -MoveRange && transform.position.x < MoveRange)
            {
                AxisXMovement();
            }

        }

        RayCastControl();
        SleepingOnTheGround();
        RotationControl();
        SmoothMovement();

        if ((PlayerAnimator.GetBool("IsFall") == true) && JustOne == false)
        {
            Speed = 0;
            JustOne = true;

        }
        if (GameManager.Instance.RailObstacle == true && JustOne == false)
        {
            DOTween.KillAll();
            PlayerAnimator.SetBool("IsFallBack", true);
            Speed = 0;
            transform.DOMoveY(transform.position.y - 1f, 1f);
            transform.DOMoveZ(transform.position.z + 7f, 1f);
            JustOne = true;
        }
        if (GameManager.Instance.AirObstacle == true && JustOne == false)
        {
            DOTween.KillAll();
            PlayerAnimator.SetBool("IsFallBack", true);
            Speed = 0;
            PlayerAnimator.SetBool("IsGround", false);
            PlayerAnimator.SetBool("IsSleeping", false);
            PlayerAnimator.SetBool("IsJump", false);
            transform.DOMoveZ(transform.position.z + 15f, 2f);
            transform.DOMoveY(Empty.transform.position.y, 5f);
            JustOne = true;
        }
        if ((PlayerAnimator.GetBool("IsFall") == true) || (PlayerAnimator.GetBool("IsFallBack") == true))
        {
            if (OnThe != "Final")
            {
                GameManager.Instance.GameOver = true;
            }

        }
    }
    void AxisXMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);

            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        }
        if (Input.GetMouseButton(0))
        {

            realPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            realPosition = Camera.main.ScreenToWorldPoint(realPoint) + offset;

            if (realPosition.x > -MoveRange && realPosition.x < MoveRange)
            {

                if (OnThe != "Rail")
                {
                    transform.position = new Vector3(realPosition.x, transform.position.y, transform.position.z);
                }


            }

        }

    }



    void RayCastControl()
    {
        if (Physics.Raycast(transform.position, -transform.up, out crashed, 100f))
        {


        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            OnThe = "Ground";

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RailStart")
        {

            if (PositionNow == "Sleeping")
            {
                if (transform.position.z < Rail1Position)
                {
                    transform.DOMoveY(RailFinished.transform.position.y + 0.4f, 1.9f).SetEase(Ease.Linear);
                }
                else if (transform.position.z < Rail2Position)
                {
                    transform.DOMoveY(RailFinished2.transform.position.y + 0.4f, 1.9f).SetEase(Ease.Linear);
                }
                Speed = 20;
            }
            else
            {

                transform.DOMoveY(crashed.point.y - 0.8f, 0.5f);
                transform.DOMoveZ(transform.position.z + 5f, 1f);
                PlayerAnimator.SetBool("IsFall", true);
                PlayerAnimator.SetBool("IsGround", false);
                PlayerAnimator.SetBool("IsJump", false);
                
            }
            if (PositionNow == "OnAir")
            {
                Shield.transform.DOLocalMoveY(0.5f, 1f);
            }



        }






        if (other.gameObject.tag == "FlyStart")
        {
            if (PositionNow == "OnAir")
            {
                if (transform.position.z < Fly1Position)
                {
                    transform.DOMoveY(FlyFinished.transform.position.y + 0.4f, 7f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        PositionNow = "Running";
                        PlayerAnimator.SetBool("IsJump", false);
                        PlayerAnimator.SetBool("IsGround", true);
                        Shield.transform.parent = GameObject.FindGameObjectWithTag("Hand").transform;
                        Vector3 Runningrot = new Vector3(-141, 90, 67);
                        Shield.transform.DOLocalRotate(Runningrot, 0.5f);
                        Shield.transform.DOLocalMoveX(0.1740731f, 0.25f);
                        Shield.transform.DOLocalMoveY(-0.025245f, 0.25f);
                        Shield.transform.DOLocalMoveZ(0.1538867f, 0.25f);
                        Speed = 20;
                        Wind.Stop();
                    });
                }
                else if (transform.position.z < Fly2Position)
                {
                    transform.DOMoveY(FlyFinished2.transform.position.y + 0.4f, 7f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        PositionNow = "Running";
                        PlayerAnimator.SetBool("IsJump", false);
                        PlayerAnimator.SetBool("IsGround", true);
                        Shield.transform.parent = GameObject.FindGameObjectWithTag("Hand").transform;
                        Vector3 Runningrot = new Vector3(-141, 90, 67);
                        Shield.transform.DOLocalRotate(Runningrot, 0.5f);
                        Shield.transform.DOLocalMoveX(0.1740731f, 0.25f);
                        Shield.transform.DOLocalMoveY(-0.025245f, 0.25f);
                        Shield.transform.DOLocalMoveZ(0.1538867f, 0.25f);
                        Speed = 20;
                        Wind.Stop();
                    });
                }

            }
            else
            {
                PlayerAnimator.SetBool("IsFall", true);
                PlayerAnimator.SetBool("IsGround", false);
                PlayerAnimator.SetBool("IsSleeping", false);
                transform.DOMoveZ(transform.position.z + 15f, 2f);
                transform.DOMoveY(Empty.transform.position.y, 5f);
            }
            OnThe = "Fly";

        }
        if (other.gameObject.tag == "Empty")
        {

            Speed = 0;

        }
        if (other.gameObject.tag == "Bomb")
        {
            other.GetComponentInChildren<ParticleSystem>().Play();
            Speed = 0;
            PlayerAnimator.SetBool("IsFall", true);
            transform.DOMoveY(other.gameObject.transform.position.y - 0.5f, 1f);
            transform.DOMoveZ(transform.position.z + 7f, 1f);
            GameManager.Instance.Win = true;

        }
        if (other.gameObject.tag == "Obstacle")
        {


            PlayerAnimator.SetBool("IsFall", true);
            PlayerAnimator.SetBool("IsGround", false);
            PlayerAnimator.SetBool("IsSleeping", false);
            PlayerAnimator.SetBool("IsJump", false);
            transform.DOMoveZ(transform.position.z + 15f, 2f);
            transform.DOMoveY(Empty.transform.position.y, 5f);


        }

        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Animator>().SetTrigger("EnemyFight");
            if (PositionNow == "Sleeping")
            {
                Speed = 0;
                PlayerAnimator.SetBool("IsFallBack", true);
                transform.DOMoveY(other.gameObject.transform.position.y - 0.5f, 1f);
                transform.DOMoveZ(transform.position.z + 7f, 1f);
            }
            else if (PositionNow == "OnAir")
            {
                Speed = 0;
                PlayerAnimator.SetBool("IsFallBack", true);
                transform.DOMoveY(other.gameObject.transform.position.y - 0.5f, 1f);
                transform.DOMoveZ(transform.position.z + 7f, 1f);
                Shield.transform.DOLocalMoveY(0.5f, 1f);

            }
            else
            {

                PlayerAnimator.SetTrigger("IsFight");
                StartCoroutine("ShakeCoroutine");
                other.gameObject.transform.DOMoveZ(other.gameObject.transform.position.z, 0.3f).OnComplete(() =>
                {
                    other.gameObject.transform.DOMoveY(other.gameObject.transform.position.y + 15f, 2f);
                    other.gameObject.transform.DOMoveZ(other.gameObject.transform.position.z + 50f, 2f);
                    other.gameObject.transform.DOMoveX(other.gameObject.transform.position.x + 30f, 2f);
                });

            }

        }

        if (other.gameObject.tag == "Xcount")
        {
            OnThe = "Final";
            GameManager.Instance.Xcount += 1;
            Destroy(other.gameObject);
            Speed += Speed * 0.15f;
            if (GameManager.Instance.Xcount == 10)
            {

                Speed = 0;
                transform.DOMoveZ(transform.position.z + 8f, 1f);
                PlayerAnimator.SetTrigger("IsDancing");
                Shield.transform.parent = null;
                Shield.transform.DOMoveX(30f, 1f);
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z); ;
                StartCoroutine(FireWorkCoroutine());
                GameManager.Instance.Win = true;

            }
            if (PositionNow == "OnAir")
            {
                Speed = 0;
                PlayerAnimator.SetBool("IsFall", true);
                transform.DOMoveY(other.gameObject.transform.position.y - 0.5f, 1f);
                transform.DOMoveZ(transform.position.z + 7f, 1f);
                Shield.transform.DOLocalMoveY(0.5f, 1f);
            }
        }

        if (other.gameObject.tag == "Coin")
        {
            GameManager.Instance.CoinCount += 1;
            Destroy(other.gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "RailStart")
        {

            OnThe = "Rail";
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -(SwipeZ2 - SwipeZ1) * RotationSize);
        }
    }

    void SleepingOnTheGround()
    {
        if (OnThe == "Ground" && PositionNow == "Sleeping")
        {
            if (Speed > 0)
            {
                Speed -= Time.deltaTime / 4;
            }
            if (Speed <= 0)
            {
                PlayerAnimator.SetBool("IsSleeping", false);
                PlayerAnimator.SetBool("IsFall", true);
            }

        }
    }

    void RotationControl()
    {
        if (OnThe == "Rail")
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -(SwipeZ2 - SwipeZ1) * RotationSize);
            transform.DOMoveX(0.46f, 0.1f);
        }
        if (OnThe == "Ground" && PositionNow != "OnAir")
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);

        }

        if (transform.rotation.z > 0.45 || transform.rotation.z < -0.45)
        {
            DOTween.KillAll();
            PlayerAnimator.SetBool("IsSleeping", false);
            PlayerAnimator.SetBool("IsFall", true);
            transform.DOMoveY(Empty.transform.position.y, 5f);

        }

    }

    public void SwipeUp()
    {

        if (PositionNow == "Sleeping" && OnThe != "Rail")
        {
            PositionNow = "Running";
            PlayerAnimator.SetBool("IsSleeping", false);
            PlayerAnimator.SetBool("IsGround", true);
            PlayerAnimator.SetBool("IsJump", false);
            Speed = 20;

            Shield.transform.parent = GameObject.FindGameObjectWithTag("Hand").transform;
            Vector3 Runningrot = new Vector3(-141, 90, 67);
            Shield.transform.DOLocalRotate(Runningrot, 0.5f);
            Shield.transform.DOLocalMoveX(0.1740731f, 0.25f);
            Shield.transform.DOLocalMoveY(-0.025245f, 0.25f);
            Shield.transform.DOLocalMoveZ(0.1538867f, 0.25f);

        }
        else if (PositionNow == "Sleeping" && OnThe == "Rail")
        {
            DOTween.KillAll();
            PlayerAnimator.SetBool("IsSleeping", false);
            PlayerAnimator.SetBool("IsFall", true);


        }
        else if (PositionNow == "Running")
        {
            transform.DOMoveY(transform.position.y + 5f, 0.3f);
            PositionNow = "OnAir";
            OnThe = "Fly";
            PlayerAnimator.SetBool("IsJump", true);
            PlayerAnimator.SetBool("IsGround", false);
            PlayerAnimator.SetBool("IsSleeping", false);

            Shield.transform.parent = gameObject.transform;
            Vector3 Flyrot = new Vector3(-184, 0, 90);
            Shield.transform.DOLocalRotate(Flyrot, 0.5f);
            Shield.transform.DOLocalMoveY(1.9f, 0.25f);
            Shield.transform.DOLocalMoveX(0f, 0.25f);
            Speed = 15;
            Wind.Play();
        }



    }

    public void SwipeDown()
    {
        if (PositionNow == "OnAir" && OnThe != "Fly")
        {
            PositionNow = "Running";
            PlayerAnimator.SetBool("IsGround", true);
            PlayerAnimator.SetBool("IsJump", false);
            PlayerAnimator.SetBool("IsSleeping", false);

            DOTween.Kill(transform);
            transform.DOMoveY(crashed.point.y + 0.5f, 0.5f);

            Shield.transform.parent = GameObject.FindGameObjectWithTag("Hand").transform;
            Vector3 Runningrot = new Vector3(-141, 90, 67);
            Shield.transform.DOLocalRotate(Runningrot, 0.5f);
            Shield.transform.DOLocalMoveX(0.1740731f, 0.25f);
            Shield.transform.DOLocalMoveY(-0.025245f, 0.25f);
            Shield.transform.DOLocalMoveZ(0.1538867f, 0.25f);

            Speed = 20;
            Wind.Stop();
        }
        else if (PositionNow == "OnAir" && OnThe == "Fly" && crashed.collider.tag == "Empty")
        {

            PlayerAnimator.SetBool("IsJump", false);
            PlayerAnimator.SetBool("IsFall", true);
            DOTween.KillAll();
            transform.DOMoveY(crashed.point.y + 0.5f, 1.5f);


        }
        else if (PositionNow == "OnAir" && OnThe == "Fly" && crashed.collider.tag == "Ground")
        {
            PositionNow = "Running";
            PlayerAnimator.SetBool("IsJump", false);
            PlayerAnimator.SetBool("IsGround", true);
            DOTween.Kill(transform);
            transform.DOMoveY(crashed.point.y + 0.5f, 0.5f);

            Shield.transform.parent = GameObject.FindGameObjectWithTag("Hand").transform;
            Vector3 Runningrot = new Vector3(-141, 90, 67);
            Shield.transform.DOLocalRotate(Runningrot, 0.5f);
            Shield.transform.DOLocalMoveX(0.1740731f, 0.25f);
            Shield.transform.DOLocalMoveY(-0.025245f, 0.25f);
            Shield.transform.DOLocalMoveZ(0.1538867f, 0.25f);
            Debug.Log("3");
            Speed = 20;
            Wind.Stop();
        }
        else if (PositionNow == "Running")
        {
            PositionNow = "Sleeping";
            PlayerAnimator.SetBool("IsSleeping", true);
            PlayerAnimator.SetBool("IsGround", false);
            PlayerAnimator.SetBool("IsJump", false);



            Shield.transform.parent = gameObject.transform;
            Vector3 sleepingrot = new Vector3(0, 0, 90);
            Shield.transform.DORotate(sleepingrot, 0.5f);
            Shield.transform.DOMoveY(transform.position.y, 0.25f);
            Shield.transform.DOLocalMoveX(0f, 0.25f);
            Shield.transform.DOLocalMoveZ(-0.2f, 0.25f);
            Debug.Log("5");

        }
    }

    public void SmoothMovement()
    {

        Vector3 movement = transform.position - oldPos;
        if (movement.x > 0 && Input.GetAxis("Mouse X") > 0)
        {



            if (PositionNow != "Sleeping")
            {
                GameObject.FindGameObjectWithTag("StickMan").transform.rotation = Quaternion.Lerp(GameObject.FindGameObjectWithTag("StickMan").transform.rotation, Quaternion.Euler(0, 150 * Input.GetAxis("Mouse X"), 0), 0.1f);

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -50 * Input.GetAxis("Mouse X")), 0.1f);
                if (PositionNow == "OnAir")
                {
                    Shield.transform.rotation = Quaternion.Lerp(Shield.transform.rotation, Quaternion.Euler(-184, 150 * Input.GetAxis("Mouse X"), 90), 0.1f);
                }
            }
        }
        else if (movement.x < 0 && Input.GetAxis("Mouse X") < 0)
        {



            if (PositionNow != "Sleeping")
            {
                GameObject.FindGameObjectWithTag("StickMan").transform.rotation = Quaternion.Lerp(GameObject.FindGameObjectWithTag("StickMan").transform.rotation, Quaternion.Euler(0, 150 * Input.GetAxis("Mouse X"), 0), 0.1f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -50 * Input.GetAxis("Mouse X")), 0.1f);
                if (PositionNow == "OnAir")
                {
                    Shield.transform.rotation = Quaternion.Lerp(Shield.transform.rotation, Quaternion.Euler(-184, 150 * Input.GetAxis("Mouse X"), 90), 0.1f);
                }


            }

        }

        oldPos = transform.position;
    }


    IEnumerator FireWorkCoroutine()
    {
        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(0).gameObject.SetActive(true);

        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(2).gameObject.SetActive(true);

        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(4).gameObject.SetActive(true);


        yield return new WaitForSeconds(0.5f);

        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(5).gameObject.SetActive(true);

        GameObject.FindGameObjectWithTag("FireWork").transform.GetChild(6).gameObject.SetActive(true);

    }

    IEnumerator ShakeCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        Camera.main.transform.DOShakePosition(0.1f, 1f, 10, 90, false, true);
    }
}
