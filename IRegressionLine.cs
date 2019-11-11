using System;
using System.Collections.Generic;
using System.Text;

namespace Where1.stat.Regression
{
    interface IRegressionLine
    {
        double[] Calculate(VectorSet vset);
        double Residual(VectorSet vset, double[] vector);
    }
}
