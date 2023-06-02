using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerPosition;
    [SerializeField]
    private int velocidad = 2;
    private Rigidbody2D rigidbody2D;
    private enum estadosEnemigo { idle, perseguir, golpear};
    private estadosEnemigo m_EnemyEstados;
    private Animator m_Animator;
    private float m_StateDeltaTime;
    private bool viendo;
    private bool canAttack;
    private Coroutine perseguirC;
    private Coroutine golpear;
    private bool golpeable;
    [SerializeField]
    private GameEvent GameEvent;

    private int vidas = 5;
    // Start is called before the first frame update
    void Start()
    {
        golpeable = true;
        viendo= false;
        canAttack= false;
        m_Animator = GetComponent<Animator>();
        m_EnemyEstados = estadosEnemigo.idle;
        playerPosition = FindObjectOfType<CharacterMovement>().transform;
        Vector2 si = playerPosition.position - transform.position;
        if (si.x > 0)
            transform.localEulerAngles = new Vector3(0,0,0);
        else
            transform.localEulerAngles = new Vector3(0, -180, 0);

        rigidbody2D= GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = transform.right * velocidad;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState(m_EnemyEstados);
    }

    private void ChangeState(estadosEnemigo newState)
    {
        if (newState == m_EnemyEstados)
            return;

        ExitState(m_EnemyEstados);
        InitState(newState);
    }

    private void InitState(estadosEnemigo initState)
    {
        m_EnemyEstados = initState;
        m_StateDeltaTime = 0;

        switch (m_EnemyEstados)
        {
            case estadosEnemigo.idle:
                m_Animator.Play("idleEnemigo");
                break;
            case estadosEnemigo.perseguir:
                m_Animator.Play("idleEnemigo");
                perseguirC = StartCoroutine(perseguir());
                break;
            case estadosEnemigo.golpear:
                m_Animator.Play("GolpeEnemigo");
                rigidbody2D.velocity = Vector2.zero;
                break;
            default:
                break;
        }
    }


    private void UpdateState(estadosEnemigo updateState)
    {
        m_StateDeltaTime += Time.deltaTime;

        switch (updateState)
        {
            case estadosEnemigo.idle:
                if (viendo)
                    ChangeState(estadosEnemigo.perseguir);
                break;
            case estadosEnemigo.perseguir:
                if (!viendo)
                    ChangeState(estadosEnemigo.idle);
                if (canAttack)
                    ChangeState(estadosEnemigo.golpear);
                break;
            case estadosEnemigo.golpear:
                if (m_StateDeltaTime > 1)
                    ChangeState(estadosEnemigo.perseguir);
                break;
            default:
                break;
        }
    }

    private void ExitState(estadosEnemigo exitState)
    {
        switch (exitState)
        {
            case estadosEnemigo.idle:
                break;
            case estadosEnemigo.perseguir:
                StopCoroutine(perseguirC);
                break;
            case estadosEnemigo.golpear:
                break;
            default:
                break;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Jugador"))
        {
            if (viendo && !canAttack)
                canAttack = true;
            if (!viendo)
                viendo = true;
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer("hacha"))
        {
            if (golpeable)
            {
                vidas--;
                if (vidas == 0)
                {
                    this.GameEvent.Raise();
                    Destroy(this.gameObject);
                }
                StartCoroutine("noGolpeable");
            }
           
        }
       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (canAttack && viendo)
            canAttack = false;
        if (viendo && !canAttack)
            viendo = false;
    }
    private IEnumerator perseguir()
    {
        while(m_EnemyEstados == estadosEnemigo.perseguir)
        {
            Vector2 si = playerPosition.position - transform.position;
            if (si.x > 0)
                transform.localEulerAngles = new Vector3(0, 0, 0);
            else
                transform.localEulerAngles = new Vector3(0, -180, 0);
            rigidbody2D.velocity = (playerPosition.position - transform.position).normalized * velocidad;
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator noGolpeable()
    {
       golpeable= false;
       yield return new WaitForSeconds(0.6f);
       golpeable = true;
    }

}
