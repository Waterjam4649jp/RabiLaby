using Godot;
using RabiLaby.src;
using RabiLaby.src.Object;
using RabiLaby.src.Object.Player;
using System;

public partial class LilyController : CharacterBody2D
{
    [Export] private float Gravity = 8.5f;
    [Export] private int WalkSpeed = 60;
    [Export] private int JumpForce = 190;
    [Export] private int BounceForce = 300;
    [Export] private int TerminalVelocity = 250;

    private CharacterBody2D _body;
    private AnimatedSprite2D _animation;
    private AliceController _alice;
    private float AnimationSpeed = 1.0f;
    private bool isLilyControlled = false;
    private (string pre, string post) floorType = ("None", "None");
    private bool isSpecificAnimationIsPlaying = false;

    private Vector2 velocity;

    private string[] CollisionLayersAndMaps = new string[2] { "Map", "Player" };

    public override void _Ready()
    {
        _body = GetNode<CharacterBody2D>(GetPath());
        _animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _alice = new AliceController();

        _alice.AliceSteppedOnLily += OnAliceSteppedOnLily;
        _animation.AnimationFinished += () => isSpecificAnimationIsPlaying = false;

        PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;

        base._Ready();
        AddToGroup("Mountable");
    }


    public override void _PhysicsProcess(double delta)
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

        if (isSpecificAnimationIsPlaying)
            Flipping(_animation, IsOnFloor(), isLilyControlled);
        else
            PlayerAnimation.Apply(_animation,
                                  AnimationSpeed,
                                  Velocity,
                                  IsOnFloor(),
                                  isLilyControlled);

        floorType = (floorType.post, CollisionStates.GetFloorType(_body));
        CollisionController.MountOnAliceOrHat(floorType);
        GD.Print(CollisionSelecter.ToDigit(CollisionLayersAndMaps));
    }
    private void OnAliceSteppedOnLily()
    {
        SpecificAnimation(_animation, "vibe", AnimationSpeed);
    }


    private void SpecificAnimation(AnimatedSprite2D _animation, string AnimationName, float speed)
    {
        _animation.Play(AnimationName, speed);
        isSpecificAnimationIsPlaying = true;
    }

    private static void Flipping(AnimatedSprite2D _animation, bool isOnFloor, bool isControlled)
    {
        if (isControlled)
        {
            if (Input.IsActionPressed(GetControllMap.Left))
            {
                _animation.FlipH = true;
            }
            else if (Input.IsActionPressed(GetControllMap.Right))
            {
                _animation.FlipH = false;
            }
        }
    }

}