using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody2D m_RigidBody;
    [SerializeField]
    float m_Speed = 4;
    private enum estados { idle, golpe1, golpe2, golpe3};
    private estados m_EstadosPlayer;
    private float m_StateDeltaTime;
    private Animator m_Animator;
    private bool combo;
    private Coroutine m_ComboTimeCoroutine;
    [SerializeField]
    private PlayerVidas m_PlayerVidas;
    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_EstadosPlayer = estados.idle;
        m_PlayerVidas.vidas = 3;
        m_PlayerVidas.HP = 3;
    }

    void Update()
    {
        UpdateState(m_EstadosPlayer);
        Vector3 movement = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
        {
            movement += transform.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
        }

        if (m_EstadosPlayer == estados.idle)
            m_RigidBody.velocity = movement.normalized * m_Speed;
        else
            m_RigidBody.velocity = Vector2.zero;

        //rotem segons on estem anant
        if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.up*180);
        }
        else if(movement.x > 0)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void ChangeState(estados newState)
    {
        if (newState == m_EstadosPlayer)
            return;

        ExitState(m_EstadosPlayer);
        InitState(newState);
    }

    private void InitState(estados initState)
    {
        m_EstadosPlayer = initState;
        m_StateDeltaTime = 0;

        switch (m_EstadosPlayer)
        {
            case estados.idle:
                m_Animator.Play("idle");
                break;
            case estados.golpe1:
                m_Animator.Play("Golpe1");
                m_ComboTimeCoroutine = StartCoroutine(CorutineCombo(0.65f, 1.1f));
                break;
            case estados.golpe2:
                m_Animator.Play("Golpe2");
                m_ComboTimeCoroutine = StartCoroutine(CorutineCombo(0.65f, 1.1f));
                break;
            case estados.golpe3:
                m_Animator.Play("Golpe3");
                break;
            default:
                break;
        }
    }


    private void UpdateState(estados updateState)
    {
        m_StateDeltaTime += Time.deltaTime;

        switch (updateState)
        {
            case estados.idle:
                if (Input.GetKey(KeyCode.Space))
                    ChangeState(estados.golpe1);
                break;
            case estados.golpe1:
                if (Input.GetKey(KeyCode.Space) && combo)
                    ChangeState(estados.golpe2);
                if (m_StateDeltaTime > 1.5)
                    ChangeState(estados.idle);
                break;
            case estados.golpe2:
                if (Input.GetKey(KeyCode.Space) && combo)
                    ChangeState(estados.golpe3);
                if(m_StateDeltaTime > 1.5)
                    ChangeState(estados.idle);
                break;
            case estados.golpe3:
                if (m_StateDeltaTime > 1.5)
                    ChangeState(estados.idle);
                break;
            default:
                break;
        }
    }

    private void ExitState(estados exitState)
    {
        switch (exitState)
        {
            case estados.idle:
                break;
            case estados.golpe1:
                StopCoroutine(m_ComboTimeCoroutine);
                break;
            case estados.golpe2:
                StopCoroutine(m_ComboTimeCoroutine);
                break;
            case estados.golpe3:
                break;
            default:
                break;
        }

    }
    private IEnumerator CorutineCombo(float comboWindowStart, float comboWindowEnd)
    {
        combo = false;
        yield return new WaitForSeconds(comboWindowStart);
        combo = true;
        yield return new WaitForSeconds(comboWindowEnd);
        combo = false;
    }



}
