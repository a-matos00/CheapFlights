using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CheapFlights.Domain.Entities;

namespace CheapFlights.Domain.Validation
{
    public static class SearchParametersValidator
    {
        private static HashSet<string> validIataCodes;

        const int MaxPassengerCount = 9;
        const int IATA_CODE_LENGTH = 3;

        static SearchParametersValidator()
        {
            LoadIataCodes();
        }

        private static void LoadIataCodes()
        {
            try
            {
                string jsonContent = File.ReadAllText("Domain\\Validation\\Data\\iata_codes.json");
                var codes = JsonSerializer.Deserialize<string[]>(jsonContent);
                validIataCodes = new HashSet<string>(codes, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize IATA codes", ex);
            }
        }

        public static ValidationResult ValidateIataCode(string code, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ValidationResult("Airport code cannot be empty");
            }

            if (code.Length != IATA_CODE_LENGTH)
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
            if (passengers.AdultsCount + passengers.ChildCount + passengers.InfantCount > MaxPassengerCount)
            {
                return new ValidationResult($"Total passenger count cannot exceed {MaxPassengerCount}.");
            }

            if (passengers.InfantCount > passengers.AdultsCount)
            {
                return new ValidationResult("Number of infants cannot exceed the number of adults.");
            }

            return ValidationResult.Success;
        }
    }
}