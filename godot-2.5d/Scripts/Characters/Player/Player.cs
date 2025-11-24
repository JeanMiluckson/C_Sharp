using Godot;
using System;
using System.Threading.Tasks; // Necessário para Task.Delay e async/await

public partial class Player : CharacterBody3D
{
    // ======== PARÂMETROS EXPORTADOS (Ajuste no editor Godot) ========

    [Export] public float Speed = 4.0f;                 // Velocidade de movimento
    [Export] public float MouseSensitivity = 0.2f;      // Sensibilidade do mouse
    [Export] public float JumpVelocity = 5.0f;          // Força do pulo
    [Export] public float RotationSmoothness = 10.0f;   // Suavidade da rotação do modelo visual
    [Export] public float MinPitch = -20f;              // Limite inferior da câmera
    [Export] public float MaxPitch = 10f;               // Limite superior da câmera

    [Export] public Node3D Pivot;                       // Nó que controla a rotação vertical da câmera
    [Export] public Node3D CharacterModel;              // Modelo visual do personagem
    [Export] public float Gravity = 0;                  // (Usado o valor do ProjectSettings se for 0)

    // ======== VARIÁVEIS PRIVADAS DE ESTADO ========
    
    private bool _knockbacked = false; 
    private float _gravityOverride = 0.0f;              // Controla o valor da gravidade customizada (usado em Pulo/Knockback)
    private bool _usingGravityOverride = false;          // Indica se a física customizada está ativa

    private Camera3D camera;
    private AnimationPlayer animator;
    private float gravityForce;                         // Gravidade padrão do Godot
    private float yaw = 0f;                             // Rotação horizontal da câmera
    private float pitch = 0f;                           // Rotação vertical da câmera

    // ======== MÉTODOS BASE DA GODOT ========

    public override void _Ready()
    {
        // Garante que todos os nós estão referenciados
        CharacterModel = GetNode<Node3D>("gdbot");
        animator = CharacterModel.GetNode<AnimationPlayer>("AnimationPlayer");
        Pivot = GetNode<Node3D>("Pivot");
        camera = Pivot.GetNode<Camera3D>("Camera3D");

        // Configurações iniciais
        Input.MouseMode = Input.MouseModeEnum.Captured;
        gravityForce = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


    
    }

    public override void _PhysicsProcess(double delta)
    {
    
        HandleMovement((float)delta);
        
        // 2. Aplica a gravidade e o pulo (lógica customizada)
        ApplyCustomPhysics((float)delta);
        
        // 3. Atualiza as animações
        HandleAnimation();
        
        // 4. Aplica o movimento físico
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        // === CONTROLE DO MOUSE/CÂMERA ===
        if (@event is InputEventMouseMotion motion)
        {
            yaw -= motion.Relative.X * MouseSensitivity * 0.01f;
            pitch += motion.Relative.Y * MouseSensitivity * 0.01f;

            pitch = Mathf.Clamp(pitch, Mathf.DegToRad(MinPitch), Mathf.DegToRad(MaxPitch));
            
            // Aplica rotação ao Pivot (câmera)
            Pivot.Rotation = new Vector3(pitch, yaw, 0);
        }

        // === CONTROLE DA JANELA ===
        if (@event.IsActionPressed("esc"))
            Input.MouseMode = Input.MouseModeEnum.Visible;

        if (@event is InputEventMouseButton && Input.MouseMode != Input.MouseModeEnum.Captured)
            Input.MouseMode = Input.MouseModeEnum.Captured;

        // === ZOOM DA CÂMERA ===
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                camera.Fov = Mathf.Max(camera.Fov - 2f, 60f); 
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                camera.Fov = Mathf.Min(camera.Fov + 2f, 90f); 
        }
    }

    // ======== LÓGICA DE JOGABILIDADE ========

    private void HandleMovement(float delta)
    {
        Vector3 input = Vector3.Zero;
        input.X = Input.GetAxis(GConstants.MOVE_LEFT, GConstants.MOVE_RIGHT);
        input.Z = Input.GetAxis(GConstants.MOVE_BACKWARD, GConstants.MOVE_FORWARD);

        if (input.LengthSquared() > 1.0f)
            input = input.Normalized();

        // Vetores de orientação da câmera
        Vector3 camFwd = Pivot.GlobalTransform.Basis.Z;
        Vector3 camRight = Pivot.GlobalTransform.Basis.X;
        camFwd.Y = 0;
        camRight.Y = 0;
        camFwd = camFwd.Normalized();
        camRight = camRight.Normalized();

        // Calcula a direção de movimento
        Vector3 moveDir = (-camFwd * input.Z) + (camRight * input.X);
        moveDir = moveDir.Normalized();

        // Aplica o movimento
        Vector3 vel = Velocity;
        vel.X = moveDir.X * Speed;
        vel.Z = moveDir.Z * Speed;
        Velocity = vel;

        // Gira o modelo visual do personagem na direção do movimento
        if (moveDir.LengthSquared() > 0.01f)
        {
            float targetYaw = Mathf.Atan2(moveDir.X, moveDir.Z);
            float newYaw = Mathf.LerpAngle(CharacterModel.Rotation.Y, targetYaw, delta * RotationSmoothness);
            CharacterModel.Rotation = new Vector3(0, newYaw, 0);
        }
    }

    private void ApplyCustomPhysics(float delta)
    {
        var vel = Velocity;
        
        // === Lógica do PULO (Input) ===
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            // Ativa o modo de gravidade customizada do GDScript para o pulo
            _gravityOverride = -JumpVelocity;
            _usingGravityOverride = true; 
            vel.Y = JumpVelocity;
        }

        // === Lógica de Aplicação da Gravidade ===
        if (!_usingGravityOverride)
        {
            // Usa a gravidade padrão do Godot
            if (!IsOnFloor())
                vel.Y -= gravityForce * delta;
            else if (vel.Y < 0)
                vel.Y = -0.1f;
        }
        else // Se _usingGravityOverride for TRUE (Pulo )
        {
            // Implementa a lógica: gravity += 25 * delta
            if (!IsOnFloor())
            {
                _gravityOverride += gravityForce * delta;
            }
            
            // Aplica a gravidade customizada (vel.Y = -_gravityOverride)
            vel.Y = -_gravityOverride;
            
            // Reseta a gravidade customizada ao tocar o chão (lógica do jump/apply_gravity)
            if (_gravityOverride > 0 && IsOnFloor())
            {
                _gravityOverride = 0.0f;
                _usingGravityOverride = false; 
            }
        }
        
        Velocity = vel;
    }

    private void HandleAnimation()
    {
        // Se o jogador estiver no ar
        if (!IsOnFloor())
        {
            if (Velocity.Y > 0 && animator.CurrentAnimation != "jump")
                animator.Play("jump", 0.3);
            
            else if (Velocity.Y < 0 && animator.CurrentAnimation != "fall")
                animator.Play("fall", 0.3);

            return;
        }

        // Se estiver no chão
        if (Mathf.Abs(Velocity.X) > 0.1f || Mathf.Abs(Velocity.Z) > 0.1f)
            animator.Play("run", 0.3);

        else
            animator.Play("Idle", 0.3);
    }
  
}