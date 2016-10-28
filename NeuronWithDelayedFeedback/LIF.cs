using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronWithDelayedFeedback
{
    /// <summary>
    /// Leaky Integrate-and-Fire neuronal model
    /// </summary>
    class LIF : Neuron
    {
        private double voltage;      // membrane potential counted from the resting state, mV
        private double tau;          // membrane time constant, s
        private double threshold;    // membrane voltage threshold, mV
        private double epsp;         // an altitude of input impulse, an analogue of EPSP, mV

        public LIF(double threshold, double tau, double epsp)
        {
            this.threshold = threshold;
            this.tau = tau;
            this.epsp = epsp;
            timeFromLastSpike = 0;

            voltage = 0;
        }

        /// <summary>
        /// Updates the membrane voltage after arrival of input impulse,
        /// checks if the threshold is reached
        /// and makes the neuron fire, if positive
        /// </summary>
        public override void ObtainImpulse()
        {

            if (voltage + epsp >= threshold)
            {
                voltage = 0;
                pprevISI = prevISI;
                prevISI = isi;
                isi = timeFromLastSpike;
                timeFromLastSpike = 0;

                GenerateSpike();
            }
            else voltage = voltage + epsp;

        }

        /// <summary>
        /// Updates the membrane voltage after a given time interval.
        /// </summary>
        /// <param name="timeInterval"></param>
        public override void UpdateState(double timeInterval)
        {

            if (voltage > 0)
                voltage = voltage * Math.Exp(-timeInterval / tau);

            timeFromLastSpike = timeFromLastSpike + timeInterval;
        }
    }
}
