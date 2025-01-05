using Godot;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class PlayerMovement : CharacterBody2D
{
    [Export] public float Gravity = 200.0f;
    [Export] public int WalkSpeed = 200;
    [Export] public int JumpForce = 500;
    [Export] public float AnimationSpeed = 1.0f;

    private (Vector2 velocity, string name) CharacterAction;

    private AnimatedSprite2D _animatedSprite;
    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _PhysicsProcess(double delta)
    {
        CharacterAction = (Velocity, "stop");
        CharacterAction = HorizontalMove(CharacterAction.velocity);
        CharacterAction.velocity = Jump(CharacterAction.velocity); //Jump animation depedences on whether player is grounded

        AnimationReady(_animatedSprite, CharacterAction, AnimationSpeed);

        CharacterAction.velocity.Y += Gravity; //Apply Gravity
        Velocity = CharacterAction.velocity;

        MoveAndSlide();

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
        }
    }

    private (Vector2 velocity, string animation) HorizontalMove(Vector2 velocity)
    {
        if (Input.IsActionPressed("move_left"))
        {
            velocity.X = -WalkSpeed;
            return (velocity, "walk");
        }
        else if (Input.IsActionPressed("move_right"))
        {
            velocity.X = WalkSpeed;
            return (velocity, "walk");
        }
        else
        {
            velocity.X = 0;
            return (velocity, "stop");
        }
    }

    private Vector2 Jump(Vector2 velocity)
    {
        if (Input.IsActionPressed("jump"))
        {
            velocity.Y = -JumpForce;
            return velocity;
        }
        else
        {
            return velocity;
        }
    }
    private void AnimationReady(AnimatedSprite2D _animatedSprite, (Vector2 velocity, string name) Action, float speed)
    {
        if (Action.name == "walk")
        {
            _animatedSprite.AnimationChanged += () => _animatedSprite.Pause();
            _animatedSprite.Play("walk", speed);
            if (Action.velocity.X > 0)
            {
                _animatedSprite.FlipH = false;
            }
            else if (Action.velocity.X < 0)
            {
                _animatedSprite.FlipH = true;
            }
        }

        if (Action.name == "stop")
        {
            _animatedSprite.AnimationChanged += () => _animatedSprite.Pause();
            _animatedSprite.Play("stop", speed);
        }

        /*if (name == "jump")
        {
            if (velocity.Y > 0)

            
        }
        */
    }
}
