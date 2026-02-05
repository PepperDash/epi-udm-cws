using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Static helper for parsing and validating PATCH requests
    /// Per API spec: Only standard.state and standard.activity are writable
    /// apiVersion is required in PATCH requests
    /// </summary>
    public static class PatchRequestParser
    {
        /// <summary>
        /// Parses a PATCH request JSON body and validates that only writable properties are present
        /// </summary>
        /// <param name="jsonBody">The JSON body from the PATCH request</param>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>Parsed State object, or null if validation fails</returns>
        public static State ParsePatchRequest(string jsonBody, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(jsonBody))
            {
                errorMessage = "Request body is empty";
                return null;
            }

            try
            {
                // Parse JSON to JObject for validation
                var jObject = JObject.Parse(jsonBody);

                // Validate apiVersion is present (required per API spec)
                if (jObject["apiVersion"] == null)
                {
                    errorMessage = "apiVersion is required in PATCH requests";
                    return null;
                }

                // Validate that only writable properties are present
                // Per API spec: only standard.state and standard.activity are writable
                if (!ValidateWritableProperties(jObject, out errorMessage))
                {
                    return null;
                }

                // Deserialize to State object
                var state = JsonConvert.DeserializeObject<State>(jsonBody);
                return state;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error parsing request: {ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Validates that only writable properties are present in the PATCH request
        /// Per API spec: Only standard.state and standard.activity are writable
        /// </summary>
        private static bool ValidateWritableProperties(JObject jObject, out string errorMessage)
        {
            errorMessage = null;

            // Check for status (read-only)
            if (jObject["status"] != null)
            {
                errorMessage = "status properties are read-only and cannot be modified";
                return false;
            }

            // Check for custom (read-only)
            if (jObject["custom"] != null)
            {
                errorMessage = "custom properties are read-only and cannot be modified";
                return false;
            }

            // Check for standard
            var standard = jObject["standard"] as JObject;
            if (standard != null)
            {
                // Only "state" and "activity" are writable within standard
                foreach (var property in standard.Properties())
                {
                    if (property.Name != "state" && property.Name != "activity")
                    {
                        errorMessage = $"standard.{property.Name} is read-only. Only state and activity are writable";
                        return false;
                    }
                }

                // Ensure at least one writable property is present
                if (standard["state"] == null && standard["activity"] == null)
                {
                    errorMessage = "No writable properties provided. Only standard.state and standard.activity are writable";
                    return false;
                }
            }
            else
            {
                // No standard object provided
                errorMessage = "standard is required. Only standard.state and standard.activity are writable";
                return false;
            }

            return true;
        }
    }
}
