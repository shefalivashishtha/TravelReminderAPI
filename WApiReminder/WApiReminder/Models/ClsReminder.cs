using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Maps;
using Google.Apis.Util.Store;
using System.Threading;
using System.IO;
using System.Net;
using System.Data;
using System.Xml;
using System.Configuration;
using Newtonsoft.Json;

namespace WApiReminder.Models
{
    public class ClsReminder
    {

        #region Properties

        private string _accessToken;
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        private string _apiKey;

        public string ApiKey
        {
            get { return _apiKey; }
            set { _apiKey = value; }
        }


        private string _summary;
        public string Summary
        {
            get { return _summary; }
            set { _summary = value; }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private EventDateTime _startDate;
        public EventDateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }


        private DateTime _travelDate;
        public DateTime TravelDate
        {
            get { return _travelDate; }
            set { _travelDate = value; }
        }

        private int _reminderDuration;
        public int ReminderDuration
        {
            get { return _reminderDuration; }
            set { _reminderDuration = value; }
        }

        private string _sourcePostalCode;
        public string SourcePostalCode
        {
            get { return _sourcePostalCode; }
            set { _sourcePostalCode = value; }
        }

        private string _boardingPointPostalCode;
        public string BoradingPointPostalCode
        {
            get { return _boardingPointPostalCode; }
            set { _boardingPointPostalCode = value; }
        }

        private Google.Maps.TravelMode _modeOfTravel;
        private  IClock clock;

        public Google.Maps.TravelMode ModeOfTravel
        {
            get { return _modeOfTravel; }
            set { _modeOfTravel = value; }
        }

        #endregion

        #region Functions

        public bool SetReminder(ClsReminder objReminder)
        {

            try
            {

                UserCredential credential = null;


                ClientSecrets clsec = new ClientSecrets
                {
                    ClientId = ConfigurationManager.AppSettings.Get("CalendarAPIKey"), //"658810886525-8eq69ms22roa4rlvajqid8ttpn5bom9c.apps.googleusercontent.com",
                    ClientSecret =ConfigurationManager.AppSettings.Get("CalendarAPISecret")  //"ntIQcF32vDXHrxAT-Ivko83D"
                };

                var _apiCredential = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clsec,
                    Scopes = new[]{ "https://www.googleapis.com/auth/calendar" }
                    
                });

                

                credential = new UserCredential(_apiCredential, Environment.UserName, new TokenResponse
                {
                    AccessToken = objReminder.AccessToken,
                    ExpiresInSeconds = 500000,
                    Scope = "https://www.googleapis.com/auth/calendar",
                    RefreshToken = GetRefreshToken(objReminder.AccessToken)


                });

                //credential.RefreshTokenAsync(CancellationToken.None);
               // string[] Scopes = { "https://www.googleapis.com/auth/calendar", "https://www.googleapis.com/auth/userinfo.profile" };

                //credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                //    clsec,
                //    Scopes,
                //    "user",
                //    CancellationToken.None,
                //    null).Result;

                if (credential != null)
                {
                    CalendarService calendarService = new CalendarService(new BaseClientService.Initializer()
                    {

                        HttpClientInitializer = credential,
                        ApplicationName = "Reminder"

                    });

                    objReminder.ReminderDuration = GetDuration(this.SourcePostalCode, this.BoradingPointPostalCode, this.ModeOfTravel);
                    //IList<CalendarListEntry> calList = calendarService.CalendarList.List().Execute().Items;
                    //IList<Event> _lstCal = calendarService.Events.List("primary").Execute().Items;

                    //if (calList.Count > 0)
                    //{

                    #region Create Event

                    //EventDateTime eveStTime = new EventDateTime()
                    //{
                    //    DateTime = System.DateTime.Now.AddMinutes(50)
                    //};
                    EventDateTime eveEdTime = new EventDateTime()
                    {
                        DateTime = objReminder.StartDate.DateTime.Value.AddMinutes(objReminder.ReminderDuration)
                    };

                    

                    Event eve = new Event()
                        {
                            Summary = objReminder.Summary,
                            Location = objReminder.Location,
                            Description = objReminder.Description,
                            Start = objReminder.StartDate,
                            End = eveEdTime
                        };


                        //EventAttendee ea1 = new EventAttendee();
                        //ea1.DisplayName = "Shefali";
                        //ea1.Email = "vashishthashefali15@gmail.com";
                        //ea1.Organizer = false;
                        //ea1.Resource = false;
                        //IList<EventAttendee> ealist = new List<EventAttendee>();
                        //ealist.Add(ea1);
                        //eve.Attendees = ealist;

                        // This will create reminder a 1 hour prior to the event as email
                        //EventReminder erbyEmail =
                        //    new EventReminder()
                        //    {
                        //        Method = "email",
                        //        Minutes = 60
                        //    };

                        // This will create a reminder 5 minutes before the event as popup
                        EventReminder erNotify5 = new EventReminder()
                        {
                            Method = "popup",
                            Minutes = (objReminder.ReminderDuration + 5)

                        };

                        // This will create a reminder 15 minutes before the event as popup
                        EventReminder erNotify15 = new EventReminder()
                        {
                            Method = "popup",
                            Minutes = (objReminder.ReminderDuration + 15)
                        };

                        // This will create a reminder 30 minutes before the event as popup
                        EventReminder erNotify30 = new EventReminder()
                        {
                            Method = "popup",
                            Minutes = objReminder.ReminderDuration + 30
                        };

                        // This will create a reminder at the exact time of the event as popup
                        EventReminder erNotify0 = new EventReminder()
                        {
                            Method = "popup",
                            Minutes = objReminder.ReminderDuration
                        };

                        Event.RemindersData erdata = new Event.RemindersData
                        {
                            UseDefault = false,
                            Overrides = new[]{
                                erNotify5,erNotify15
                    }
                        };


                    objReminder.clock = SystemClock.Default;
                    //if (credential.Token.IsExpired(objReminder.clock))
                    //{
                    //  string  token =  GetRefreshToken(credential.Token.AccessToken);

                    //    credential = new UserCredential(_apiCredential, Environment.UserName, new TokenResponse
                    //    {
                    //        AccessToken = objReminder.AccessToken,
                    //        RefreshToken = token
                            

                    //    });

                    //    calendarService = new CalendarService(new BaseClientService.Initializer()
                    //    {

                    //        HttpClientInitializer = credential,
                    //        ApplicationName = "Reminder"

                    //    });

                    //}

                    
                        eve.Reminders = erdata;
                        eve = calendarService.Events.Insert(eve, "primary").Execute();


                       // var calList = calendarService.CalendarList.List().Execute().Items;

                        #endregion

                        return true;
                    //}
                    //else
                    //{
                    //    return false;
                    //}
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }
        
