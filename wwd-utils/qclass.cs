using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace wwd_utils {

    public enum qstate {
        NoState = 0,
        Waiting = 1,
        Running = 2,
        Completed,
        NoEntry,
        RunOK,
        RunFailed,
        CompletedWithError
    };

    public class qclass {

        public int id {
            get;
            set;
        }

        public qstate State {
            get;
            set;
        }

        public BackgroundWorker worker {
            get;
            set;
        }

        public qclass(int Id) {
            id = Id;
            State = qstate.NoState;
            worker = new BackgroundWorker();
        }

        ~qclass() {
            if (State == qstate.Running) {
                while (State == qstate.Running) {
                    Take_a_Nap();
                }
            }
        }

        private void Take_a_Nap() {
            System.Threading.Thread.Sleep(1000);
        }
    }
}