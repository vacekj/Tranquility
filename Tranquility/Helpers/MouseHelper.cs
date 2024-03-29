﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Tranquility.Modules;
using Tranquility.Objects;

namespace Tranquility.Helpers
{
    internal class MouseHelper
    {
        public static MouseProxy Proxy;

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        /// <summary>
        /// Clicks the mouse.
        /// </summary>
        public static void Click()
        {
            Proxy.Fire();
        }

        /// <summary>
        /// Moves the mouse.
        /// </summary>
        /// <param name="myFov"></param>
        /// <param name="coordinates"></param>
        public static void Move(ref Fov myFov, Point coordinates, bool forceHeadshot = false)
        {
            // Get the crosshair position.
            var crosshair = new Point
            {
                X = myFov.Resolution.X / 2,
                Y = myFov.Resolution.Y / 2
            };

            // Determine the stepcounts.
            var stepCount = new Point();
            var step = new Point();

            // Calculate the destination.
            var destination = new Point
            {
                X = coordinates.X + myFov.RangeValues.X,
                Y = coordinates.Y + myFov.RangeValues.Y
            };

            // Calculate the difference from the crosshair.
            var difference = new Point
            {
                X = Math.Abs(crosshair.X - destination.X),
                Y = Math.Abs(crosshair.Y - destination.Y)
            };

            stepCount.X = 5;
            stepCount.Y = 3;

            // X-axis.
            if (difference.X < 10)
            {
                stepCount.X = 1;

                if (SettingsManager.Aimbot.AntiShake)
                {
                    Thread.Sleep(1);
                }
            }
            else if (difference.X < 40)
            {
                stepCount.X = 5;
                Thread.Sleep(1);
            }

            step.X = stepCount.X;

            if (crosshair.X > destination.X)
            {
                step.X = -stepCount.X;
            }

            // Y-axis.
            if (difference.Y < 10)
            {
                stepCount.Y = 1;
            }

            step.Y = stepCount.Y;

            if (crosshair.Y > destination.Y)
            {
                step.Y = -stepCount.Y;
            }

            if (crosshair.X > destination.X + myFov.Tolerance.X || crosshair.X < destination.X - myFov.Tolerance.X)
            {
                ExecuteMove(step.X, 0);

                if (forceHeadshot)
                {
                    if (crosshair.Y > destination.Y + myFov.Tolerance.Y || crosshair.Y < destination.Y - myFov.Tolerance.Y)
                    {
                        ExecuteMove(0, step.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Executes the mouse moves.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="async"></param>
        private static void ExecuteMove(Point step, bool async = false)
        {
            if (!async)
            {
                Proxy.Move(step.X, step.Y);
            }
            else
            {
                // Move X.
                new Thread(() =>
                {
                    Proxy.Move(step.X, 0);
                }).Start();

                // Move Y.
                new Thread(() =>
                {
                    Proxy.Move(0, step.Y);
                }).Start();
            }
        }

        /// <summary>
        /// Moves the mouse.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void ExecuteMove(int x, int y)
        {
            ExecuteMove(new Point(x, y));
        }
    }
}
