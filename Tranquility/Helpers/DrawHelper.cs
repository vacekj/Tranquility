﻿using System;
using System.Drawing;
using System.Threading;
using Tranquility.Modules;
using static Tranquility.Helpers.PrettyLog;
using Timer = System.Timers.Timer;


namespace Tranquility.Helpers
{
    public class DrawHelper
    {
        private Timer _timer;
        private Rectangle _fov;
        private Pen _pen;
        private Thread _thread;

        public DrawHelper(IModule module)
        {
            _fov = module.GetFov().FieldOfView;
            _pen = new Pen(Color.Green, 3);
        }


        public void ToggleFov()
        {
            if (_thread == null || _timer == null)
            {
                run();
                LogSuccess("fov enabled ");
            }
            else
            {
                _timer.Enabled = false;
                _timer = null;
                _thread.Abort();
                LogWarning("fov disabled");
            }
        }


        private void run()
        {
            _thread = new Thread(() =>
            {
                _timer = new Timer() { Interval = 2, Enabled = true };
                _timer.Elapsed += (sender, args) =>
                {
                    using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        g.DrawRectangle(_pen, _fov);
                    }
                };
            });
            _thread.Start();
        }
    }
}