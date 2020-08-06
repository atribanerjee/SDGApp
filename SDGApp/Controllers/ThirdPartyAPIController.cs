using SDGApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDGApp.Controllers
{
    public class ThirdPartyAPIController : ApiController
    {
        DeviceServicesModel DSM;

        public ThirdPartyAPIController()
        {
            DSM = new DeviceServicesModel();
        }


        [HttpGet]
        [Route("api/ActivatePublicKey")]
        public  HttpResponseMessage ActivatePublicKey(String Email, String PublicKey, String ConfirmationCode)
        {
            if(!String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(PublicKey) && !String.IsNullOrEmpty(ConfirmationCode))
            {
                if (DSM.CheckEmailID(Email))
                {
                    if (DSM.ActivateKey(Email, PublicKey, ConfirmationCode))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "You are verified successfully.");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid public key or confirmation code!");
                    }
                    
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Email id is not registered or invalid!");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Either the email, the public key or the confirmation code is not supplied!");
            }                        
        }

        [HttpGet]
        [Route("api/GetMeasurmentData")]
        public HttpResponseMessage GetMeasurmentData(String PublicKey, String UserCode, DateTime FromDate, DateTime ToDate, int PageIndex, int PageSize)
        {
            
            if (!String.IsNullOrEmpty(PublicKey) && (!String.IsNullOrEmpty(UserCode)) && FromDate!=null && ToDate!=null && PageIndex>0 && PageSize>0)
            {
                if (DSM.CheckAPIAccessKey(PublicKey))
                {
                    var result= DSM.GetMeasurmentDtlsByUserID(PublicKey, UserCode, FromDate, ToDate, PageIndex, PageSize);

                    if(result!=null )
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "No data found.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized access!");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "The PublicKey or UserCode or FromDate or ToDate or PageIndex or PageSize not supplied!");
            }
        }

    }

}
