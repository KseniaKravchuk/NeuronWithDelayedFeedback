using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronWithDelayedFeedback
{
    /// <summary>
    /// An interconnection line between neurons.
    /// </summary>
    class Axon
    {
        private double s = -1;     // time left for impulse to reach the end of an axon, 0 <= s <= delay, if there is no impulse: s = -1; sec
        private double delay = 0;  // time needed for impulse to pass the whole axon, sec

        public double S { get { return s; } }

        /// <summary>
        /// Spike reached the end of an axon.
        /// </summary>
        public event NeuronEvent SpikeReleased;

        public Axon(double delay)
        {
            this.delay = delay;
        }

        public Axon()
        {
        }

        public void ObtainImpulse(double interval)
        {
            //if axon already keeps an impulse, it does not accept the new one
            if (s > 0) return;

            // an impulse enters empty axon and has time to live equal to the given time interval
            s = interval;

        }

        /// <summary>
        /// Decides, whether to obtain a new impulse or not, 
        /// and sets the impulse to its start position, if obtained 
        /// </summary>
        public void ObtainImpulse()
        {
            //if axon already keeps an impulse, it does not accept the new one
            if (s > 0) return;

            // an impulse enters empty axon and has time to live equal to delay
            s = delay;

        }

        /// <summary>
        ///  updates the position of impulse in axon after a given time interval
        /// </summary>
        /// <param name="time"></param>
        public void UpdateState(double time)
        {
            // if there is no impulse in axon -- nothing to do
            if (s < 0) return;

            s = s - time;
            if (s == 0)
                ReleaseImpulse();
        }

        private void ReleaseImpulse()
        {
            SpikeReleased.Invoke();
        }
    }
}
