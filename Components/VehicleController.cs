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
using System.Collections.Generic;
using DotNetNuke.Data;

namespace Christoc.Modules.VehiDataCollector.Components
{
    class VehicleController
    {
        public void CreateVehicle(Vehicle t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                rep.Insert(t);
            }
        }

        public void DeleteVehicle(int VehicleId, int moduleId)
        {
            var t = GetVehicle(VehicleId, moduleId);
            DeleteVehicle(t);
        }

        public void DeleteVehicle(Vehicle t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Vehicle> GetVehicles(int moduleId)
        {
            IEnumerable<Vehicle> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public Vehicle GetVehicle(int VehicleId, int moduleId)
        {
            Vehicle t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                t = rep.GetById(VehicleId, moduleId);
            }
            return t;
        }

        public void UpdateVehicle(Vehicle t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Vehicle>();
                rep.Update(t);
            }
        }

    }
}
