using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronWithDelayedFeedback
{
    /// <summary>
    /// contains all the parameters for simulation and calculation
    /// </summary>
    static class Data
    {
        public static readonly double lambda = 500;    //an intensity of input Poisson process, 1/sec
        public static readonly double tau = 0.01;      //membrane voltage time constant, sec
        public static readonly double threshold = 20;  //membrane voltage threshod, mV
        public static readonly double epsp = 15;      //an altitude of input impulse, epsp amplitude, mV 
        public static readonly double delta = 0.008;    // delay of an impulse in the feedback line, sec
        public static readonly long N = 1000000;       // the number of impulses to be generated for P(t)
//        public static readonly int binNumber = 300;    // the number of bins in the distribution P(t)


        public static readonly double t1 = 0.015;      // previous value of ISI for conditional p.d.f. P(t|t1) and P(t|t1,t0)
        public static readonly double t0 = 0.010;      // the second last value of ISI for conditional p.d.f. P(t|t1,t0)
        public static readonly long N2 = 30000;        // the number of impulses to be generated for P(t|t1)
        public static readonly long N3 = 30000;        // the number of impulses to be generated for P(t|t1,t0)


    }
}
