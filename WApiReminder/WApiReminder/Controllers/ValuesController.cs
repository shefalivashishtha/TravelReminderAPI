using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WApiReminder.Models;

namespace WApiReminder.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values/SetReminder
        // Set reminder basis the input fetched as serialized JSON object of type Class Reminder
        [HttpPost]
        public bool SetReminder([FromBody]ClsReminder objParamReminder)
        {
            try
            {
                bool bResult;
                ClsReminder objReminder = new ClsReminder();

                if (objReminder.AuthenticateClient(objParamReminder.ApiKey))
                {

                    objReminder.AccessToken = objParamReminder.AccessToken.Trim();
                    objReminder.Summary = "Reminder to leave for " + objParamReminder.Location;
                    objReminder.Location = objParamReminder.Location;
                    objReminder.BoradingPointPostalCode = objParamReminder.BoradingPointPostalCode;
                    objReminder.SourcePostalCode = objParamReminder.SourcePostalCode;
                    objReminder.StartDate = objReminder.GetEventDate(objParamReminder.TravelDate);
                    objReminder.Description = "Reminder to leave for " + objParamReminder.Location;



                    if (objParamReminder.ModeOfTravel.ToString().ToUpper().Trim() == "DRIVING")
                    {
                        objReminder.ModeOfTravel = Google.Maps.TravelMode.driving;
                    }
                    else if (objParamReminder.ModeOfTravel.ToString().ToUpper().Trim() == "WALKING")
                    {
                        objReminder.ModeOfTravel = Google.Maps.TravelMode.walking;
                    }
                    else if (objParamReminder.ModeOfTravel.ToString().ToUpper().Trim() == "BICYCLING")
                    {
                        objReminder.ModeOfTravel = Google.Maps.TravelMode.bicycling;
                    }

                    bResult = objReminder.SetReminder(objReminder);
                    return bResult;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Writes the API key in the authetication file to register client
        [HttpGet]
        public bool GetApiKey([FromUri]string sLicenseKey)
        {
            try
            {
                ClsReminder objReminder = new ClsReminder();
                bool bResult = objReminder.AuthorizeUser(sLicenseKey);
                return bResult; 
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