        protected int GetDuration(string sSource, string sBoardingPoint, Google.Maps.TravelMode gTravelMode)
        {
            try
            {

                string wbUrl = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + sSource + "&destinations=" + sBoardingPoint + "&travelMode = " + gTravelMode + " & sensor = false&key="+ ConfigurationManager.AppSettings.Get("DistanceAPIKey");
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(wbUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string sDuration = "";
                DataSet dsResponse = new DataSet();
                dsResponse.ReadXml(response.GetResponseStream());
                if (dsResponse != null)
                {
                    if (dsResponse.Tables.Count > 0)
                    {
                        sDuration = dsResponse.Tables["duration"].Rows[0]["text"].ToString();
                    }
                }

                int iResult = GetMinutes(sDuration);
                return iResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected int GetMinutes(string sInput)
        {
            string sOutput = "";
            sOutput = sInput.Remove(sInput.IndexOf('m'), 4).Trim();
            int iResult = Convert.ToInt32(sOutput);
            return iResult;
        }

        public EventDateTime GetEventDate(DateTime dtInput)
        {
            try
            {
                EventDateTime eveDate = new EventDateTime();
                eveDate.DateTime = dtInput;
                return eveDate;
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        protected string GetRefreshToken(string accessToken)
        {
            try
            {
                string tokenUrl = "https://www.googleapis.com/oauth2/v4/token";
                string gurl = "client_id=658810886525-8eq69ms22roa4rlvajqid8ttpn5bom9c.apps.googleusercontent.com" +
           "&client_secret=ntIQcF32vDXHrxAT-Ivko83D&scope=https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/calendar https://www.googleapis.com/auth/userinfo.email&refresh_token=" + accessToken + "&grant_type=refresh_token";

                string postData = (gurl);

                // create the POST request
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(tokenUrl);
                webRequest.Host = "www.googleapis.com";
                
                webRequest.Headers.Add("Bearer", accessToken);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postData.Length;

                // POST the data
                using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter2.Write(postData);
                }

                //This actually does the request and gets the response back
                HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
                string googleAuth;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    //dumps the HTML from the response into a string variable
                    googleAuth = responseReader.ReadToEnd();
                }

                var models = JsonConvert.DeserializeObject<ClsJsonModel>(googleAuth);

                return models.access_token;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        // Authenticates user
        public bool AuthenticateClient(string sApiKey)
        {
            try
            {
                var sPath = HttpContext.Current.Server.MapPath("~/TxtFlApiKey.txt");
                StreamReader stReader = File.OpenText(sPath);
                string line;
                while((line = stReader.ReadLine()) != null)
                {
                    if(line == sApiKey)
                    {
                        stReader.Close();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        // Authorises user to access API
        public bool AuthorizeUser(string sApiKey)
        {
            try
            {
                var sPath = HttpContext.Current.Server.MapPath("~/TxtFlApiKey.txt");
                
                File.AppendAllLines(sPath,new[] { sApiKey});
                bool bCheck = AuthenticateClient(sApiKey);
                return bCheck;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }

    public class ClsJsonModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}