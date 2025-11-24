using Godot;
using System;

public partial class Pivot : Node3D
{
    // 🔹 Sensibilidade do movimento do mouse — controla o quanto a câmera gira por pixel movido.
    [Export] public float MouseSensitivity = 0.1f;

    // 🔹 Ângulo mínimo e máximo que a câmera pode olhar (em graus).
    // Isso impede que a câmera gire demais pra cima ou pra baixo.
    [Export] public float MinVerticalAngle = -30f; // Limite para olhar pra baixo
    [Export] public float MaxVerticalAngle = 20f;  // Limite para olhar pra cima

    // 🔹 Guarda o valor atual da inclinação vertical (pitch) da câmera.
    private float _pitch = 0f;

    // 🔹 O método _Input é chamado toda vez que um evento de entrada acontece (mouse, teclado, etc.)
    public override void _Input(InputEvent @event)
    {
        // Verifica se o evento é um movimento do mouse
        if (@event is InputEventMouseMotion motion)
        {
            // 🎯 Atualiza o ângulo de inclinação vertical da câmera (pitch)
            // Multiplica o movimento vertical do mouse (Y) pela sensibilidade.
            // Subtrai porque mover o mouse para cima deve fazer a câmera olhar pra cima.
            _pitch += motion.Relative.Y * MouseSensitivity;

            // 🔒 Limita o valor do pitch entre os ângulos mínimos e máximos definidos.
            // Mathf.DegToRad converte graus em radianos (o Godot usa radianos internamente).
            _pitch = Mathf.Clamp(
                _pitch,
                Mathf.DegToRad(MinVerticalAngle),
                Mathf.DegToRad(MaxVerticalAngle)
            );

            // 🔄 Aplica a rotação no eixo X (vertical)
            // Isso faz o Pivot girar pra cima ou pra baixo,
            // e como a câmera é filha dele, ela acompanha esse movimento.
            Rotation = new Vector3(_pitch, 0, 0);
        }
    }
}