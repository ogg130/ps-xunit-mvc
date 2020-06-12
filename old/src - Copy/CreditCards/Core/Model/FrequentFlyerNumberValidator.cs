using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CreditCards.Core.Model
{
    /// <summary>
    /// A frequent flyer number consists of 2 parts separated by a '-':
    /// [member number]-[scheme identifier]
    /// Member numbers consist of 6 numeric digits
    /// Scheme identifiers are a single uppercase alphabetic character
    /// </summary>
    public class FrequentFlyerNumberValidator : IFrequentFlyerNumberValidator
    {
        private readonly char[] _validSchemeIdentifiers = { 'A', 'Q', 'Y' };
        private const int ExpectedTotalLength = 8;
        private const int ExpectedMemberNumberLength = 6;

        public bool IsValid(string frequentFlyerNumber)
        {
            // If the frequent flyer number is null, throw ANE
            if (frequentFlyerNumber is null)
            {
                throw new ArgumentNullException(nameof(frequentFlyerNumber));
            }

            // If the frequent flyer number length is not equal to expected total length, return false - invalid
            if (frequentFlyerNumber.Length != ExpectedTotalLength)
            {
                return false;
            }

            // Store memberNumber section of frequent flyer number in a variable
            var memberNumberPart = frequentFlyerNumber.Substring(0, ExpectedMemberNumberLength);


            // If member number section is not an integer return false
            if (!int.TryParse(memberNumberPart, NumberStyles.None, null, out int _))
            {
                return false;
            }


            // If the scheme identifier is correct, return true 
            var schemeIdentifier = frequentFlyerNumber.Last();
            return _validSchemeIdentifiers.Contains(schemeIdentifier);
        }

    }
}
