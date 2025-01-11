using Godot;
using System;

namespace RabiLaby.src.AnimationHandler
{

    public static class CommonAnimation
    {
        public static string AnimationName { get; set; }
    }
    public static class AnimationHandler
    {

        private static void AnimationReady(AnimatedSprite2D animated, (Vector2 velocity, string animation) Action, float speed, bool isOnFloor)
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
        private static void AnimationReady(this (Vector2 velocity, string animation) Action, AnimatedSprite2D animated, float speed, bool isOnFloor)
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
}