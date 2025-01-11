using Godot;
using RabiLaby.src.PlayerController;
using RabiLaby.src.AnimationController;

public partial class LilyController : CharacterBody2D
{
    [Export] private float Gravity = 8.5f; // main()
    [Export] private int WalkSpeed = 60; // HorizontalMove()
    [Export] private int JumpForce = 190; // Jump()
    [Export] private int BounceForce = 300;
    [Export] private int TerminalVelocity = 250;

    private CharacterBody2D _body;
    private AnimatedSprite2D _animation;
    private float LilyAnimationSpeed = 1.0f; // AnimationReady()
    private bool isLilyControlled = false;
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
            isLilyControlled = !isLilyControlled;

        Velocity = PlayerMovement.Apply(Velocity,
                                        Gravity, 
                                        WalkSpeed,
                                        JumpForce,
                                        TerminalVelocity,
                                        IsOnFloor(),
                                        isLilyControlled);
        MoveAndSlide();

        CommonAnimation.Apply(_animation,
                              LilyAnimationSpeed,
                              Velocity,
                              IsOnFloor(),
                              isLilyControlled);

        floorType = (floorType.post, CollisionStates.GetFloorType(_body));
        if (floorType.pre != floorType.post) { GD.Print(floorType); }
    }
}