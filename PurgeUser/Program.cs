using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CETYPE;
using NextLabs.CSCInvoke;

namespace PurgeUser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid number of arguments. One argument specifying the user to purge must be specified");
                Environment.Exit(0);
            }

            IntPtr connectHandle = IntPtr.Zero;


            var app = new CETYPE.CEApplication(null, "purge_app", null);
            var user = new CETYPE.CEUser("purge_client", "purge_client");

            CEResult_t result = CESDKAPI.CECONN_Initialize(app, user, "127.0.0.1", out connectHandle, 5000);

            Console.WriteLine("PC response : " + result.ToString());

            if (result != CEResult_t.CE_RESULT_SUCCESS)
            {
                Console.WriteLine("Could not connect to PC.");
                Environment.Exit(0);
            }

            Console.WriteLine("Connected to PC.");


            String userId = args[0];

            Console.WriteLine("Request to purge user " + userId + " from cache sent...");

            CETYPE.CERequest[] requests = new CETYPE.CERequest[1];
            requests[0] = new CETYPE.CERequest("PURGE",
                                         new CETYPE.CEResource("random", "fso"),
                                         null,
                                         null,
                                         null,
                                         new CETYPE.CEUser(userId, userId),
                                         new string[] { "purge", "yes" },
                                         new CETYPE.CEApplication("purge_app", "purge_app", null),
                                         null,
                                         null,
                                         null,
                                         true,
                                         CETYPE.CENoiseLevel_t.CE_NOISE_LEVEL_USER_ACTION);

            CETYPE.CEEnforcement[] enfs = new CETYPE.CEEnforcement[requests.Length];

            CETYPE.CEResult_t res = CESDKAPI.CEEVALUATE_CheckResourcesEx(connectHandle,
                                                                   requests,
                                                                   "POLICY X FOR * ON PURGE BY USER.TRIGGERPURGE < 0 DO ALLOW",
                                                                   true,
                                                                   0,
                                                                   out enfs,
                                                                   30000);

            Console.WriteLine("Request sent successfully. Please check the agent log for any error occurred");


        }



    } 

}
