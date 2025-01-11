using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

using RabiLaby.src.PlayerController;

public partial class LilyMovement : CharacterBody2D
{
    [Export] private float LilyGravity = 8.5f; // main()
    [Export] private int LilyWalkSpeed = 60; // LilyHorizontalMove()
    [Export] private int LilyJumpForce = 190; // LilyJump()
    [Export] private int LilyBounceForce = 300;
    [Export] private int LilyTerminalVelocity = 250;
    private float LilyAnimationSpeed = 1.0f; // LilyAnimationReady()
    private bool isLilyControlled = false;
    private (string pre, string post) floorType = ("None", "None");

    private (Vector2 velocity, string animation) lilyAction;
    private AnimatedSprite2D animatedLily;
    private CharacterBody2D alice;

    public override void _Ready()
    {
        animatedLily = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        alice = GetNode<CharacterBody2D>("../../Alice/CharacterBody2D");

        PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;
        SafeMargin = 0.05f;
    }

    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            isLilyControlled = !isLilyControlled;

        lilyAction = (Velocity, "wait")
            .LilyHorizontalMove(LilyWalkSpeed, IsOnFloor(), isLilyControlled)
            .LilyJump(LilyJumpForce, IsOnFloor(), isLilyControlled)
            .LilyInteractionMovement(floorType, alice, LilyGravity)
            .LilyApplyGravity(LilyGravity)
            .LilyApplyTerminalVelocity(LilyTerminalVelocity);

        Velocity = lilyAction.velocity;
        LilyAnimationReady(animatedLily, lilyAction, LilyAnimationSpeed, IsOnFloor());

        MoveAndSlide();

        floorType = (floorType.post, LilyGetFloorType());
        if (floorType.pre != floorType.post) { GD.Print(floorType); }
    }

    private void LilyAnimationReady(AnimatedSprite2D animatedLily, (Vector2 velocity, string animation) lilyAction, float speed, bool isOnFloor)
    {
        if (lilyAction.animation == "walk" && isOnFloor)
        {
            animatedLily.AnimationChanged += () => animatedLily.Stop();
            animatedLily.Play("walk", speed);
            switch (lilyAction.velocity.X)
            {
                case > 0:
                    animatedLily.FlipH = false; // move to right
                    break;
                case < 0:
                    animatedLily.FlipH = true;
                    break;
            }
        }

        if (lilyAction.animation == "wait" && isOnFloor)
        {
            animatedLily.AnimationChanged += () => animatedLily.Stop();
            animatedLily.Play("wait", speed);
        }

        if (lilyAction.animation == "jump" && !isOnFloor) // IsOnFloor can be omitted, but inputted on purpose to indicate.
        {
            animatedLily.AnimationChanged += () => animatedLily.Stop();
            animatedLily.Play("jump");

            const float threshold = 15f; // TODO: determine threshold

            switch (lilyAction.velocity.Y)
            {
                case > threshold:
                    animatedLily.SetFrame(2); break; // jumping
                case < -threshold:
                    animatedLily.SetFrame(0); break; // falling
                default:
                    animatedLily.SetFrame(1); break; // hovering (-threshold =< velocity.Y =< threshold)
            }
            switch (lilyAction.velocity.X)
            {
                case > 0:
                    animatedLily.FlipH = false; // move to right
                    break;
                case < 0:
                    animatedLily.FlipH = true;
                    break;
            }
        }
    }

    private string LilyGetFloorType() // List <KinematicBody2D> collisions
    {
        if (GetSlideCollisionCount() == 0)
        {
            return "None";
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            var contactedObj = collision.GetCollider();

            if ((contactedObj as Node).Name != "CharacterBody2D" && (contactedObj as Node).Name != "TileMapLayer")
            {
                return "Undefined";
            }

            if ((contactedObj as Node).Name == "CharacterBody2D" && (contactedObj as CharacterBody2D).Position.Y > Position.Y)
            {
                return (contactedObj as Node).GetParent().Name;
            }
            if ((contactedObj as Node).Name == "TileMapLayer")
            {
                return (contactedObj as Node).Name;
            }
        }

        return "Undefined";
    }

}

public static class LilyActionExtensions
{
    static LilyMovement lilyMovement { get; set; }

    public static (Vector2 velocity, string animation) LilyHorizontalMove(this (Vector2 velocity, string animation) lilyAction, float walkSpeed, bool isOnFloor, bool isLilyControlled)
    {
        // When inputting right and left, or no key, player waits
        if (isLilyControlled)
        {
            if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right"))
            {
                lilyAction.velocity.X = 0;
                return (lilyAction.velocity, "wait");
            }
            else if (Input.IsActionPressed("move_left"))
            {
                lilyAction.velocity.X = -walkSpeed;
                return (lilyAction.velocity, "walk");
            }
            else if (Input.IsActionPressed("move_right"))
            {
                lilyAction.velocity.X = walkSpeed;
                return (lilyAction.velocity, "walk");
            }
        }

        switch (isOnFloor)
        {
            case true:
                {
                    lilyAction.velocity.X = 0;
                    return (lilyAction.velocity, "wait");
                }
            case false:
                {
                    lilyAction.velocity.X = 0;
                    return (lilyAction.velocity, "jump");
                }
        }
    }

