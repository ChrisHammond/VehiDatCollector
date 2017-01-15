//
// Christoc.com - http://www.christoc.com
// Copyright (c) 2017
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
using System.Web.Http;
using DotNetNuke.Instrumentation;
using Christoc.Modules.VehiDataCollector.Components;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Web.Api;

//TODO: create a postman collection

namespace Christoc.Modules.VehiDataCollector.Services
{
    public class VehiDataCollectorController : DnnApiController
    {
        //URL to get the list of vehicles
        //http://dnndev.me/DesktopModules/VehiDataCollector/API/VehiDataCollector.ashx/GetVehicles?moduleid=4428

        [HttpGet]
        //[DnnAuthorize()]
        //TODO: make it so we can call without ModuleId to get a list of vehicles
        [AllowAnonymous]
        public HttpResponseMessage GetVehicles()
        {
            try
            {
                var mc = new ModuleController();
                var mi = mc.GetModuleByDefinition(0, "VehiDataCollector"); //TODO: assuming PortalId=0 if moduleid =0 
                return GetVehicles(mi.ModuleID); //TODO: remove this hardcoded id
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Module Not On A Page, or No Vehicles Exist"); //todo: probably should localize that?
            }
        }

        [HttpGet]
        //[DnnAuthorize(AllowAnonymous = true)]
        [AllowAnonymous]
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

        [HttpGet]
        //Sample vehicle call http://dnndev.me/DesktopModules/VehiDataCollector/API/VehiDataCollector.ashx/GetVehicle?vehicleid=1&moduleid=4428
        //[DnnAuthorize(AllowAnonymous = true)]
        public HttpResponseMessage GetVehicle(int vehicleId, int moduleId)
        {
            try
            {
                var vc = new VehicleController();
                var v = vc.GetVehicle(vehicleId, moduleId); //TODO: figure out ModuleId
                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "error in request"); //todo: probably should localize that?
            }
        }


        //TODO: This shouldn't allow anonymous calls
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage CreateEntry(Entry e)
        {
            try
            {
                /* we're taking in an entry, and want to update the basics */
                e.CreatedOnDate = DateTime.UtcNow;
                e.CreatedByUserId = -1;
                e.LastModifiedOnDate = DateTime.UtcNow;
                e.LastModifiedByUserId = -1;

                /* if we don't have a moduleId coming in, let's look it up */
                if (e.ModuleId < 1)
                {
                    //look up module
                    var mc = new ModuleController();
                    var mi = mc.GetModuleByDefinition(0, "VehiDataCollector"); //TODO: assuming PortalId=0 if moduleid =0 
                    if (mi != null)
                    {
                        e.ModuleId = mi.ModuleID;
                    }
                }
                var vc = new EntryController();
                vc.CreateEntry(e);

                return Request.CreateResponse(HttpStatusCode.OK, "valid");

            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "error in request"); //todo: probably should localize that?
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage CreateEntry(int vehicleId, string entryName, string entryDescription, string entrySource)
        {
            try
            {
                var e = new Entry();
                e.EntryName = entryName;
                e.EntryDescription = entryDescription;
                e.EntrySource = entrySource;
                e.CreatedOnDate = DateTime.UtcNow;
                e.CreatedByUserId = -1;
                e.LastModifiedOnDate = DateTime.UtcNow;
                e.VehicleId = vehicleId;

                var vc = new EntryController();
                vc.CreateEntry(e);

                return Request.CreateResponse(HttpStatusCode.OK, "valid");

            }
            catch (Exception exc)
            {
                DnnLog.Error(exc); //todo: obsolete
                return Request.CreateResponse(HttpStatusCode.BadRequest, "error in request"); //todo: probably should localize that?
            }
        }


    }
}