using Godot;

public partial class PlayerMovement : CharacterBody2D
{
    [Export] public float Gravity = 200.0f;
    [Export] public int WalkSpeed = 200;
    [Export] public int JumpForce = 500;

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        velocity.Y += (float)delta * Gravity;

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X = -WalkSpeed;
        }
        else if (Input.IsActionPressed("move_right"))
        {
            velocity.X = WalkSpeed;
        }
        else
        {
            velocity.X = 0;
        }

        if (Input.IsActionPressed("jump"))
        {
            velocity.Y = -JumpForce;
        }

        Velocity = velocity;

        MoveAndSlide();
    }
}
