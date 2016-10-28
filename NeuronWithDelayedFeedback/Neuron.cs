using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronWithDelayedFeedback
{
    public delegate void NeuronEvent();
    internal delegate void NE(Neuron n, double a, double b);

    abstract class Neuron
    {
        protected double timeFromLastSpike; // time elapsed from the moment of last spike
        protected double isi;               // current value of inter-spike interval (ISI)
        protected double prevISI;           // previous value of ISI
        protected double pprevISI;          // the second last value of ISI
        protected int counter = 0;          // counter for the number of spikes generated

        public double ISI { get { return isi; } }
        public int Counter { get { return counter; } set { counter = value; } }
        public double PrevISI { get { return prevISI; } }
        public double PPrevISI { get { return pprevISI; } }

        /// <summary>
        /// The neuron has generated a spike.
        /// </summary>
        public event NeuronEvent SpikeGenerated;
        /// <summary>
        /// The neuron has generated a spike.
        /// </summary>
        public event NE SpikeFired;


        public abstract void ObtainImpulse();
        public abstract void UpdateState(double timeInterval);

        /// <summary>
        /// Invokes the SpikeGenerated event.
        /// </summary>
        protected void GenerateSpike()
        {
            SpikeGenerated.Invoke();
            SpikeFired.Invoke(this, Data.t1, Data.t0);
        }
    }
}
