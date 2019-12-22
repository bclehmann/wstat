using MathNet.Numerics;
using System;
using System.Collections.Generic;

namespace Where1.wstat.Regression
{
    class LinearRegressionLine : IRegressionLine
    {
        public double[] Calculate(VectorSet vset)
        {
            if (vset.Dimensions < 2)
            {
                throw new NotSupportedException("linreg only makes sense on vector spaces (2+ dimensions)");
            }

            if (vset.Dimensions == 2)
            {//As dimensions go up, closed form solutions become sparse, but for 2-D, it isn't so bad, and its more precise
                double a = 1;
                double b;

                double sumXYResidual = 0;
                double sumXSquareResidual = 0;



                for (int i = 0; i < vset.Length; i++)
                {
                    sumXYResidual += (vset.Vectors[i][0] - vset.DataSets[0].Mean) * (vset.Vectors[i][1] - vset.DataSets[1].Mean);
                    sumXSquareResidual += Math.Pow((vset.Vectors[i][0] - vset.DataSets[0].Mean), 2);
                }

                b = sumXYResidual / (sumXSquareResidual);

                a = vset.DataSets[1].Mean - (b * vset.DataSets[0].Mean);//LSRL always passes through the point (x̅,y̅)

                return new double[] { a, b };

            }
            else
            {
                List<double[]> inputX = new List<double[]>();
                double[] inputY = new double[vset.Length];
                for (int i = 0; i < vset.Length; i++)
                {
                    for (int j = 0; j < vset.Dimensions; j++)
                    {
                        if (j != vset.Dimensions - 1)
                        {
                            if (j == 0) {
                                inputX.Add(new double[vset.Dimensions - 1]);
                            }
                            inputX[i][j] = vset.Vectors[i][j];
                        }
                        else
                        {
                            inputY[i] = vset.Vectors[i][j];
                        }
                    }
                }


                return Fit.MultiDim(inputX.ToArray(), inputY, intercept: true);
            }
        }

        public double CoefficientOfDetermination(VectorSet vset)
        {
            double SS_total = 0;
            double SS_residual = 0;

            VectorSet residSet = vset.ResidualSet(this);
            

            for(int i=0; i< vset.Vectors.Count; i++) {
                //Sum of (yi - ̅y̅)^2
                SS_total += Math.Pow((vset.Vectors[i][vset.Dimensions - 1] - vset.DataSets[vset.Dimensions - 1].Mean), 2);

                SS_residual += Math.Pow((residSet.Vectors[i][vset.Dimensions - 1]), 2);
            }

            return 1 - (SS_residual / SS_total);
        }

        public double Residual(VectorSet vset, double[] vector)
        {
            double[] coefficients = Calculate(vset);
            double residual = vector[vector.Length - 1];

            for (int i = 0; i < coefficients.Length; i++) {
                if (i == 0)
                {
                    residual -= coefficients[i];
                }
                else {
                    residual -= coefficients[i] * vector[i - 1];
                }
            }
            
            return residual;
        }
    }
}
