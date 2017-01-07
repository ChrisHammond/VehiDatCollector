/*
' Copyright (c) 2017 Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;

namespace Christoc.Modules.VehiDataCollector.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for VehiDataCollector
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FeatureController : ModuleSearchBase, IPortable, IUpgradeable
    {
        // feel free to remove any interfaces that you don't wish to use
        // (requires that you also update the .dnn manifest file)

        #region Optional Interfaces

        /// <summary>
        /// Gets the modified search documents for the DNN search engine indexer.
        /// </summary>
        /// <param name="moduleInfo">The module information.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();
            var controller = new VehicleController();
            var vehicles = controller.GetVehicles(moduleInfo.ModuleID);

            foreach (var vehicle in vehicles)
            {
                if (vehicle.LastModifiedOnDate.ToUniversalTime() <= beginDate.ToUniversalTime() ||
                    vehicle.LastModifiedOnDate.ToUniversalTime() >= DateTime.UtcNow)
                    continue;

                var content = string.Format("{0}<br />{1}", vehicle.VehicleName, vehicle.VehicleDescription);

                var searchDocumnet = new SearchDocument
                {
                    UniqueKey = string.Format("Vehicles:{0}:{1}", moduleInfo.ModuleID, vehicle.VehicleId),  // any unique identifier to be able to query for your individual record
                    PortalId = moduleInfo.PortalID,  // the PortalID
                    TabId = moduleInfo.TabID, // the TabID
                    AuthorUserId = vehicle.LastModifiedByUserId, // the person who created the content
                    Title = moduleInfo.ModuleTitle,  // the title of the content, but should be the module title
                    Description = moduleInfo.DesktopModule.Description,  // the description or summary of the content
                    Body = content,  // the long form of your content
                    ModifiedTimeUtc = vehicle.LastModifiedOnDate.ToUniversalTime(),  // a time stamp for the search results page
                    CultureCode = moduleInfo.CultureCode, // the current culture code
                    IsActive = true  // allows you to remove the vehicle from the search index (great for soft deletes)
                };

                searchDocuments.Add(searchDocumnet);
            }

            return searchDocuments;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be exported</param>
        /// -----------------------------------------------------------------------------
        public string ExportModule(int moduleId)
        {
            var controller = new VehicleController();
            var vehicles = controller.GetVehicles(moduleId);
            var sb = new StringBuilder();

            var vehicleList = vehicles as IList<Vehicle> ?? vehicles.ToList();

            if (!vehicleList.Any()) return string.Empty;

            sb.Append("<Vehicles>");

            foreach (Vehicle vehicle in vehicleList)
            {
                sb.Append("<Vehicle>");

                sb.AppendFormat("<VehicleOwnerUserId>{0}</VehicleOwnerUserId>", vehicle.VehicleOwnerUserId);
                sb.AppendFormat("<CreatedByUserId>{0}</CreatedByUserId>", vehicle.CreatedByUserId);
                sb.AppendFormat("<CreatedOnDate>{0}</CreatedOnDate>", vehicle.CreatedOnDate);
                sb.AppendFormat("<VehicleId>{0}</VehicleId>", vehicle.VehicleId);
                sb.AppendFormat("<VehicleDescription>{0}</VehicleDescription>", XmlUtils.XMLEncode(vehicle.VehicleDescription));
                sb.AppendFormat("<VehicleName>{0}</VehicleName>", XmlUtils.XMLEncode(vehicle.VehicleName));
                sb.AppendFormat("<LastModifiedByUserId>{0}</LastModifiedByUserId>", vehicle.LastModifiedByUserId);
                sb.AppendFormat("<LastModifiedOnDate>{0}</LastModifiedOnDate>", vehicle.LastModifiedOnDate);
                sb.AppendFormat("<ModuleId>{0}</ModuleId>", vehicle.ModuleId);

                sb.Append("</Vehicle>");
            }

            sb.Append("</Vehicles>");

            // you might consider doing something similar here for any important module settings

            return sb.ToString();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be imported</param>
        /// <param name="content">The content to be imported</param>
        /// <param name="version">The version of the module to be imported</param>
        /// <param name="userId">The Id of the user performing the import</param>
        /// -----------------------------------------------------------------------------
        public void ImportModule(int moduleId, string content, string version, int userId)
        {
            var controller = new VehicleController();
            var vehicles = DotNetNuke.Common.Globals.GetContent(content, "Vehicles");
            var xmlNodeList = vehicles.SelectNodes("Vehicle");

            if (xmlNodeList == null) return;

            foreach (XmlNode vehicle in xmlNodeList)
            {
                var newVehicle = new Vehicle()
                {
                    ModuleId = moduleId,
                    // assigning everything to the current UserID, because this might be a new DNN installation
                    // your use case might be different though
                    CreatedByUserId = userId,
                    LastModifiedByUserId = userId,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedOnDate = DateTime.Now
                };

                // NOTE: If moving from one installation to another, this user will not exist
                newVehicle.VehicleOwnerUserId = int.Parse(vehicle.SelectSingleNode("VehicleOwnerUserId").InnerText, NumberStyles.Integer);
                newVehicle.VehicleDescription = vehicle.SelectSingleNode("VehicleDescription").InnerText;
                newVehicle.VehicleName = vehicle.SelectSingleNode("VehicleName").InnerText;

                controller.CreateVehicle(newVehicle);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeModule implements the IUpgradeable Interface
        /// </summary>
        /// <param name="version">The current version of the module</param>
        /// -----------------------------------------------------------------------------
        public string UpgradeModule(string version)
        {
            try
            {
                switch (version)
                {
                    case "00.00.01":
                        // run your custom code here
                        return "success";
                    default:
                        return "success";
                }
            }
            catch
            {
                return "failure";
            }
        }

        #endregion
    }
}