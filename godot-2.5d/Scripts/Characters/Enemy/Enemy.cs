using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    private const float SPEED = 200.0f;
    private const float CHASE_RANGE = 20.0f;
    private const float ATTACK_RANGE = 1.2f;

    [Export]
    public CharacterBody3D target;

    private NavigationAgent3D navAgent;
    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback stateMachine;

    public override void _Ready()
    {
        navAgent = GetNode<NavigationAgent3D>("nav_agent");
        animationTree = GetNode<AnimationTree>("gdbot_skin/AnimationTree");

        // Obtemos o state machine interno do AnimationTree
        stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public override void _Process(double delta)
    {
        Velocity = Vector3.Zero;

        string currentNode = stateMachine.GetCurrentNode();

        switch (currentNode)
        {
            case "Idle":
                LookAt(new Vector3(target.GlobalPosition.X, GlobalPosition.Y, target.GlobalPosition.Z), Vector3.Up);
                break;

            case "walk":
                if (GlobalPosition.DistanceTo(target.GlobalPosition) < CHASE_RANGE)
                {
                    navAgent.TargetPosition = target.GlobalTransform.Origin;
                    Vector3 nextNavPoint = navAgent.GetNextPathPosition();
                    Velocity = (nextNavPoint - GlobalTransform.Origin).Normalized() * SPEED * (float)delta;
                    LookAt(new Vector3(target.GlobalPosition.X, GlobalPosition.Y, target.GlobalPosition.Z), Vector3.Up);
                }
                break;

            case "simple_punch":
                LookAt(new Vector3(target.GlobalPosition.X, GlobalPosition.Y, target.GlobalPosition.Z), Vector3.Up);
                break;
        }

        // Atualiza as condições do AnimationTree
        animationTree.Set("parameters/conditions/walk", ChasePlayer());
        animationTree.Set("parameters/conditions/Idle", !ChasePlayer());
        animationTree.Set("parameters/conditions/attack", AttackPlayer());

        MoveAndSlide();
    }

    private bool ChasePlayer()
    {
        return GlobalPosition.DistanceTo(target.GlobalPosition) < CHASE_RANGE;
    }

    private bool AttackPlayer()
    {
        return GlobalPosition.DistanceTo(target.GlobalPosition) < ATTACK_RANGE;
    }
}