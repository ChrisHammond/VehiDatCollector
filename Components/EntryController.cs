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
    class EntryController
    {
        public void CreateEntry(Entry t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Entry>();
                rep.Insert(t);
            }
        }

        public void DeleteEntry(int EntryId, int moduleId)
        {
            var t = GetEntry(EntryId, moduleId);
            DeleteEntry(t);
        }

        public void DeleteEntry(Entry t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Entry>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Entry> GetEntries(int moduleId)
        {
            IEnumerable<Entry> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Entry>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public Entry GetEntry(int EntryId, int moduleId)
        {
            Entry t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Entry>();
                t = rep.GetById(EntryId, moduleId);
            }
            return t;
        }

        public void UpdateEntry(Entry t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Entry>();
                rep.Update(t);
            }
        }

    }
}
