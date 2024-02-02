using Sang.CommonLibraries.String;
using System.Net.Http.Json;

namespace Sang.Baidu.TranslateAPI
{
    /// <summary>
    /// 文档：https://fanyi-api.baidu.com/doc/21
    /// </summary>
    public class BaiduTranslator
    {
        /// <summary>
        /// APPID
        /// </summary>
        private string _appId;

        /// <summary>
        /// 密钥
        /// </summary>
        private string _secretKey;

        /// <summary>
        ///  翻译服务终结点
        /// </summary>
        private readonly string _endpoint;

        /// <summary>
        /// HttpClient
        /// </summary>
        private readonly HttpClient _httpClient;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <param name="secretKey">密钥</param>
        /// <param name="endpoint">翻译服务终结点</param>
        public BaiduTranslator(string appId, string secretKey,string endpoint = "https://fanyi-api.baidu.com/api/trans/vip/translate")
        {
            _appId = appId;
            _secretKey = secretKey;
            _endpoint = endpoint;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) }; // 设置超时时间
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <param name="secretKey">密钥</param>
        /// <param name="httpClient">HttpClient</param>
        /// <param name="endpoint">翻译服务终结点</param>
        public BaiduTranslator(string appId, string secretKey, HttpClient httpClient, string endpoint = "https://fanyi-api.baidu.com/api/trans/vip/translate")
        {
            _appId = appId;
            _secretKey = secretKey;
            _endpoint = endpoint;
            _httpClient = httpClient;
        }

        /// <summary>
        /// 重新设置appId 和 secretKey
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <param name="secretKey">密钥</param>
        public void SetAppIdAndSecretKey(string appId, string secretKey)
        {
            _appId = appId;
            _secretKey = secretKey;
        }

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="text">请求翻译文本，长度控制在 6000 bytes以内（汉字约为输入参数 2000 个）</param>
        /// <param name="toLanguage">翻译目标语言</param>
        /// <returns></returns>
        public async Task<BaiduTranslateResult> Translate(string text, string toLanguage)
        {
            return await Translate(text, toLanguage, "auto");
        }


        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="text">请求翻译文本，长度控制在 6000 bytes以内（汉字约为输入参数 2000 个）</param>
        /// <param name="toLanguage">翻译目标语言</param>
        /// <param name="fromLanguage">翻译源语言，可为auto，自动检测</param>
        /// <param name="salt">设置后将使用传入的随机数</param>
        /// <param name="sign">设置后将使用传入的签名结果</param>
        /// <returns></returns>
        public async Task<BaiduTranslateResult> Translate(string text, string toLanguage, string fromLanguage = "auto",string salt = "",string sign = "")
        {
            // 使用时间戳
            if (string.IsNullOrWhiteSpace(salt))
            {
                salt = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            }
            // 使用MD5加密
            if (string.IsNullOrWhiteSpace(sign))
            {
                sign = (_appId + text + salt + _secretKey).MD5();
            }
            // 拼接URL
            string url = $"{_endpoint}?q={text}&from={fromLanguage}&to={toLanguage}&appid={_appId}&salt={salt}&sign={sign}";
            // 发送请求
            try
            {
                return await _httpClient.GetFromJsonAsync<BaiduTranslateResult>(url);
            }
            catch (Exception ex)
            {
                return new BaiduTranslateResult { Error_Code = "Exception", Error_Msg = ex.Message };
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

    }

    public class BaiduTranslateResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success => Error_Code == null;
        /// <summary>
        /// 翻译源语言
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 翻译目标语言
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 翻译结果
        /// </summary>
        public List<TransResult> Trans_Result { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public string Error_Code { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Msg { get; set; }

        /// <summary>
        /// 获取翻译结果
        /// </summary>
        /// <returns></returns>
        public string GetResult()
        {
            return Trans_Result != null && Trans_Result.Count > 0 ? Trans_Result[0].Dst : null;
        }

    }

    /// <summary>
    /// 翻译结果
    /// </summary>
    public class TransResult
    {
        /// <summary>
        /// 原文
        /// </summary>
        public string Src { get; set; }
        /// <summary>
        /// 译文
        /// </summary>
        public string Dst { get; set; }
    }
}
