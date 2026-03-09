using System;

namespace lab30vN
{
    public class LoanCalculator
    {
        public double CalculateMonthlyPayment(double loanAmount, double annualRate, int months)
        {
            if (loanAmount <= 0)
                throw new ArgumentException("Loan amount must be positive");

            if (annualRate < 0)
                throw new ArgumentException("Rate cannot be negative");

            if (months <= 0)
                throw new ArgumentException("Months must be greater than 0");

            double monthlyRate = annualRate / 12 / 100;

            if (monthlyRate == 0)
                return loanAmount / months;

            double payment = loanAmount * monthlyRate /
                            (1 - Math.Pow(1 + monthlyRate, -months));

            return payment;
        }
    }
}