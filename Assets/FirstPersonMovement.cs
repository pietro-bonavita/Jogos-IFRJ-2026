using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    
    // Componentes
    private CharacterController controller;
    
    // Movimento
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    
    // Input
    private float horizontalInput;
    private float verticalInput;
    private bool isRunning;
    private bool jumpPressed;
    
    void Start()
    {
        // Pegar o componente CharacterController
        controller = GetComponent<CharacterController>();
        
        // Verificar se o CharacterController foi encontrado
        if (controller == null)
        {
            Debug.LogError("CharacterController não encontrado! Adicione um componente CharacterController ao GameObject.");
        }
        
        // Configurar o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Se não tiver câmera atribuída, tenta encontrar
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }
        }
    }
    
    void Update()
    {
        // Pegar inputs
        GetInput();
        
        // Verificar se está no chão
        CheckGround();
        
        // Aplicar gravidade
        ApplyGravity();
        
        // Movimentação
        HandleMovement();
        
        // Pulo
        HandleJump();
        
        // Rotação da câmera
        HandleCameraRotation();
    }
    
    void GetInput()
    {
        // Input de movimento
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        // Input de corrida (Shift esquerdo)
        isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // Input de pulo (Espaço)
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }
    
    void CheckGround()
    {
        // Verificar se está no chão usando um raycast
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Resetar velocidade vertical se estiver no chão e caindo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeno valor para manter o personagem grudado no chão
        }
    }
    
    void ApplyGravity()
    {
        // Aplicar gravidade
        velocity.y += gravity * Time.deltaTime;
        
        // Aplicar a velocidade ao CharacterController
        controller.Move(velocity * Time.deltaTime);
    }
    
    void HandleMovement()
    {
        // Calcular direção do movimento baseada na rotação do player
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        
        // Mover o personagem
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
    
    void HandleJump()
    {
        // Pular se estiver no chão e pressionou o botão de pulo
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpPressed = false;
        }
        
        // Resetar o input de pulo se não estiver no chão
        if (!isGrounded)
        {
            jumpPressed = false;
        }
    }
    
    void HandleCameraRotation()
    {
        // Input do mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotação horizontal (Yaw)
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotação vertical (Pitch) - limitada
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        // Aplicar rotação à câmera
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
    
    // Método para debug - mostra o ground check no editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}