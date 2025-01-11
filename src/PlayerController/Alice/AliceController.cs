using Godot;
using RabiLaby.src;
using RabiLaby.src.AnimationController;
using RabiLaby.src.PlayerController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class AliceController : CharacterBody2D
{
    private float Gravity = 7.5f; // main()
    private int WalkSpeed = 60; // HorizontalMove()
    private int JumpForce = 135; // Jump()
    private int BounceForce = 195;
    private int TerminalVelocity = 120;
    private float AnimationSpeed = 1.0f; // AnimationReady()
    private bool isControlled = true;

    private CharacterBody2D _body;
    private AnimatedSprite2D _animation;
    private (string pre, string post) floorType = ("None", "None");

    private Vector2 velocity;

    public override void _Ready()
    {
        _body = GetNode<CharacterBody2D>(GetPath());
        _animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;
    }

    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            isControlled = !isControlled;

        Velocity = PlayerMovement.Apply(Velocity,
                                        Gravity,
                                        WalkSpeed,
                                        JumpForce,
                                        TerminalVelocity,
                                        IsOnFloor(),
                                        isControlled);

        Velocity = InteractionMovement(Velocity, floorType);
        MoveAndSlide();

        CommonAnimation.Apply(_animation,
                              AnimationSpeed,
                              Velocity,
                              IsOnFloor(),
                              isControlled);

        floorType = (floorType.post, CollisionStates.GetFloorType(_body));
        if (floorType.pre != floorType.post) { GD.Print(floorType); }
    }

    private Vector2 InteractionMovement(Vector2 velocity, (string pre, string post) floorType)
    {
        if (floorType.post == "Lily") // When  Alis higher than 
        {
            velocity.Y = -BounceForce;
        }
        return velocity;
    }
}
