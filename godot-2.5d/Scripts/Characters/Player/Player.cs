using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] private AnimationPlayer animPlayerNode;

    // ======== CÂMERA ========
    [Export] public Node3D Pivot;       // Pivot que tem a Camera como filho
    [Export] public Camera3D Camera;
    [Export] public float MouseSensitivity = 0.2f;
    [Export] public float MinPitch = -30f;
    [Export] public float MaxPitch = 45f;

    // Offset vertical do pivot em relação ao jogador (altura da câmera)
    [Export] public float PivotHeight = 1.6f;

    // Distância lateral/atrás é controlada pela transform da Camera dentro do Pivot
    // (deixe a Camera no editor com translation, por exemplo (0, 0, 3) e rotacionada).
    // =================================================

    private float yaw = 0f;
    private float pitch = 0f;

    private Vector2 direction = new();
    private float speed = 5f;

    // suavização da rotação do personagem (0 = sem suavização, maior = mais suave)
    [Export] public float RotationSmoothness = 12f;

    public override void _Ready()
    {
        animPlayerNode.Play(GConstants.ANIM_IDLE);

        Input.MouseMode = Input.MouseModeEnum.Captured;

        Pivot ??= GetNode<Node3D>("Pivot");
        Camera ??= Pivot.GetNode<Camera3D>("Camera3D");

        // Inicializa yaw/pitch a partir do pivot atual (se houver)
        var rot = Pivot.GlobalRotation;
        pitch = rot.X;
        yaw = rot.Y;
    }

    public override void _PhysicsProcess(double delta)
    {
        // 1) Atualiza posicionamento do Pivot para "seguir" o jogador (apenas posição)
        UpdatePivotPosition();

        // 2) Lê input e move o jogador baseado no yaw do mouse
        HandleMovement((float)delta);

        // 3) Faz o personagem olhar para a direção do movimento (suavemente)
        HandleCharacterRotation((float)delta);

        // 4) Aplica o movimento
        MoveAndSlide();
    }

    // Faz o Pivot acompanhar a posição global do jogador (mantendo yaw/pitch controlados pelo mouse)
    private void UpdatePivotPosition()
    {
        // Coloca o pivot na posição do jogador + altura (usa Global para ignorar parent transforms)
        Vector3 desiredPos = GlobalPosition + Vector3.Up * PivotHeight;
        Pivot.GlobalPosition = desiredPos;

        // Aplica a rotação atual do mouse no pivot (pitch, yaw)
        Pivot.GlobalRotation = new Vector3(pitch, yaw, 0);
    }

    private void HandleMovement(float delta)
    {
        // Pega input WASD
        direction = Input.GetVector(
            GConstants.MOVE_LEFT,
            GConstants.MOVE_RIGHT,
            GConstants.MOVE_FORWARD,
            GConstants.MOVE_BACKWARD
        );

        // Animações
        if (direction == Vector2.Zero)
            animPlayerNode.Play(GConstants.ANIM_IDLE);
        else
            animPlayerNode.Play(GConstants.ANIM_WALK);

        // Cria uma base apenas com o yaw atual do mouse (ignora pitch).
        Basis yawBasis = Basis.FromEuler(new Vector3(0, yaw, 0));

        Vector3 forward = yawBasis.Z;
        Vector3 right = yawBasis.X;

        // MoveDir em mundo baseado no yaw da câmera
        Vector3 moveDir = (forward * direction.Y) + (right * direction.X);

        // evita NaN ou zero-length
        if (moveDir.LengthSquared() > 0.0001f)
            moveDir = moveDir.Normalized();
        else
            moveDir = Vector3.Zero;

        // Mantém componente Y atual (se quiser gravidade, adicione ela)
        Vector3 vel = Velocity;
        vel.X = moveDir.X * speed;
        vel.Z = moveDir.Z * speed;
        Velocity = vel;
    }

    private void HandleCharacterRotation(float delta)
    {
        // Rotaciona o personagem para a direção do movimento (se houver movimento)
        Vector3 horizontalVel = new Vector3(Velocity.X, 0, Velocity.Z);

        if (horizontalVel.LengthSquared() > 0.0001f)
        {
            float targetYaw = Mathf.Atan2(horizontalVel.X, horizontalVel.Z);
            // suaviza a rotação do player (lerp com velocidade dependente de delta)
            float currentYaw = Rotation.Y;
            float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, RotationSmoothness * (float)delta);
            Rotation = new Vector3(0, newYaw, 0);
        }
    }

    // ================= INPUT (mouse controla yaw/pitch do pivot) =================
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            yaw -= motion.Relative.X * MouseSensitivity * 0.01f;
            pitch += motion.Relative.Y * MouseSensitivity * 0.01f;

            pitch = Mathf.Clamp(
                pitch,
                Mathf.DegToRad(MinPitch),
                Mathf.DegToRad(MaxPitch)
            );

            // Atualiza pivot rotation aqui também (redundante com UpdatePivotPosition, mas mantém responsividade)
            Pivot.GlobalRotation = new Vector3(pitch, yaw, 0);
        }

        if (@event.IsActionPressed("ui_cancel"))
            Input.MouseMode = Input.MouseModeEnum.Visible;

        if (@event is InputEventMouseButton && Input.MouseMode != Input.MouseModeEnum.Captured)
            Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
