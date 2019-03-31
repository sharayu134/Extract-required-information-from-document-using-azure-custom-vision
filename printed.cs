using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ClassLibrary14;
using System.Text;
using System.Threading;

namespace CSHttpClientSample
{
    static class printed
    {

        //Account number And IFSC number find
    
        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "a02378056afc41c886ff93c7f5cbb17a";

      
        const string uriBase =
            "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

        static void Main()
        {
           
           //input
            string imageFilePath = @"D:\as.jpg";

               // MakeOCRRequest(imageFilePath).Wait();

            string sdn = MakeOCRRequest(imageFilePath).Result;
           // Console.WriteLine(sdn);
            
            var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(sdn);

            var regions = oMycustomclassname.regions;

            StringBuilder AC = new StringBuilder();
            StringBuilder IFSC = new StringBuilder();

            foreach (var region in regions)
            {

                foreach (var line in region.lines)
                {
                    int k = 0;
                    int counter = 0;
                    foreach (var word in line.words)
                    {
                        var txt = word.text;

                        //For Acoount Number 
                        if (txt.Contains("A/C"))
                        {

                            foreach (var word1 in line.words)
                            {

                                AC.Append(word1.text);
                            }

                        }
                        //for IFSC Number 

                        counter++;


                        int len = 0;
                        if (txt.Contains("IFSC"))
                        {
                            k = counter;

                            foreach (var word2 in line.words)
                            {
                                len++;
                            }

                        }

                        for (int g = counter - 1; g < len; g++)
                        {
                            IFSC.Append(line.words[g].text);
                        }

                    }

                }
            }

            Console.WriteLine(AC);
            Console.WriteLine(IFSC);

            Thread.Sleep(100000);


        }

       

        /// <param name="imageFilePath">The image file with printed text.</param>
        static async Task<string> MakeOCRRequest(string imageFilePath)
        {
            
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. 
                // The language parameter doesn't specify a language, so the 
                // method detects it automatically.
                // The detectOrientation parameter is set to true, so the method detects and
                // and corrects text orientation before detecting text.
                string requestParameters = "language=unk&detectOrientation=true";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;
                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // Asynchronously get the JSON response.
                string contentString; // = await response.Content.ReadAsStringAsync();

                return (contentString = await response.Content.ReadAsStringAsync());

               

          
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);


                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}