using System;

namespace lab30vN
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new LoanCalculator();

            double payment = calc.CalculateMonthlyPayment(100000, 10, 12);

            Console.WriteLine($"Monthly payment: {payment:F2}");
        }
    }
}