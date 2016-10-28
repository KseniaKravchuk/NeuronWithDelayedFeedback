using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuronWithDelayedFeedback
{
    class Calculator
    {

        private double[] prob;       // ISI distribution over bins, P(t)
        private double[] probtt;     // ISI distribution over bins provided previous ISI was equal t1, P(t|t1)
        private double[] probttt;    // ISI distribution over bins provided previous ISIs were equal t1 and t0, P(t|t1,t0)
        private double M1;     // the first moment of ISI distribution
        private double M2;     // the second moment of ISI distribution
        private double CV;     // the variation coefficient 

        private double counter;  // a counter for spikes generated
        private double counter2; // a counter for spikes generated under condition |t1
        private double counter3; // a counter for spikes generated under condition |t1,t0

        private int binN;      // the number of bins ISI distribution is composed of
        private double MaxISI; // the max value of an ISI taken to calculate the distribution


        public Calculator()
        {
            binN = 300;
            MaxISI = 0.1;

            prob = new double[binN];
            probtt = new double[binN];
            probttt = new double[binN];

            for (int i = 0; i < binN; i++)
            {
                prob[i] = 0;
                probtt[i] = 0;
                probttt[i] = 0;
            }

            counter = 0;
            counter2 = 0;
            counter3 = 0;
            M1 = 0;
            M2 = 0;
            CV = 0;

        }

        public void UpdateDistribution(Neuron neuron, double t1, double t0)
        {
            int j = 0;

            j = (int)(neuron.ISI * binN / MaxISI + 0.5);
            if (j < binN) prob[j]++;

            M1 = M1 + neuron.ISI;
            M2 = M2 + (neuron.ISI * neuron.ISI);

            counter++;

            //if the condition |t1 is satisfied, accounts ISI in the distribution P(t|t1)
            if ((neuron.PrevISI >= t1 - 0.5 * MaxISI / binN) && (neuron.PrevISI < t1 + 0.5 * MaxISI / binN))
            {
                j = (int)(neuron.ISI * binN / MaxISI + 0.5);

                if (j < binN) probtt[j]++;
                counter2++;

                //if the condition |t1,t0 is satisfied, accounts ISI in the distribution P(t|t1,t0)
                if ((neuron.PPrevISI >= t0 - 0.5 * MaxISI / binN) && (neuron.PPrevISI < t0 + 0.5 * MaxISI / binN))
                {
                    j = (int)(neuron.ISI * binN / MaxISI + 0.5);

                    if (j < binN) probttt[j]++;
                    counter3++;
                }
            }
        }

        internal bool Continue(long N)
        {
            if (counter < N) /* (counter3 < N3)*/
                return true;
            else
                return false;
        }

        internal void NormalizeDistr(long N, long N2, long N3)
        {
            for (int i = 0; i < binN; i++)
            {
                prob[i] = prob[i] * binN / (MaxISI * N);
                probtt[i] = probtt[i] * binN / (MaxISI * N2);
                probttt[i] = probttt[i] * binN / (MaxISI * N3);
            }

            M1 = M1 / counter;
            M2 = M2 / counter;

            CV = Math.Sqrt(M2 - M1 * M1) / M1;

        }

        internal void SaveToFile()
        {
            string[] dataArray = new string[binN];
            double isi;

            for (int i = 0; i < binN; i++)
            {
                isi = Math.Round(i * MaxISI / binN, 5);
                dataArray[i] = isi + "\t\t\t"
                          + prob[i] + "\t\t\t"
                          + Math.Round(probTheor(isi, Data.tau, Data.epsp, Data.threshold, Data.lambda, Data.delta),3) + "\t\t\t"
                          + probtt[i] + "\t\t\t"
                          + probttt[i] + "\t\t\t";
            }

            File.WriteAllLines("prob.dat", dataArray);

            string data = "" + Data.lambda + "\t\t\t"
                             + M1 + "\t\t\t"
                             + M2 + "\t\t\t"
                             + CV + "\r\n";

            File.AppendAllText("variation.dat", data);
        }

        private double probTheor(double isi, double tauLIF, double epsp, double threshold, double lambda, double delta)
        {
            double P = 0, a, K0, K1, K2, T2, T3;
            double tau = tauLIF;
            double h = epsp;
            double V0 = threshold;
            double l = lambda;
            double D = delta;

            T2 = tau * (Math.Log(h) - Math.Log(V0 - h));
            T3 = tau * (Math.Log(V0) - Math.Log(V0 - h));

            a = 4 * Math.Exp(2 * l * D) / ((2 * l * D + 3) * Math.Exp(2 * l * D) + 1);

            // when t < Delta
            if (isi < D - 0.5 * MaxISI / binN)
            {
                P = 0.25 * a * l * Math.Exp(-l * isi) * (l * isi * (2 * l * D + 7) 
                    + Math.Exp(-2 * l * D) - (l * isi + 1) * Math.Exp(-2 * l * (D - isi)) - 2 * l * l * isi * isi);
            }

            // when t = Delta
            if ((isi >= D - 0.5 * MaxISI / binN) && (isi < D + 0.5 * MaxISI / binN))
                P = a * l * D * Math.Exp(-l * D) * binN / MaxISI;

            // when Delta < t < T2
            if ((isi >= D + 0.5 * MaxISI / binN) && (isi < T2 + 0.5 * MaxISI / binN))
                P = l * Math.Exp(-l * isi);

            //when T2 < t < Delta + T2
            if ((isi >= T2 + 0.5 * MaxISI / binN) && (isi < D + T2 + 0.5 * MaxISI / binN))
            {
                K0 = 2 * l * l * T2 * T2 + 4 * l * T2 + 4 * l * D + 6 + (1 - 2 * l * T2) * Math.Exp(-2 * l * D);
                K1 = (2 * Math.Exp(-2 * l * D) - 4 * (1 + l * T2)) * l;
                K2 = 2 * l * l;
                P = 0.125 * a * l * Math.Exp(-l * isi) * (K0 + K1 * isi + K2 * Math.Pow(isi,2) + Math.Exp(2 * l * (isi - T2 - D)));
            }

            //when t > D + T2
            if (isi >= D + T2 + 0.5 * MaxISI / binN)
            {
                int m;
                m = Convert.ToInt32(Math.Floor((isi - D - T2) / T3));

                P = 0.5 * l * Math.Exp(-l * isi) * (
                0.5 * l * l * (isi - T2) * (isi - T2) - 0.5 * l * l * (isi - D - T2) * (isi - D - T2)
                + 0.5 * Math.Exp(-2 * l * D) * (l * (isi - T2) + 0.5)
                - 0.5 * (l * (isi - D - T2) + 0.5)
                + 2 * (isi - D - T2) * l);

                P = a * P;

            }
 

            return P;
        }
    }
}
