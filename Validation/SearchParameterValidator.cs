using CheapFlights.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CheapFlights.Validation
{
    public static class SearchParametersValidator
    {
        private static HashSet<string> validIataCodes;

        static SearchParametersValidator()
        {
            LoadIataCodes();
        }

        private static void LoadIataCodes()
        {
            try
            {
                string jsonContent = File.ReadAllText("iata_codes.json");
                var codes = JsonSerializer.Deserialize<string[]>(jsonContent);
                validIataCodes = new HashSet<string>(codes, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                // Log the error appropriately
                Console.WriteLine($"Error loading IATA codes: {ex.Message}");
                validIataCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public static ValidationResult ValidateIataCode(string code, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ValidationResult("Airport code cannot be empty");
            }

            if (code.Length != 3)
            {
                return new ValidationResult("Airport code must be exactly 3 characters");
            }

            if (!validIataCodes.Contains(code))
            {
                return new ValidationResult($"Invalid airport code: {code}");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateDepartureDate(DateTime departureDate, ValidationContext context)
        {
            if (departureDate.Date < DateTime.Now.Date)
            {
                return new ValidationResult("Departure date cannot be in the past");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateReturnDate(DateTime? returnDate, ValidationContext context)
        {
            var instance = (SearchParameters)context.ObjectInstance;
            
            if (instance.IsOneWay)
            {
                return ValidationResult.Success;
            }

            if (returnDate?.Date < DateTime.Now.Date)
            {
                return new ValidationResult("Return date cannot be in the past");
            }
            if (returnDate?.Date < instance.DepartureDate.Date)
            {
                return new ValidationResult("Return date must be after departure date");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidatePassengerCount(Passengers passengers, ValidationContext context)
        {
            if (passengers.AdultsCount + passengers.ChildCount + passengers.InfantCount > 9)
            {
                return new ValidationResult("Total passenger count cannot exceed 9.");
            }

            if (passengers.InfantCount > passengers.AdultsCount)
            {
                return new ValidationResult("Number of infants cannot exceed the number of adults.");
            }

            return ValidationResult.Success;
        }
    }
}