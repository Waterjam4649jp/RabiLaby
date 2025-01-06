using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class AliceMovement : CharacterBody2D
{
    private float AliceGravity = 5.5f; // main()
    private int AliceWalkSpeed = 50; // AliceHorizontalMove()
    private int AliceJumpForce = 112; // AliceJump()
    private float AliceAnimationSpeed = 1.0f; // AliceAnimationReady()
    private bool AliceControlled = true;

    private (Vector2 velocity, string animation) AliceAction;
    private AnimatedSprite2D _animatedAlice;

    private List<KinematicCollision2D> collisions; //IsMounted()

    public override void _Ready()
    {
        _animatedAlice = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            AliceControlled = !AliceControlled;

        if (AliceAction.velocity.Y - Velocity.Y < 0) // Ignore Velocity.Y
        {
            AliceAction.velocity.X = Velocity.X;
            AliceAction.animation = "wait";
        }
        else
        {
            AliceAction = (Velocity, "wait");
        }
        AliceAction = AliceHorizontalMove(AliceAction.velocity, AliceWalkSpeed);
        AliceAction = AliceJump(AliceAction, AliceJumpForce, IsOnFloor()); //Jump animation depedences on whether player is grounded

        AliceAnimationReady(_animatedAlice, AliceAction, AliceAnimationSpeed, IsOnFloor());

        AliceAction.velocity.Y += AliceGravity; //Apply Gravity
        Velocity = AliceAction.velocity;

        MoveAndSlide();
    }

    private (Vector2 velocity, string animation) AliceHorizontalMove(Vector2 velocity, float WalkSpeed)
    {
        if (!AliceControlled)
        {
            velocity.X = 0;
            return (velocity, "wait");
        }

        // When inputting right and left, or no key, player waits
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
        velocity.X = 0;
        return (velocity, "wait");
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

        if (AliceAction.animation == "jump" && !IsOnFloor)
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

    public bool AliceIsMounted() // List <KinematicBody2D> collisions
    {
        if (GetSlideCollisionCount() == 0)
        {
            return false;
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            collisions.Append(GetSlideCollision(i));
            if (collisions.Exists(x => (x.GetCollider() as Node).Name == "Lily"))
                return true;
        }

        return false;
    }
    
}
