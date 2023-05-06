// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace SpaceHelmet.Client.Gravatar.Models {
    public class Name {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Formatted { get; set; }

        public Name() {
            GivenName = String.Empty;
            FamilyName = String.Empty;
            Formatted = String.Empty;
        }
    }
}
