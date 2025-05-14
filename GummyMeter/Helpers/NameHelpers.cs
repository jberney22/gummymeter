using System.Text;

namespace GummyMeter.Helpers
{
    public static class NameHelpers
    {
        /// <summary>
        /// Returns the concatenated uppercase initials of each word in the name.
        /// E.g. "john doe" => "JD", "Mary-Jane Smith" => "MJS"
        /// </summary>
        public static string GetInitials(this string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            // Split on whitespace, discard empty entries
            var parts = fullName
                .Trim()
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            var initials = new StringBuilder(parts.Length);

            foreach (var part in parts)
            {
                // If you want to handle hyphenated names like "Mary-Jane"
                // you could further split on '-' and take each sub-part's initial:
                var subParts = part.Split('-', StringSplitOptions.RemoveEmptyEntries);
                foreach (var sub in subParts)
                {
                    initials.Append(char.ToUpperInvariant(sub[0]));
                }
            }

            return initials.ToString();
        }
    }

}
