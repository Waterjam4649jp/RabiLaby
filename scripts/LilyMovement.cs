using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class LilyMovement : CharacterBody2D
{
    private float LilyGravity = 5.5f; // main()
    private int LilyWalkSpeed = 50; // LilyHorizontalMove()
    private int LilyJumpForce = 112; // LilyJump()
    private int LilyBounceForce = 300;
    private int LilyTerminalVelocity = 240;
    private float LilyAnimationSpeed = 1.0f; // LilyAnimationReady()
    private bool LilyControlled = false;

    private (Vector2 velocity, string animation) LilyAction;
    private AnimatedSprite2D _animatedLily;
    private CharacterBody2D _alice;

    public override void _Ready()
    {
        _animatedLily = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _alice = GetNode<CharacterBody2D>("../../Alice/CharacterBody2D");

        PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;
        SafeMargin = 0.05f;
    }
    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            LilyControlled = !LilyControlled;

        LilyAction = (Velocity, "wait");
        LilyAction = LilyHorizontalMove(LilyAction, LilyWalkSpeed, IsOnFloor());
        LilyAction = LilyJump(LilyAction, LilyJumpForce, IsOnFloor()); //Jump animation depedences on whether player is grounded
        LilyAction = LilyInteractionMovement(LilyAction, LilyGetFloorType());
        LilyAction = LilyApplyGravity(LilyAction, LilyGravity);
        LilyAction = LilyApplyTerminalVelocity(LilyAction, LilyTerminalVelocity);

        LilyAnimationReady(_animatedLily, LilyAction, LilyAnimationSpeed, IsOnFloor());


        Velocity = LilyAction.velocity;

        MoveAndSlide();
    }

    private (Vector2 velocity, string animation) LilyHorizontalMove((Vector2 velocity, string animation) LilyAction, float WalkSpeed, bool IsOnFloor)
    {
        // When inputting right and left, or no key, player waits
        if (LilyControlled)
        {
            if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right"))
            {
                LilyAction.velocity.X = 0;
                return (LilyAction.velocity, "wait");
            }
            else if (Input.IsActionPressed("move_left"))
            {
                LilyAction.velocity.X = -WalkSpeed;
                return (LilyAction.velocity, "walk");
            }
            else if (Input.IsActionPressed("move_right"))
            {
                LilyAction.velocity.X = WalkSpeed;
                return (LilyAction.velocity, "walk");
            }
        }

        switch (IsOnFloor)
        {
            case true:
                {
                    LilyAction.velocity.X = 0;
                    return (LilyAction.velocity, "wait");
                }
            case false:
                {
                    LilyAction.velocity.X = 0;
                    return (LilyAction.velocity, "jump");
                }
        }
    }

    private (Vector2 velocity, string animation) LilyJump((Vector2 velocity, string animation) LilyAction, int LilyJumpForce, bool IsOnFloor)
    {
        if (!LilyControlled)
        {
            return LilyAction;
        }

        if (IsOnFloor)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                LilyAction.velocity.Y = -LilyJumpForce;
                LilyAction.animation = "jump";
                return LilyAction;
            }
            else
            {
                return LilyAction;
            }
        }
        else
        {
            LilyAction.animation = "jump"; //run jump animation when free falling
            return LilyAction;
        }
    }

    private (Vector2 velocity, string animation) LilyInteractionMovement((Vector2 velocity, string animation) LilyAction, String FloorType)
    {
        if (FloorType == "Alice") // When Lily is higher than Alice
        {
            LilyAction.velocity.X += _alice.GetPositionDelta().X;
            LilyAction.velocity.Y -= LilyGravity;
        }
        return LilyAction;
    }

    private (Vector2 velocity, string animation) LilyApplyGravity((Vector2 velocity, string animation) LilyAction, float LilyGravity)
    {
        LilyAction.velocity.Y += LilyGravity;
        return LilyAction;
    }

    private (Vector2 velocity, string animation) LilyApplyTerminalVelocity((Vector2 velocity, string animation) LilyAction, int LilyTerminalVelocity)
    {
        if (LilyAction.velocity.Y > LilyTerminalVelocity)
        {
            LilyAction.velocity.Y = LilyTerminalVelocity;
        }
        return LilyAction;
    }

    private void LilyAnimationReady(AnimatedSprite2D _animatedLily, (Vector2 velocity, string animation) LilyAction, float speed, bool IsOnFloor)
    {
        if (LilyAction.animation == "walk" && IsOnFloor)
        {
            _animatedLily.AnimationChanged += () => _animatedLily.Stop();
            _animatedLily.Play("walk", speed);
            switch (LilyAction.velocity.X)
            {
                case > 0:
                    _animatedLily.FlipH = false; // move to right
                    break;
                case < 0:
                    _animatedLily.FlipH = true;
                    break;
            }
        }

        if (LilyAction.animation == "wait" && IsOnFloor)
        {
            _animatedLily.AnimationChanged += () => _animatedLily.Stop();
            _animatedLily.Play("wait", speed);
        }

        if (LilyAction.animation == "jump" && !IsOnFloor) // IsOnFloor can be omitted, but inputted on purpose to indicate.
        {
            _animatedLily.AnimationChanged += () => _animatedLily.Stop();
            _animatedLily.Play("jump");

            const float threshold = 15f; // TODO: determine threshold

            switch (LilyAction.velocity.Y)
            {
                case > threshold:
                    _animatedLily.SetFrame(2); break; // jumping
                case < -threshold:
                    _animatedLily.SetFrame(0); break; // falling
                default:
                    _animatedLily.SetFrame(1); break; // hovering (-threshold =< velocity.Y =< threshold)
            }
            switch (LilyAction.velocity.X)
            {
                case > 0:
                    _animatedLily.FlipH = false; // move to right
                    break;
                case < 0:
                    _animatedLily.FlipH = true;
                    break;
            }
        }
    }

    public String LilyGetFloorType() // List <KinematicBody2D> collisions
    {
        if (GetSlideCollisionCount() == 0)
        {
            return "None";
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            var ContactedObj = collision.GetCollider();

            if ((ContactedObj as Node).Name != "CharacterBody2D")
            {
                return "Undefined";
            }

            if ((ContactedObj as CharacterBody2D).Position.Y > Position.Y)
            {
                return (ContactedObj as Node).GetParent().Name;
            }

        }

        return "Undefine";
    }

}
