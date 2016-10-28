using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

namespace NeuronWithDelayedFeedback
{
    /// <summary>
    /// A class for producing a sequence of input ISIs with Poisson distribution
    /// of a given intensity
    /// </summary>
    class Generator
    {
        private Exponential generator;
        private double interval;
        private double intensity; // intencity of a Poisson process

        /// <summary>
        /// The constructor takes an intensity of the Poisson process as an input parameter.
        /// </summary>
        /// <param name="intensity"></param>
        public Generator(double intensity)
        {
            generator = new Exponential(intensity);
            interval = -1;
            this.intensity = intensity;

        }

        /// <summary>
        /// generates a random value with Poisson
        /// distribution which models an input ISI
        /// </summary>
        public double GenerateInterval()
        {
                interval = Exponential.Sample(intensity);                

            return interval;
        }

        /// <summary>
        /// returns a time step until the next input signal
        /// (from the Poisson process or from the feedback line)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public double GetTimeStep(double s)
        {
            double step;

            GenerateInterval();

            if (s <= 0) step = interval;
	        else step = ( interval > s)? s: interval;

        	interval = interval -step;
   
            return step;
        }
    }
}
