using Godot;
using RabiLaby.src.Object;
using RabiLaby.src.Player;
using System;

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

    public (string pre, string post) floorType = ("None", "None"); // used for AnimationController\Lily\Lily.cs
    private Vector2 velocity;

    [Signal]
    public delegate void AliceSteppedOnLilyEventHandler();

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

        PlayerAnimation.Apply(_animation,
                              AnimationSpeed,
                              Velocity,
                              IsOnFloor(),
                              isControlled);

        floorType = (floorType.post, CollisionStates.GetFloorType(_body));
    }

    private Vector2 InteractionMovement(Vector2 velocity, (string pre, string post) floorType)
    {
        if (floorType.post == "Lily")
        {
            velocity.Y = -BounceForce;
            EmitSignal(SignalName.AliceSteppedOnLily);
        }
        return velocity;
    }
}
