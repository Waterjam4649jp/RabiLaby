using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class AliceMovement : CharacterBody2D
{
    private float AliceGravity = 5.5f; // main()
    private int AliceWalkSpeed = 50; // AliceHorizontalMove()
    private int AliceJumpForce = 112; // AliceJump()
    private int AliceBounceForce = 200;
    private float AliceAnimationSpeed = 1.0f; // AliceAnimationReady()
    private bool AliceControlled = true;

    private (Vector2 velocity, string animation) AliceAction;
    private AnimatedSprite2D _animatedAlice;

    public override void _Ready()
    {
        _animatedAlice = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            AliceControlled = !AliceControlled;

        AliceAction = (Velocity, "wait");
        AliceAction = AliceHorizontalMove(AliceAction.velocity, AliceWalkSpeed, IsOnFloor());
        AliceAction = AliceJump(AliceAction, AliceJumpForce, IsOnFloor()); //Jump animation depedences on whether player is grounded

        AliceAnimationReady(_animatedAlice, AliceAction, AliceAnimationSpeed, IsOnFloor());

        AliceAction.velocity.Y += AliceGravity; //Apply Gravity
        AliceAction = AliceInteractionMovement(AliceAction, AliceGetFloorType());
        Velocity = AliceAction.velocity;

        MoveAndSlide();
    }

    private (Vector2 velocity, string animation) AliceHorizontalMove(Vector2 velocity, float WalkSpeed, bool IsOnFloor)
    {
        // When inputting right and left, or no key, player waits
        if (AliceControlled)
        {
            if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right"))
            {
                velocity.X = 0;
                return (velocity, "wait");
            }
            else if (Input.IsActionPressed("move_left"))
            {
                velocity.X = -WalkSpeed;
                return (velocity, "walk");
            }
            else if (Input.IsActionPressed("move_right"))
            {
                velocity.X = WalkSpeed;
                return (velocity, "walk");
            }
        }

        switch (IsOnFloor)
        {
            case true:
                {
                    velocity.X = 0;
                    return (velocity, "wait");
                }
            case false:
                {
                    velocity.X = 0;
                    return (velocity, "jump");
                }
        }
    }

    private (Vector2 velocity, string animation) AliceJump((Vector2 velocity, string animation) AliceAction, int AliceJumpForce, bool IsOnFloor)
    {
        if (!AliceControlled)
        {
            return AliceAction;
        }

        if (IsOnFloor)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                AliceAction.velocity.Y = -AliceJumpForce;
                AliceAction.animation = "jump";
                return AliceAction;
            }
            else
            {
                return AliceAction;
            }
        }
        else
        {
            AliceAction.animation = "jump"; //run jump animation when free falling
            return AliceAction;
        }
    }

    private (Vector2 velocity, string animation) AliceInteractionMovement((Vector2 velocity, string animation) AliceAction, String FloorType)
    {
        if (FloorType == "Lily") // When Alice is higher than Lily
        {
            AliceAction.velocity.Y = -AliceBounceForce;
        }
        return AliceAction;
    }
    private void AliceAnimationReady(AnimatedSprite2D _animatedAlice, (Vector2 velocity, string animation) AliceAction, float speed, bool IsOnFloor)
    {
        if (AliceAction.animation == "walk" && IsOnFloor)
        {
            _animatedAlice.AnimationChanged += () => _animatedAlice.Stop();
            _animatedAlice.Play("walk", speed);
            switch (AliceAction.velocity.X)
            {
                case > 0:
                    _animatedAlice.FlipH = false; // move to right
                    break;
                case < 0:
                    _animatedAlice.FlipH = true;
                    break;
            }
        }

        if (AliceAction.animation == "wait" && IsOnFloor)
        {
            _animatedAlice.AnimationChanged += () => _animatedAlice.Stop();
            _animatedAlice.Play("wait", speed);
        }

        if (AliceAction.animation == "jump" && !IsOnFloor) // IsOnFloor can be omitted, but inputted on purpose to indicate.
        {
            _animatedAlice.AnimationChanged += () => _animatedAlice.Stop();
            _animatedAlice.Play("jump");

            const float threshold = 15f; // TODO: determine threshold

            switch (AliceAction.velocity.Y)
            {
                case > threshold:
                    _animatedAlice.SetFrame(2);    break; // jumping
                case < -threshold:
                    _animatedAlice.SetFrame(0);    break; // falling
                default:
                    _animatedAlice.SetFrame(1);    break; // hovering (-threshold =< velocity.Y =< threshold)
            }
            switch (AliceAction.velocity.X)
            {
                case > 0:
                    _animatedAlice.FlipH = false; // move to right
                    break;
                case < 0:
                    _animatedAlice.FlipH = true;
                    break;
            }
        }
    }

    public String AliceGetFloorType() // List <KinematicBody2D> collisions
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
                GD.Print((ContactedObj as CharacterBody2D).Position.Y , Position.Y);
                return (ContactedObj as Node).GetParent().Name;
            }

        }

        return "Undefine";
    }

}
