using System;
using System.Collections.Generic;
using System.Text;

namespace Where1.wstat.Distribution
{
    public static class NormalDistribution
    {
        public static double Erf(double x)
        {

            //The definite integral from 0 to x of e^(-t^2) * dt
            //dt is taken to be the differential of the independent variable, as x is taken
            double erfIntegral = MathNet.Numerics.Integration.GaussLegendreRule.Integrate(t => Math.Pow(Math.E, -Math.Pow(t, 2)), 0, x, 64);


            //The real function is 2/sqrt(Pi) * erfIntegral
            double erfCoefficient = 2.0 / Math.Sqrt(Math.PI);

            return erfCoefficient * erfIntegral;
        }

        public static double Cdf(double x)
        {
            //erf and cdf are related
            //cdf= 1/2 * (1 + erf((x/sqrt(2)))
            return 0.5 * (1 + Erf(x / Math.Sqrt(2.0)));
        }

        public static double InvCdf(double x)//This function is normally given in terms of InvErf, however that function is horrible, so we're doing it this over/under way
        {
            double estimate = 0;
            double shiftBy = 1;
            while (Math.Abs(Cdf(estimate) - x) > 0.000000001)//This precision is probably optimistic, given that the precision of the integral is, ambiguous, not to mention floating point
            {
                double error = Math.Abs(Cdf(estimate) - x);
                if (error > Math.Abs(Cdf(estimate - shiftBy) - x))
                {
                    estimate -= shiftBy;
                }
                else if (error > Math.Abs(Cdf(estimate + shiftBy) - x))
                {
                    estimate += shiftBy;
                }
                else
                {
                    shiftBy /= 2.0;
                }
            }

            return estimate;
        }
    }
}
