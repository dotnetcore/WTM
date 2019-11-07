using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WalkingTec.Mvvm.Core.Auth
{
    /// <summary>
    /// Compares two instances of Claim
    /// </summary>
    public class ClaimComparer : EqualityComparer<Claim>
    {
        /// <summary>
        /// Claim comparison options
        /// </summary>
        public class Options
        {
            /// <summary>
            /// Specifies if the issuer value is being taken into account
            /// </summary>
            public bool IgnoreIssuer { get; set; } = false;

            /// <summary>
            /// Specifies if claim and issuer value comparison should be case-sensitive
            /// </summary>
            public bool IgnoreValueCase { get; set; } = false;
        }

        private readonly Options _options = new Options();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class with default options.
        /// </summary>
        public ClaimComparer()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class with given comparison options.
        /// </summary>
        /// <param name="options">Comparison options.</param>
        public ClaimComparer(Options options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public override bool Equals(Claim x, Claim y)
        {
            if (x == null && y == null) return true;
            if (x == null && y != null) return false;
            if (x != null && y == null) return false;

            StringComparison valueComparison = StringComparison.Ordinal;
            if (_options.IgnoreValueCase == true) valueComparison = StringComparison.OrdinalIgnoreCase;

            var equal = (String.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase) &&
                         String.Equals(x.Value, y.Value, valueComparison) &&
                         String.Equals(x.ValueType, y.ValueType, StringComparison.Ordinal));


            if (_options.IgnoreIssuer)
            {
                return equal;
            }
            else
            {
                return (equal && String.Equals(x.Issuer, y.Issuer, valueComparison));
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode(Claim claim)
        {
            if (claim is null) return 0;

            int typeHash = claim.Type?.ToLowerInvariant().GetHashCode() ?? 0 ^ claim.ValueType?.GetHashCode() ?? 0;
            int valueHash;
            int issuerHash;

            if (_options.IgnoreValueCase)
            {
                valueHash = claim.Value?.ToLowerInvariant().GetHashCode() ?? 0;
                issuerHash = claim.Issuer?.ToLowerInvariant().GetHashCode() ?? 0;
            }
            else
            {
                valueHash = claim.Value?.GetHashCode() ?? 0;
                issuerHash = claim.Issuer?.GetHashCode() ?? 0;
            }

            if (_options.IgnoreIssuer)
            {
                return typeHash ^ valueHash;

            }
            else
            {
                return typeHash ^ valueHash ^ issuerHash;
            }
        }
    }

}
