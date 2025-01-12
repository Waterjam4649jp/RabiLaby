using Godot;

namespace RabiLaby.src.Player
{
    public static class PlayerMovement
    {
        public static Vector2 Apply(Vector2 velocity,
                                    float Gravity,
                                    int WalkSpeed,
                                    int JumpForce,
                                    int TerminalVelocity,
                                    bool isOnFloor,
                                    bool isControlled)
        {
            return velocity
            .PlayerController(WalkSpeed, JumpForce, isOnFloor, isControlled)
            .ApplyGravity(Gravity)
            .ApplyTerminalVelocity(TerminalVelocity);
        }


    }

    public static class VelocityController
    {

        public static Vector2 Velocity { get; set; }

        public static Vector2 PlayerController(this Vector2 velocity, float walkSpeed, int JumpForce, bool isOnFloor, bool isControlled)
        {
            if (isControlled)
            {
                if (isOnFloor)
                {
                    if (Input.IsActionJustPressed(GetControllMap.Jump))
                    {
                        velocity.Y = -JumpForce;
                    }
                }

                // When inputting right and left, or no key, player waits
                if (Input.IsActionPressed(GetControllMap.Left) && Input.IsActionPressed(GetControllMap.Right))
                {
                    velocity.X = 0;
                    return velocity;
                }
                else if (Input.IsActionPressed(GetControllMap.Left))
                {
                    velocity.X = -walkSpeed;
                    return velocity;
                }
                else if (Input.IsActionPressed(GetControllMap.Right))
                {
                    velocity.X = walkSpeed;
                    return velocity;
                }
                else
                {
                    velocity.X = 0;
                    return velocity;
                }
            }

            velocity.X = 0;
            return velocity;
        }

        public static Vector2 ApplyGravity(this Vector2 velocity, float Gravity)
        {
            velocity.Y += Gravity;
            return velocity;
        }

        public static Vector2 ApplyTerminalVelocity(this Vector2 velocity, int TerminalVelocity)
        {
            if (velocity.Y > TerminalVelocity)
            {
                velocity.Y = TerminalVelocity;
            }
            return velocity;
        }
    }
}