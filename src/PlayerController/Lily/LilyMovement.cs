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
    [Export] private float Gravity = 8.5f; // main()
    [Export] private int WalkSpeed = 60; // HorizontalMove()
    [Export] private int JumpForce = 190; // Jump()
    [Export] private int BounceForce = 300;
    [Export] private int TerminalVelocity = 250;

    private float LilyAnimationSpeed = 1.0f; // AnimationReady()
    private bool isLilyControlled = false;
    private (string pre, string post) floorType = ("None", "None");

    private Vector2 velocity;

    public override void _Ready()
    {
        PlatformOnLeave = PlatformOnLeaveEnum.DoNothing;
    }

    public override void _PhysicsProcess(double delta) // as main
    {
        if (Input.IsActionJustPressed("change_character"))
            isLilyControlled = !isLilyControlled;

        Velocity = PlayerMovement.Apply(Velocity, Gravity, WalkSpeed, JumpForce, TerminalVelocity, IsOnFloor(), isLilyControlled);
        MoveAndSlide();

        floorType = (floorType.post, LilyGetFloorType());
        if (floorType.pre != floorType.post) { GD.Print(floorType); }
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