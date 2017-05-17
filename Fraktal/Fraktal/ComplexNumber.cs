using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraktal
{
    class ComplexNumber
    {
        public double Real;
        public double Imaginary;
        public ComplexNumber(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }
        public ComplexNumber squared()
        {
            double newReal = Real * Real - Imaginary * Imaginary;
            double newImaginary = Real * Imaginary + Imaginary * Real;
            return new ComplexNumber(newReal, newImaginary);
        }
        public static ComplexNumber operator +(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }
    }
}
