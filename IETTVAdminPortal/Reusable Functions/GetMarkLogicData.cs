using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
//using java.math;

namespace IETTVAdminportal.Reusable
{
    class GetMarkLogicData
    {
        MarkLogicHelper markLogicHelper = new MarkLogicHelper();

        string connetionString = null;

        String userID = null;

        public string connecDBGetUserID(String userName)
        {

                connetionString = "Data Source=RAV-DSK-395;Initial Catalog=IET_TV_SP4;uid=sa;password=R@ve1234";

                String getUserID = "select Id from Users where Username='" + userName + "'";

                Console.WriteLine("Query:" + getUserID);

                SqlConnection cnn = new SqlConnection(connetionString);

                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    SqlCommand commandforExecution = new SqlCommand(getUserID, cnn);

                     try
                     {   
                         cnn.Open();

                         Object objUserID = commandforExecution.ExecuteScalar();

                         userID = objUserID.ToString();

                     }
                    catch(Exception e)
                     {
                        Console.WriteLine(e.StackTrace + e.Message);
                     }
               
                }

                cnn.Close();

                cnn.Dispose();

                return userID;

        }

        public string getMyVideos(string UserID)
        {
            string searchResponse = "";

             try
             {
                 //Here we are calling to MarkLogicHelper class and passing three arguments
                 // 1. QUeryName
                 // 2. URL (Values of URL is retrieved from app.config file)
                 // 3. Search Parameters. 

                 Console.WriteLine("Inside get my videos");

                 ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();

                 String configPath = AppDomain.CurrentDomain.BaseDirectory + "AdminPortal.config";

                 Console.WriteLine("Configpath:" + configPath);

                 configMap.ExeConfigFilename = configPath;

                 Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);                         
            
                 searchResponse = markLogicHelper.GetMarkLogicResponse("get_admin_video_by_criteria.xqy",
                    "xcc://" + config.AppSettings.Settings["MLUser"].Value.ToString() + ":"
                     + config.AppSettings.Settings["MLPassword"].Value.ToString()
                     + "@" + config.AppSettings.Settings["MLServer"].Value.ToString()
                    + ":" + config.AppSettings.Settings["xdbcPort"].Value.ToString(), CreateXMLInputForGetMyVideos(UserID));

             }
             catch (NullReferenceException nullReferenceException)
             {

                 throw nullReferenceException;
             }
             catch (Exception ex)
             {
                 throw ex;
             }

             return searchResponse;            
            
        }

        string CreateXMLInputForGetMyVideos(string UserID)
        {
            System.Text.StringBuilder videoListingRecent = new System.Text.StringBuilder();

            //Here we are defining the search parameters.
            videoListingRecent.Append("<?xml version='1.0' encoding='UTF-8'?>");
            videoListingRecent.Append("<Video xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>");
            videoListingRecent.Append("<UserID>" + UserID + "</UserID>");                                         // Need to take this value from database
            videoListingRecent.Append("<TopList>" + "5" + "</TopList>");
            videoListingRecent.Append("<SearchKey>" + "Title" + "</SearchKey>");
            videoListingRecent.Append("<SearchValue>**</SearchValue>");
            videoListingRecent.Append("<VideoType>" + "NonPromo" + "</VideoType>");
            videoListingRecent.Append("<PageLength>" + "10" + "</PageLength>");
            videoListingRecent.Append("<StartPage>" + "1" + "</StartPage>");
            videoListingRecent.Append("<IsAbused>" + "no" + "</IsAbused>");
            videoListingRecent.Append("</Video>");

            Console.WriteLine("Parameters ::: " + videoListingRecent.ToString());

            return videoListingRecent.ToString();
        }
    }
}
