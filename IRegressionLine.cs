using System;
using System.Collections.Generic;
using System.Text;

namespace Where1.wstat.Regression
{
    interface IRegressionLine
    {
        double[] Calculate(VectorSet vset);
        double Residual(VectorSet vset, double[] vector);
        double CoefficientOfDetermination(VectorSet vset);//r-squared
    }
}
