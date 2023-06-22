using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KillerSodukoLambdaV2.models;
using KillerSodukoLambdaV2.Services;
using Newtonsoft.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace KillerSodukoLambdaV2.Controllers
{
    
    public class ValuesController : ControllerBase
    {
        // GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    Console.WriteLine("Get called");
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    Console.WriteLine("Get ID called");
        //    return "value";
        //}

        // POST api/values
        //List<KillerSodukoSection> sections
        [HttpPost]
        [Route("api/[controller]")]
        public async Task<List<string>> Post([FromBody] List<KillerSodukoSection> killerSodukoSections)
        {
            try
            {
                var dataToLog = JsonConvert.SerializeObject(killerSodukoSections);

                Console.WriteLine($"request({dataToLog})");
                try
                {
                    Request.Headers.TryGetValue("sender-ip", out var ipAddress);
                    var result = await doLogging(new LogData { Data = $"{ipAddress} {dataToLog}" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"exception ({ex.Message}) stack({ex.StackTrace})");
                }
                List<string> output = KillerSodukoCalcService.calculateUniqueComboSets(killerSodukoSections).OrderBy(_ => _).ToList();
                Console.WriteLine($"response({JsonConvert.SerializeObject(output)})");

                return output;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception ({ex.Message}) stack({ex.StackTrace})");
            }
            List<string> defaultReturn = new List<string> { new string("default return")};
            return defaultReturn;

        }

        // POST api/values
        //List<KillerSodukoSection> sections
        [HttpPost]
        [Route("api/[controller]/contact")]
        public async Task<PutObjectResponse> Post([FromBody] ContactData contactData)
        {
            try
            {
                Console.WriteLine($"contactData request({JsonConvert.SerializeObject(contactData)})");

                DateTime currentDT = DateTime.Now;

                string path = $"{currentDT.ToString("yyyy-MM-dd-HH-mm-")}{Guid.NewGuid()}";
                Console.WriteLine($"path({path})");

                using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = "killersudokucontactdata",
                        Key = path,
                        ContentBody = JsonConvert.SerializeObject(contactData)
                    };
                    
                    return await client.PutObjectAsync(request);

                    //Console.WriteLine($"done with S3 part 1");
                    //Console.WriteLine($"done with S3 write response {response}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"contactData exception ({ex.Message}) stack({ex.StackTrace})");
            }
            return null;
                        
        }


        [HttpPost]
        [Route("api/[controller]/log")]
        public async Task<PutObjectResponse> Log([FromBody] LogData logData)
        {
            try
            {
                return await doLogging(logData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Post Activity Log exception ({ex.Message}) stack({ex.StackTrace})");
            }
            return null;

        }

        private async Task<PutObjectResponse> doLogging(LogData logData)
        {
            DateTime currentDT = DateTime.Now;
            var currentDateEST = currentDT.AddHours(-4);

            string responseBody = "";
            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
            {
                //Console.WriteLine($"activity/log create getObjectRequest");
                var currentContentsReq = new GetObjectRequest
                {
                    BucketName = "killersudokucontactdata",
                    Key = "activityLog"
                };

                //Console.WriteLine($"activity/log do GetObjectAsync");
                var currentContents = await client.GetObjectAsync(currentContentsReq);

                //Console.WriteLine($"activity/log create stream");
                var currentContentsStream = currentContents.ResponseStream;

                //Console.WriteLine($"activity/log create reader");
                var reader = new StreamReader(currentContentsStream);
                {
                    //Console.WriteLine($"activity/log read reader");
                    responseBody = reader.ReadToEnd() + Environment.NewLine;

                    //Console.WriteLine($"activity/log responseBody({responseBody})");
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = "killersudokucontactdata",
                        Key = "activityLog",
                        ContentBody = string.Concat(responseBody, currentDateEST.ToString("yyyy-MM-dd hh:mm:ss tt") + ", " + logData.Data)
                    };
                    //Console.WriteLine($"activity/log PutObjectAsync");
                    //currentContentsReq.ContentBody = string.Concat(responseBody, logData.Data);
                    return await client.PutObjectAsync(putRequest);
                }
                //return await client.PutObjectAsync(request);

            }
        }
    }



    public class LogData
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }

}