    public static (Vector2 velocity, string animation) LilyJump(this (Vector2 velocity, string animation) lilyAction, int lilyJumpForce, bool isOnFloor, bool isLilyControlled)
    {
        if (!isLilyControlled)
        {
            return lilyAction;
        }

        if (isOnFloor)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                lilyAction.velocity.Y = -lilyJumpForce;
                lilyAction.animation = "jump";
                return lilyAction;
            }
            else
            {
                return lilyAction;
            }
        }
        else
        {
            lilyAction.animation = "jump"; //run jump animation when free falling
            return lilyAction;
        }
    }

    public static (Vector2 velocity, string animation) LilyInteractionMovement(this (Vector2 velocity, string animation) lilyAction, (string pre, string post) floorType, CharacterBody2D alice, float lilyGravity)
    {
        if (floorType.pre != "TileMapLayer" && floorType.post == "Alice") // When Lily is higher than Alice
        {
            lilyAction.velocity.X += alice.GetPositionDelta().X;
            lilyAction.velocity.Y -= lilyGravity;
        }
        return lilyAction;
    }

    public static (Vector2 velocity, string animation) LilyApplyGravity(this (Vector2 velocity, string animation) lilyAction, float lilyGravity)
    {
        lilyAction.velocity.Y += lilyGravity;
        return lilyAction;
    }

    public static (Vector2 velocity, string animation) LilyApplyTerminalVelocity(this (Vector2 velocity, string animation) lilyAction, int lilyTerminalVelocity)
    {
        if (lilyAction.velocity.Y > lilyTerminalVelocity)
        {
            lilyAction.velocity.Y = lilyTerminalVelocity;
        }
        return lilyAction;
    }

    public static void LilyAnimationReady(this (Vector2 velocity, string animation) lilyAction, AnimatedSprite2D animatedLily, float speed, bool isOnFloor)
    {
        if (lilyAction.animation == "walk" && isOnFloor)
        {
            animatedLily.AnimationChanged += () => animatedLily.Stop();
            animatedLily.Play("walk", speed);
            switch (lilyAction.velocity.X)
            {
                case > 0:
                    animatedLily.FlipH = false; // move to right
                    break;
                case < 0:
                    animatedLily.FlipH = true;
                    break;
            }
        }

        if (lilyAction.animation == "wait" && isOnFloor)
        {
            animatedLily.AnimationChanged += () => animatedLily.Stop();
            animatedLily.Play("wait", speed);
        }

        if (lilyAction.animation == "jump" && !isOnFloor) // IsOnFloor can be omitted, but inputted on purpose to indicate.
        {
            animatedLily.AnimationChanged += () => animatedLily.Stop();
            animatedLily.Play("jump");

            const float threshold = 15f; // TODO: determine threshold

            switch (lilyAction.velocity.Y)
            {
                case > threshold:
                    animatedLily.SetFrame(2); break; // jumping
                case < -threshold:
                    animatedLily.SetFrame(0); break; // falling
                default:
                    animatedLily.SetFrame(1); break; // hovering (-threshold =< velocity.Y =< threshold)
            }
            switch (lilyAction.velocity.X)
            {
                case > 0:
                    animatedLily.FlipH = false; // move to right
                    break;
                case < 0:
                    animatedLily.FlipH = true;
                    break;
            }
        }
    }
}



/*
public List<string> LilyGetFloorType(string[] ContactableObjNames)
{
    var FloorTypes = new List<string>(10);

    if (GetSlideCollisionCount() == 0)
    {
        return new List<string>() { "None" };
    }

    for (int i = 0; i < GetSlideCollisionCount(); i++)
    {
        KinematicCollision2D collision = GetSlideCollision(i);
        var ContactedObj = collision.GetCollider();

        foreach (string ObjName in ContactableObjNames)
        {
            if ((ContactedObj as Node).Name == ObjName && !FloorTypes.Contains(ObjName)) // avoid overlapping
            {
                switch(ObjName)
                {
                    case "TileMapLayer":
                        FloorTypes.Add((ContactedObj as Node).Name);
                        break;
                    case "CharacterBody2D":
                        FloorTypes.Add((ContactedObj as Node).GetParent().Name);
                        break;
                    default:
                        FloorTypes.Add(ObjName);
                        break;
                }
            }
        }
    }

    if (FloorTypes != null && FloorTypes.Count > 0)
        return FloorTypes;
    else
        return new List<string>() { "Error" };
}
*/