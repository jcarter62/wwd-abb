using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace wwd_utils {
    /// <summary>
    /// This class was created to simplify the use of a timer in 
    /// a windows service.  The TimerState enumeration was created
    /// to help make it re-entrant, and provide information to parent
    /// as to status of this timer.
    /// 
    /// The start and stop methods technically do not stop the timer, 
    /// however the TimerState allows execution of ticker when
    /// state == Running.
    /// 
    /// The semaphore timelock is used to allow only one tick() call, 
    /// thus only allowing one caller.
    /// 
    /// </summary>
    public class TimerClass {
        /** <summary></summary> */
        public enum TimerState { Created=1, Starting=2, Running=3, Stopping=4, Stopped=5 }; 

        private System.Timers.Timer Timer;
        private TimerState state;
        private int TimerInterv;

        /** <summary></summary> */
        public delegate void Tick();
        private Tick ticker;

        private long clocktick;
        // | / - \ | / - \
        // 0 1 2 3 4 5 6 7 

//            TimeToRunWorker = false;
//            IdleTime = 2 * 60 * 1000; // 2 minutes in ms (milliseconds)
//            TimeUntilRun = 0;
//            WorkerRunning = false;
//            SleepTime = 1000;

        public Boolean TimeToRunWorker { get; set; }
        public int IdleTime { get; set; }
        public int TimeUntilRun { get; set; }
        public Boolean WorkerRunning { get; set; }
        public int SleepTime { get; set; }

        #region Property Code
        /** <summary>Used to provide a single character that looks like the hand of a clock.</summary> */
        public string ClockFace {
            get {
                string rval;
                switch (clocktick % 8) {
                    case 0: rval = "|"; break;
                    case 1: rval = "/"; break;
                    case 2: rval = "-"; break;
                    case 3: rval = "\\"; break;
                    case 4: rval = "|"; break;
                    case 5: rval = "/"; break;
                    case 6: rval = "-"; break;
                    case 7: rval = "\\"; break;
                    default: rval = " "; break;
                }
                return rval;
            }
        }

        public Tick TimerEvent {
            set { ticker = value; }
        }

        public TimerState State {
            get { return state; }
        }

        /** <summary>This is a simple string describing the current clock state</summary> */
        public string StateText {
            get {
                string rval = "";
                switch (state) {
                    case TimerState.Created:
                        rval = "Created";
                        break;
                    case TimerState.Starting:
                        rval = "Starting";
                        break;
                    case TimerState.Running:
                        rval = "Running";
                        break;
                    case TimerState.Stopping:
                        rval = "Stopping";
                        break;
                    case TimerState.Stopped:
                        rval = "Stopped";
                        break;
                    default:
                        rval = "Unknown";
                        break;
                }
                return rval;
            }
        }

        public int Interval {
            get { return TimerInterv; }
            set { 
                TimerInterv = value;
                if (TimerInterv <= 0)
                    TimerInterv = 1000;
                Timer.Enabled = false;
                Timer.Interval = TimerInterv;
                Timer.Enabled = true;
            }
        }
        #endregion

        /** <summary>Startup code simply changes the state.  The state is then changed in the timer event.</summary> */
        public void Start() {
            if (TimerInterv > 0) {
                switch (state) {
                    case TimerState.Created:
                    case TimerState.Stopping:
                    case TimerState.Stopped:
                        state = TimerState.Starting;
                        break;
                    default:
                        break;
                }
            }
        }

        /** <summary>We just set the state to stopping, the state is then changed to stopped at the timer event.</summary> */
        public void Stop() {
            if (state == TimerState.Running)
                state = TimerState.Stopping;
        }

        /** <summary>Initialize timer class and enable.</summary> */
        public TimerClass() {

            TimeToRunWorker = false;
            IdleTime = 2 * 60 * 1000; // 2 minutes in ms (milliseconds)
            TimeUntilRun = 0;
            WorkerRunning = false;
            SleepTime = 1000;

            Timer = new System.Timers.Timer();
            TimerInterv = 5000;
            Timer.Interval = TimerInterv;
            Timer.AutoReset = false;
            Timer.Elapsed += OnTimedEvent;
            state = TimerState.Created;
            ticker = null;
            Timer.Enabled = true;

            clocktick = 0;

        }

        /** <summary>This is the actual timer event.  Based on the state, the user supplied ticker() is/isnot called. </summary> */
        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            clocktick++;
            switch (state) {
                case TimerState.Starting:
                case TimerState.Running:
                    state = TimerState.Running;
                    try {
                        ticker();
                    } finally { }
                    break;

                case TimerState.Stopping:
                case TimerState.Stopped:
                    state = TimerState.Stopped;
                    break;

                case TimerState.Created:
                default:
                    break;
            }
            Timer.Enabled = true;
        }
    }
}
