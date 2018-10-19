using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.helpers
{
    // General extension helpers
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            // Add the error message to the header
            response.Headers.Add("Application-Error", message);
            // Expose the header to the client
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if (dateTime.AddYears(age) > DateTime.Today) // Haven't had bday yet this year
            {
                age --;
            }
            return age;
        }
    }
}