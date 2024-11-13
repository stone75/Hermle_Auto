using System;
using System.Text;
using System.Net.Http;

// using ActUtlTypeLib;
// using MCProtocol;

public delegate void MessageReceivedHandler(int addr, string message);

namespace HermleCS.Comm
{
//    public class CommHTTPComponent : CommModule
    public class CommHTTPComponent
    {
        private string IP;
        private int Port;
        private static readonly HttpClient httpClient = new HttpClient();

        private string buffer;
        private int errcode;
        private string errmsg;

        public event MessageReceivedHandler MessageReceived;


        private static readonly CommHTTPComponent instance = new CommHTTPComponent();
        private CommHTTPComponent() 
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

//        private Mitsubishi.Plc plc = null;

        public static CommHTTPComponent Instance
        {
            get
            {
                return instance;
            }
        }

        public string GetAPI(string url)
        {
            /*
            try
            {
                MessageReceived?.Invoke(0, "GET API Started..");
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                // 응답 내용을 문자열로 읽어서 반환
                string responseBody = response.Content.ReadAsStringAsync().Result;
                MessageReceived?.Invoke(0, "Response : " + responseBody);

                return responseBody;
            }
            */
            try
            {
                MessageReceived?.Invoke(0, "GET API Started..");
                string responseBody = "";

                Thread t = new Thread(() =>
                {
                    try
                    {
                        MessageReceived?.Invoke(0, "GET API Started in Thread...");
                        using (HttpClient httpClient = new HttpClient())
                        {
                            HttpResponseMessage response = httpClient.GetAsync(url).Result;
                            response.EnsureSuccessStatusCode();

                            string responseBody = response.Content.ReadAsStringAsync().Result;
                            MessageReceived?.Invoke(0, "Response: " + responseBody);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageReceived?.Invoke(1, "Error: " + ex.Message);
                    }
                });
                t.IsBackground = true;
                t.Start();
                // t.Join();

//                MessageReceived?.Invoke(0, "Response : " + responseBody);
                return responseBody;
            }
            catch (Exception ex)
            {
                // 예외 처리
                C.log($"Error occurred: {ex.Message}");
                MessageReceived?.Invoke(0, "GetAPI Exception " + ex.Message);

                return null;
            }
        }

        public string PostAPI(string url, string jsonData)
        {
            string rval = "";

            try
            {
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    rval = response.Content.ReadAsStringAsync().Result;
                    C.log($"응답 성공: {rval}");
                }
                else
                {
                    C.log($"응답 실패: {response.StatusCode}");
                }

                MessageReceived?.Invoke(0, "Response : " + rval);
            }
            catch (Exception ex)
            {
                // 예외 처리
                C.log($"요청 중 오류 발생: {ex.Message}");
                MessageReceived?.Invoke(0, "PostAPI Exception " + ex.Message);

                return null;
            }

            return rval;
        }


        public bool commandAutoStart(string jsondata)
        {
            PostAPI("AUTO_START", jsondata);
            MessageReceived?.Invoke(0, "Auto Start ... ");
            return true;
        }

        public bool commandWritePosition(string jsondata)
        {
            PostAPI("WRITE_POSITION", jsondata);
            MessageReceived?.Invoke(0, "Write Position ... ");
            return true;
        }

        public bool commandMove(string jsondata)
        {
            PostAPI("MOVE", jsondata);
            MessageReceived?.Invoke(0, "Move ... ");
            return true;
        }

        public bool commandCommand(string TPFilename)
        {
            MessageReceived?.Invoke(0, "Command ... " + TPFilename);
            GetAPI(TPFilename);
            return true;
        }

        // 소스 변경부분....
    }
}