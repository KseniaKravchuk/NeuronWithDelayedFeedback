using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuronWithDelayedFeedback
{
    class Program
    {

        //                             axon11
        //                   ________________________
        //                  |     _______________    |
        //                  |    |               |   |
        //                  |___\|               |___|
        //                      /|    neuron1    |_________________\  output
        //        axon01         |               |                 /
        //    __________________\|               |
        //      (Poisson)       /|_______________|
        //

        /// <summary>
        /// Chooses which time interval from s0 and s1
        /// will finish earlier.
        /// 
        /// If s1 is negative, returns s0,
        /// else returns min value from (s0, s1).
        /// </summary>
        /// <param name="s0"></param>
        /// <param name="s1"></param>
        /// <returns></returns>
        static double GetCloserMoment(double s0,double s1)
        {
            if (s1 <= 0)
                return  s0;
            else return (s0 > s1) ? s1 : s0;
        }

        static void Main(string[] args)
        {

            double timeStep;
            Calculator calculator = new Calculator();
            Generator generator = new Generator(Data.lambda);

            LIF neuron1 = new LIF(Data.threshold, Data.tau, Data.epsp);
            Axon axon01 = new Axon();             //axon for Poissonian input (0) to neuron1
            Axon axon11 = new Axon(Data.delta);   //axon from neuron1 to neuron1 (feedback line)
            
            neuron1.SpikeGenerated += axon11.ObtainImpulse;
            axon11.SpikeReleased += neuron1.ObtainImpulse;
            axon01.SpikeReleased += neuron1.ObtainImpulse;
            neuron1.SpikeFired += calculator.UpdateDistribution;

            do
            {
                axon01.ObtainImpulse(generator.GenerateInterval());

                // brings the system to the moment just before the next input signal:
                timeStep = GetCloserMoment(axon01.S,axon11.S);
                neuron1.UpdateState(timeStep);
                axon01.UpdateState(timeStep);
                axon11.UpdateState(timeStep);

            }
            while (calculator.Continue(Data.N));

            calculator.NormalizeDistr(Data.N, Data.N2, Data.N3);

            calculator.SaveToFile();

            Console.WriteLine("Finished.");
            Console.ReadKey();

        }

    }
}
