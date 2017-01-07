//
// Christoc.com - http://www.christoc.com
// Copyright (c) 2014-2017
// by Christoc.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using DotNetNuke.Instrumentation;
using Christoc.Modules.VehiDataCollector.Components;
//using Christoc.Modules.VehiDataCollector.ViewModels;
using DotNetNuke.Web.Api;

namespace Christoc.Modules.VehiDataCollector.Services
{
    class VehiDataCollectorController : DnnApiController
    {
        [DnnAuthorize()]
        public HttpResponseMessage GetVehicles()
        {
            return GetVehicles(ActiveModule.ModuleID);
        }

        //[DnnAuthorize(AllowAnonymous = true)]
        public HttpResponseMessage GetVehicles(int moduleId)
        {
            try
            {
                //todo: get the latest X vehicles?
                var vc = new VehicleController();
                
                var vehicles = vc.GetVehicles(moduleId);

                //because of the circular reference when cerealizing the taxonomy within content items we have to build out our article view models manually.
                var cleanVehicles = new List<Vehicle>();
                foreach (Vehicle v in vehicles)
                {
                    
                    cleanVehicles.Add(v);
                }

                return Request.CreateResponse(HttpStatusCode.OK, cleanVehicles, new MediaTypeHeaderValue("text/json"));
                
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "error in request"); //todo: probably should localize that?
            }
        }
        
        
        //[DnnAuthorize(AllowAnonymous = true)]
        public HttpResponseMessage GetVehicle(int vehicleId)
        {
            try
            {var vc = new VehicleController();
                var v = vc.GetVehicle(vehicleId,0); //TODO: figure out ModuleId

                return Request.CreateResponse(HttpStatusCode.OK, v);

            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "error in request"); //todo: probably should localize that?

            }
        }

        //[DnnAuthorize(AllowAnonymous = true)]
        public HttpResponseMessage PutEntry(int vehicleId,string entryName, string entryDescription, string entrySource)
        {
            try
            {
                var e = new Entry();
                e.EntryDescription = entryDescription;
                e.EntrySource = entrySource;
                e.CreatedOnDate = DateTime.UtcNow;
                e.CreatedByUserId = -1;
                e.LastModifiedOnDate = DateTime.UtcNow;
                e.VehicleId = vehicleId;

                var vc = new EntryController();
                vc.CreateEntry(e);
                
                return Request.CreateResponse(HttpStatusCode.OK,"valid");

            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "error in request"); //todo: probably should localize that?
            }
        }
    }
}