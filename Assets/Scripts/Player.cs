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
    
    private Animator animator;
    private Rigidbody rb;
    private BoxCollider boxCollider;

    private int currentLane = 1;
    private Vector3 verticalTargetPosition;
    private Vector3 boxColliderSize;
    public float laneSpeed;
    
    private bool jumping = false;
    private bool sliding = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        // Necessario por causa da forma que as animacoes foram feitas
        animator.Play("runStart");
    }
    
    void Update()
    {
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
            // Controla proporcao do pulo
            float proporcao = (transform.position.z - jumpStart) / jumpLength;
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
            float proporcao = (transform.position.z - slideStart) / slideLength;
            if (proporcao >= 1f)
            {
                animator.SetBool("Sliding", false);
                sliding = false;
                boxCollider.size = boxColliderSize;
            }
        }
        
        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
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
}