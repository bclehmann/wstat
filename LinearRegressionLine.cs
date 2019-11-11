using System;
using System.Collections.Generic;
using System.Text;

namespace Where1.stat.Regression
{
    class LinearRegressionLine : IRegressionLine
    {
        public double[] Calculate(VectorSet vset)
        {
            if (vset.Dimensions != 2)
            {
                throw new NotSupportedException("linreg only supported in two dimensions currently");
            }

            double a = 1;
            double b;

            double sumXYResidual = 0;
            double sumXSquareResidual = 0;


            //double aIncrement = 1;

            for (int i = 0; i < vset.Length; i++)
            {
                sumXYResidual += (vset.Vectors[i][0] - vset.DataSets[0].Mean) * (vset.Vectors[i][1] - vset.DataSets[1].Mean);
                sumXSquareResidual += Math.Pow((vset.Vectors[i][0] - vset.DataSets[0].Mean), 2);
            }

            b = sumXYResidual / (sumXSquareResidual);

            a = vset.DataSets[1].Mean - (b * vset.DataSets[0].Mean);//LSRL always passes through the point (x̅,y̅)

            //while (aIncrement > 2 >> 10)
            //{
            //    double initial = LeastSquareRegression(a, b);
            //    bool changed = false;

            //    if (LeastSquareRegression(a - aIncrement, b) < initial)
            //    {
            //        a -= aIncrement;
            //        changed = true;
            //    }
            //    else if (LeastSquareRegression(a + aIncrement, b) < initial)
            //    {
            //        a += aIncrement;
            //        changed = true;
            //    }

            //    if (!changed)
            //    {

            //        aIncrement /= 2;
            //    }
            //}

            return new double[] { a, b };
        }

        public double Residual(VectorSet vset, double[] vector)
        {
            if (vset.Dimensions != 2)
            {
                throw new NotSupportedException("linreg only supported in two dimensions currently");
            }

            double[] coefficients = Calculate(vset);
            double a = coefficients[0];
            double b = coefficients[1];

            double residual = vector[1] - (vector[0] * b + a);

            return residual;
        }
    }
}
