using Godot;

namespace RabiLaby.src.Object.Player
{
    public static class PlayerAnimation
    {
        private static string AnimationName;
        public static void Apply(AnimatedSprite2D _animation, float speed, Vector2 velocity, bool isOnFloor, bool isControlled)
        {
            AnimationName = Controll(_animation, isOnFloor, isControlled);
            Ready(_animation, AnimationName, speed, velocity, isOnFloor);
        }

        private static string Controll(AnimatedSprite2D _animation, bool isOnFloor, bool isControlled)
        {
            if (isControlled)
            {
                // When inputting right and left, or no key, player waits
                if (Input.IsActionPressed(GetControllMap.Left) && Input.IsActionPressed(GetControllMap.Right))
                {
                    return GetAnimationMap.Wait;
                }
                else if (Input.IsActionPressed(GetControllMap.Left))
                {
                    _animation.FlipH = true;
                    return GetAnimationMap.Walk;
                }
                else if (Input.IsActionPressed(GetControllMap.Right))
                {
                    _animation.FlipH = false;
                    return GetAnimationMap.Walk;
                }
                else
                {
                    return GetAnimationMap.Wait;
                }
            }
            return GetAnimationMap.Wait;
        }

        private static void Ready(AnimatedSprite2D _animation, string AnimationName, float speed, Vector2 velocity, bool isOnFloor)
        {
            _animation.AnimationChanged += () => _animation.Stop();

            if (AnimationName == GetAnimationMap.Wait && isOnFloor)
            {
                _animation.Play(AnimationName, speed);
            }

            if (AnimationName == GetAnimationMap.Walk && isOnFloor)
            {
                _animation.Play(AnimationName, speed);
            }

            if (!isOnFloor) // IsOnFloor can be omitted, but inputted on purpose to indicate.
            {
                _animation.Play(GetAnimationMap.Jump);

                const float threshold = 15f;

                switch (velocity.Y)
                {
                    case > threshold:
                        _animation.SetFrame(2); break; // falling
                    case < -threshold:
                        _animation.SetFrame(0); break; // jumping
                    default:
                        _animation.SetFrame(1); break; // hovering (-threshold =< velocity.Y =< threshold)
                }
            }
        }
    }
}