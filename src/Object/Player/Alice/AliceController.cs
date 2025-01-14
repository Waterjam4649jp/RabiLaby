using Godot;
using RabiLaby.src;
using RabiLaby.src.Object;
using RabiLaby.src.Object.Player;
using System;

public partial class AliceController : CharacterBody2D
{
    private float Gravity = 7.5f;
    private int WalkSpeed = 60;
    private int JumpForce = 135;
    private int BounceForce = 195;
    private int TerminalVelocity = 120;
    private float AnimationSpeed = 1.0f;
    private bool isControlled = true;

    private CharacterBody2D _body;
    private AnimatedSprite2D _animation;

    public (string pre, string post) floorType = ("None", "None");
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
        CollisionController.MountOnAliceOrHat(floorType);
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
