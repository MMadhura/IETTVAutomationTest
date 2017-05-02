using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marklogic.Xcc;

namespace IETTVAdminportal.Reusable
{
    public class MarkLogicHelper
    {
        #region ISearchResultService Members

        //Session, ModuleInvoke, Requestoption are the classes of MarkLogic.XCC

        private Session session;
        private ModuleInvoke request;
        private RequestOptions options;

        public string GetMarkLogicResponse(string moduleUri, string uri, string xml)
        {
            Uri serverUri = new Uri(uri); //"xcc://admin:admin@rav-dsk-294.rave-tech.co.in:9501"

            //Todo : to be linked to new server
            setXCCRequestHandle(serverUri);
            String searchResponse = InvokeToSingleString(moduleUri, "\t", xml);
            Console.WriteLine("Search Response :: " + searchResponse);
            return searchResponse;
        }

        public virtual String InvokeToSingleString(String moduleUri, String separator, String inputSearchDetails)
        {
            ResultSequence resultSequence = null;
            string strResponse = "";
            try
            {
                resultSequence = Invoke(moduleUri, inputSearchDetails);
                if (resultSequence != null)
                    strResponse = resultSequence.AsString(separator);
                if (resultSequence != null)
                    resultSequence.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strResponse;
        }
        public virtual ResultSequence Invoke(String moduleUri, String inputSearchDetails)
        {
            try
            {
                request.ModuleUri = moduleUri;
                request.Options = options;
                ResultSequence resultSequence = null;
                request.SetNewStringVariable("inputSearchDetails", inputSearchDetails);
                resultSequence = session.SubmitRequest(request);
                return resultSequence;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void setXCCRequestHandle(Uri serverUri)
        {
            try
            {
                ContentSource cs = ContentSourceFactory.NewContentSource(serverUri);
                session = cs.NewSession();
                request = session.NewModuleInvoke(null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
