using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class Movement : CharacterBody2D
{
    [Export] private float Gravity = 8.5f; // main()
    [Export] private int WalkSpeed = 60; // HorizontalMove()
    [Export] private int JumpForce = 190; // Jump()
    [Export] private int BounceForce = 300;
    [Export] private int TerminalVelocity = 250;
    private float AnimationSpeed = 1.0f; // AnimationReady()
    private bool isControlled = false;
    private (string pre, string post) floorType = ("None", "None");

    private (Vector2 velocity, string animation) Action;

    public override void _Ready()
    {

        PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;
        SafeMargin = 0.05f;
    }

    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            isControlled = !isControlled;

        Action = (Velocity, "wait")
            .HorizontalMove(WalkSpeed, IsOnFloor(), isControlled)
            .Jump(JumpForce, IsOnFloor(), isControlled)
            .ApplyGravity(Gravity)
            .ApplyTerminalVelocity(TerminalVelocity);

        Velocity = Action.velocity;

        MoveAndSlide();

        floorType = (floorType.post, GetFloorType());
        if (floorType.pre != floorType.post) { GD.Print(floorType); }
    }
    private string GetFloorType() // List <KinematicBody2D> collisions
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

public static class ActionExtensions
{
    static Movement Movement { get; set; }

    public static (Vector2 velocity, string animation) HorizontalMove(this (Vector2 velocity, string animation) Action, float walkSpeed, bool isOnFloor, bool isControlled)
    {
        // When inputting right and left, or no key, player waits
        if (isControlled)
        {
            if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right"))
            {
                Action.velocity.X = 0;
                return (Action.velocity, "wait");
            }
            else if (Input.IsActionPressed("move_left"))
            {
                Action.velocity.X = -walkSpeed;
                return (Action.velocity, "walk");
            }
            else if (Input.IsActionPressed("move_right"))
            {
                Action.velocity.X = walkSpeed;
                return (Action.velocity, "walk");
            }
        }

        switch (isOnFloor)
        {
            case true:
                {
                    Action.velocity.X = 0;
                    return (Action.velocity, "wait");
                }
            case false:
                {
                    Action.velocity.X = 0;
                    return (Action.velocity, "jump");
                }
        }
    }

    public static (Vector2 velocity, string animation) Jump(this (Vector2 velocity, string animation) Action, int JumpForce, bool isOnFloor, bool isControlled)
    {
        if (!isControlled)
        {
            return Action;
        }

        if (isOnFloor)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                Action.velocity.Y = -JumpForce;
                Action.animation = "jump";
                return Action;
            }
            else
            {
                return Action;
            }
        }
        else
        {
            Action.animation = "jump"; //run jump animation when free falling
            return Action;
        }
    }

    public static (Vector2 velocity, string animation) InteractionMovement(this (Vector2 velocity, string animation) Action, (string pre, string post) floorType, CharacterBody2D alice, float Gravity)
    {
        if (floorType.pre != "TileMapLayer" && floorType.post == "Alice") // When  is higher than Alice
        {
            Action.velocity.X += alice.GetPositionDelta().X;
            Action.velocity.Y -= Gravity;
        }
        return Action;
    }

    public static (Vector2 velocity, string animation) ApplyGravity(this (Vector2 velocity, string animation) Action, float Gravity)
    {
        Action.velocity.Y += Gravity;
        return Action;
    }

    public static (Vector2 velocity, string animation) ApplyTerminalVelocity(this (Vector2 velocity, string animation) Action, int TerminalVelocity)
    {
        if (Action.velocity.Y > TerminalVelocity)
        {
            Action.velocity.Y = TerminalVelocity;
        }
        return Action;
    }

    public static void AnimationReady(this (Vector2 velocity, string animation) Action, AnimatedSprite2D animated, float speed, bool isOnFloor)
    {
        if (Action.animation == "walk" && isOnFloor)
        {
            animated.AnimationChanged += () => animated.Stop();
            animated.Play("walk", speed);
            switch (Action.velocity.X)
            {
                case > 0:
                    animated.FlipH = false; // move to right
                    break;
                case < 0:
                    animated.FlipH = true;
                    break;
            }
        }

        if (Action.animation == "wait" && isOnFloor)
        {
            animated.AnimationChanged += () => animated.Stop();
            animated.Play("wait", speed);
        }

        if (Action.animation == "jump" && !isOnFloor) // IsOnFloor can be omitted, but inputted on purpose to indicate.
        {
            animated.AnimationChanged += () => animated.Stop();
            animated.Play("jump");

            const float threshold = 15f; // TODO: determine threshold

            switch (Action.velocity.Y)
            {
                case > threshold:
                    animated.SetFrame(2); break; // jumping
                case < -threshold:
                    animated.SetFrame(0); break; // falling
                default:
                    animated.SetFrame(1); break; // hovering (-threshold =< velocity.Y =< threshold)
            }
            switch (Action.velocity.X)
            {
                case > 0:
                    animated.FlipH = false; // move to right
                    break;
                case < 0:
                    animated.FlipH = true;
                    break;
            }
        }
    }
}



/*
public List<string> GetFloorType(string[] ContactableObjNames)
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