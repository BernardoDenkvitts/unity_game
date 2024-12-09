﻿using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    
    // Define a distancia que o player percorrera horizontalmente enquanto salta
    public float jumpLength;
    
    // Altura maxima que o player consegue pular
    public float jumpHeight;
    public float slideLength;

    // Usado para calcular a distancia percorrida no salto
    private float jumpStart;
    private float slideStart;
    private float minSpeed = 10f;
    private float maxSpeed = 30f;
    private int maxLife = 3;
    private int currLife;
    private bool invincible = false;
    public float invincibleTime;
    public GameObject model;
    
    private Animator animator;
    private Rigidbody rb;
    private BoxCollider boxCollider;

    private int currentLane = 1;
    private Vector3 verticalTargetPosition;
    private Vector3 boxColliderSize;
    public float laneSpeed;
    
    private bool jumping = false;
    private bool sliding = false;
    
    private UIManager uiManager;
    private int coins;
    private float score;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        currLife = maxLife;
        speed = minSpeed;
        uiManager = FindObjectOfType<UIManager>();
        
        // Necessario por causa da forma que as animacoes foram feitas
        animator.Play("runStart");
    }
    
    void Update()
    {
        score += Time.deltaTime * speed;
        uiManager.UpdateScore((int)score);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }

        if (jumping)
        {
            float proporcao = calculaProporcaoMovimento(jumpStart, jumpLength);
            if (proporcao >= 1f)
            {
                jumping = false;
                animator.SetBool("Jumping",  false);
            }
            else
            {
                // Faz com que o movimento do salto siga uma curva
                verticalTargetPosition.y = Mathf.Sin(proporcao * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            // Coloca o player de volta no chao
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
        }

        if (sliding)
        {
            if (calculaProporcaoMovimento(slideStart, slideLength) >= 1f)
            {
                animator.SetBool("Sliding", false);
                sliding = false;
                boxCollider.size = boxColliderSize;
            }
        }
        
        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }
    
    // Controla proporcao do movimento (pulo ou slide)
    // a proporcao varia de 0 a 1 e representa o progresso do movimento
    // se for 1 encerramos o movimento
    private float calculaProporcaoMovimento(float inicioMovimento, float tamanhoMovimento)
    {
        return (transform.position.z - inicioMovimento) / tamanhoMovimento;
    }
    
    // Faz o player andar para frente infinitamente
    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
    }
    
    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < 0 || targetLane > 2)
            return;
        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0);
    }

    void Jump()
    {
        if (!jumping)
        {
            jumpStart = transform.position.z;
            float velocidadePulo = speed / jumpLength;
            animator.SetFloat("JumpSpeed", velocidadePulo);
            animator.SetBool("Jumping", true);
            jumping = true;
        }
    }

    void Slide()
    {
        if (!jumping && !sliding)
        {
            slideStart = transform.position.z;
            // o multiplicador do slide do animator eh o mesmo do jumpspeed
            animator.SetFloat("JumpSpeed", speed / slideLength);
            animator.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y / 2;
            boxCollider.size = newSize;
            sliding = true;
        }
    }
    
    // Determina se o player atingiu um obstaculo ou pegou uma moeda
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("moeda"))
        {
            coins++;
            uiManager.UpdateCoins(coins);
            // desativa a moeda
            other.gameObject.SetActive(false);
        }
        
        if (invincible) 
            return;
        
        if (other.CompareTag("Obstacle"))
        {
            currLife--;
            uiManager.UpdateLives(currLife);
            animator.SetTrigger("Hit");
            speed = 0;
            if (currLife <= 0)
            {
                speed = 0;
                animator.SetBool("Dead", true);
                uiManager.gameOverPanel.SetActive(true);
                Invoke("CallMenu", 2f);
            }
            else
            {
                StartCoroutine(Blinking(invincibleTime));
            }
        }
    }

    // Coroutine que gerencia o efeito de "piscar" do player quando ele atinge um obstaculo
    // utiliza courotine para que o efeito aconteça ao longo do tempo sem interromper
    // o jogo
    IEnumerator Blinking(float time)
    {
        invincible = true;
        float timer = 0;
        // Controla o estado atual do "piscar"
        float currentBlink = 1f;
        float lastBlink = 0;
        // Efeito de piscar é ativado a cada 0,1 segundos
        float blinkPeriod = 0.1f;
        // Indica se o player está visível ou não
        bool enabled = false;
        
        // Espera 1 segundo antes de iniciar o efeito de "piscar"
        // Essa espera é necessário por causa da animação que ocorre quando o player bate
        yield return new WaitForSeconds(1f);
        
        speed = minSpeed;
        
        // Loop de piscar
        while(timer < time && invincible)
        {
            // Ativa ou desativa o player (faz ele piscar)
            model.SetActive(enabled);
            
            // Aguarda até o próximo frame antes de continuar a execução do loop
            // Sem esse return o efeito de piscar nao funciona, serve para sincroniza o efeito de piscar com os frames do jogo
            yield return null;
            
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            
            // Verifica se ja passou 0,1 segundos
            // Se sim, troca o estado de piscada
            if(blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                // Altera entre 0.0 e 1.0
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }
        
        // Deixa o player visivel quando o efeito de piscar acaba
        model.SetActive(true);
        invincible = false;
    }

    void CallMenu()
    {
        GameManager.gm.EndRun();
    }

    public void IncreaseSpeed()
    {
        speed *= 1.30f;
        if(speed >= maxSpeed)
        {
            speed = maxSpeed;
        }
    }
	
}