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
using DotNetNuke.Entities.Users;
using Christoc.Modules.VehiDataCollector.Components;
using DotNetNuke.Services.Exceptions;

namespace Christoc.Modules.VehiDataCollector
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from VehiDataCollectorModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Edit : VehiDataCollectorModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    //get a list of users to assign the user to the Object
                    ddlVehicleOwnerUserId.DataSource = UserController.GetUsers(PortalId);
                    ddlVehicleOwnerUserId.DataTextField = "Username";
                    ddlVehicleOwnerUserId.DataValueField = "UserId";
                    ddlVehicleOwnerUserId.DataBind();

                    //check if we have an ID passed in via a querystring parameter, if so, load that item to edit.
                    //VehicleId is defined in the ItemModuleBase.cs file
                    if (VehicleId > 0)
                    {
                        var tc = new VehicleController();

                        var t = tc.GetVehicle(VehicleId, ModuleId);
                        if (t != null)
                        {
                            txtName.Text = t.VehicleName;
                            txtDescription.Text = t.VehicleDescription;
                            ddlVehicleOwnerUserId.Items.FindByValue(t.VehicleOwnerUserId.ToString()).Selected = true;
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var t = new Vehicle();
            var tc = new VehicleController();

            if (VehicleId > 0)
            {
                t = tc.GetVehicle(VehicleId, ModuleId);
                t.VehicleName = txtName.Text.Trim();
                t.VehicleDescription = txtDescription.Text.Trim();
                t.LastModifiedByUserId = UserId;
                t.LastModifiedOnDate = DateTime.Now;
                t.VehicleOwnerUserId = Convert.ToInt32(ddlVehicleOwnerUserId.SelectedValue);
            }
            else
            {
                t = new Vehicle()
                {
                    VehicleOwnerUserId = Convert.ToInt32(ddlVehicleOwnerUserId.SelectedValue),
                    CreatedByUserId = UserId,
                    CreatedOnDate = DateTime.Now,
                    VehicleName = txtName.Text.Trim(),
                    VehicleDescription = txtDescription.Text.Trim(),

                };
            }

            t.LastModifiedOnDate = DateTime.Now;
            t.LastModifiedByUserId = UserId;
            t.ModuleId = ModuleId;

            if (t.VehicleId > 0)
            {
                tc.UpdateVehicle(t);
            }
            else
            {
                tc.CreateVehicle(t);
            }
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }
    }
}