using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
using UnityEngine;

public class RestBase {
    public TokenVo currentTokenVo;

    private int retryCount = 0;

    public void Post(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
    {
        //LogManager.Log("RestBase.Post(" + uri + ")");

        retryCount = 0;

#if UNITY_EDITOR

        //BMUtil.Instance.StartNetCoroutine(PostReceive(uri, content, responseCallback, uploadStatusCallback));

        if (BMUtil.Instance.isNetDisconnect == true)
        {
            HttpResponseMessage<string> response = new HttpResponseMessage<string>();
            response.Exception = new Exception("Net Error");

            if (uri == new Uri(RestService.ServiceUrl + "/user/action") || uri == new Uri(RestService.ServiceUrl + "/brush"))
            {
                responseCallback(response);
                return;
            }

            retryCount = 4;
            BMUtil.Instance.ReTryNetConnect(PostReceive(uri, content, responseCallback, uploadStatusCallback), retryCount, delegate { responseCallback(response); });
        }
        else
        {
            BMUtil.Instance.StartNetCoroutine(PostReceive(uri, content, responseCallback, uploadStatusCallback));
        }
#else
        BMUtil.Instance.StartNetCoroutine(PostReceive(uri, content, responseCallback, uploadStatusCallback));
#endif

        /*
        HttpClient client = new HttpClient();

        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);

        client.Post(uri, content, response =>
        {
            if (string.IsNullOrEmpty(response.Data))
            {
                responseCallback(response);
            }
            else
            {
                try
                {
                    ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);
                    if (exceptionVo.exception_status_code > 200)
                    {
                        response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        LogManager.LogWarning("RestBase.Post Warning(" + exceptionVo.exception_status_code + ") : " + exceptionVo.message);
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogError("RestBase.Post Error : " + e.Message);
                }
                responseCallback(response);
            }
        }, uploadStatusCallback);
        */
    }

    public IEnumerator PostReceive(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
    {
        retryCount++;

        HttpClient client = new HttpClient();

        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);

        client.Post(uri, content, response =>
        {
            if(response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data.IndexOf("exception_status_code") > -1)
                {
                    try
                    {
                        ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);

                        if (exceptionVo.exception_status_code > 200)
                        {
                            response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        }
                    }
                    catch (Exception e)
                    {
                        LogManager.LogError("RestBase.Post Error : " + e.Message);
                        BMUtil.Instance.ReTryNetConnect(PostReceive(uri, content, responseCallback, uploadStatusCallback), retryCount);
                    }
                }

                responseCallback(response);
                BMUtil.Instance.CloseLoading();
            }
            else
            {
                LogManager.Log("response.StatusCode : " + response.StatusCode);

                if (uri != new Uri(RestService.ServiceUrl+ "/user/action") && uri != new Uri(RestService.ServiceUrl + "/brush"))
                {
                    //네트워크 연결 실패(오류) 메시지
                    BMUtil.Instance.ReTryNetConnect(PostReceive(uri, content, responseCallback, uploadStatusCallback), retryCount, delegate { responseCallback(response); }, response.StatusCode, response.Exception.Message);
                    BMUtil.Instance.CloseLoading();
                }
                else
                {
                    responseCallback(response);
                }
            }
        }, uploadStatusCallback);

        yield return null;
    }

    public void Get(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
    {
        retryCount = 0;
#if UNITY_EDITOR
        if (BMUtil.Instance.isNetDisconnect == true)
        {
            retryCount = 4;

            HttpResponseMessage<string> response = new HttpResponseMessage<string>();
            response.Exception = new Exception("Net Error");

            BMUtil.Instance.ReTryNetConnect(GetReceive(uri, responseCallback), retryCount, delegate { responseCallback(response); });
        }
        else
        {
            BMUtil.Instance.StartNetCoroutine(GetReceive(uri, responseCallback));
        }
#else
        BMUtil.Instance.StartNetCoroutine(GetReceive(uri, responseCallback));
#endif
    }

    public IEnumerator GetReceive(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
    {
        //LogManager.Log("RestBase.Get(" + uri + ")");

        retryCount++;

        HttpClient client = new HttpClient();
        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);

        client.GetString(uri, response =>
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data.IndexOf("exception_status_code") > -1)
                {
                    try
                    {
                        ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);

                        if (exceptionVo.exception_status_code > 200)
                        {
                            response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        }
                    }
                    catch (Exception e)
                    {
                        LogManager.LogError("RestBase.Post Error : " + e.Message);
                        BMUtil.Instance.ReTryNetConnect(GetReceive(uri, responseCallback), retryCount);
                    }
                }

                responseCallback(response);
                BMUtil.Instance.CloseLoading();
            }
            else
            {
                //네트워크 연결 실패(오류) 메시지
                BMUtil.Instance.ReTryNetConnect(GetReceive(uri, responseCallback), retryCount, delegate { responseCallback(response); }, response.StatusCode, response.Exception.Message);
                BMUtil.Instance.CloseLoading();
            }
        });

        LogManager.Log("RestBase.Get(" + uri + ") END");

        yield return null;
    }

    public void Patch(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
    {
        //LogManager.Log("RestBase.Patch(" + uri + ")");

        retryCount = 0;

#if UNITY_EDITOR
        if (BMUtil.Instance.isNetDisconnect == true)
        {
            retryCount = 4;

            HttpResponseMessage<string> response = new HttpResponseMessage<string>();
            response.Exception = new Exception("Net Error");

            BMUtil.Instance.ReTryNetConnect(PatchReceive(uri, content, responseCallback, uploadStatusCallback), retryCount, delegate { responseCallback(response); });
        }
        else
        {
            BMUtil.Instance.StartNetCoroutine(PatchReceive(uri, content, responseCallback, uploadStatusCallback));
        }
#else

        BMUtil.Instance.StartNetCoroutine(PatchReceive(uri, content, responseCallback, uploadStatusCallback));
#endif
        /*
        HttpClient client = new HttpClient();
        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);
        client.Patch(uri, content, response =>
        {
            if (string.IsNullOrEmpty(response.Data))
            {
                responseCallback(response);
            }
            else
            {
                try
                {
                    ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);
                    if (exceptionVo.exception_status_code > 200)
                    {
                        response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        LogManager.LogWarning("RestBase.Patch Warning(" + exceptionVo.exception_status_code + ") : " + exceptionVo.message);
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogError("RestBase.Patch Error : " + e.Message);
                }
                responseCallback(response);
            }
        }, uploadStatusCallback);
        */
    }

    public IEnumerator PatchReceive(Uri uri, IHttpContent content, Action<HttpResponseMessage<string>> responseCallback, Action<UploadStatusMessage> uploadStatusCallback = null)
    {
        retryCount++;

        HttpClient client = new HttpClient();
        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);
        client.Patch(uri, content, response =>
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data.IndexOf("exception_status_code") > -1)
                {
                    try
                    {
                        ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);

                        if (exceptionVo.exception_status_code > 200)
                        {
                            response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        }
                    }
                    catch (Exception e)
                    {
                        LogManager.LogError("RestBase.Post Error : " + e.Message);
                        BMUtil.Instance.ReTryNetConnect(PatchReceive(uri, content, responseCallback, uploadStatusCallback), retryCount);
                    }
                }

                responseCallback(response);
            }
            else
            {
                //네트워크 연결 실패(오류) 메시지
                BMUtil.Instance.ReTryNetConnect(PatchReceive(uri, content, responseCallback, uploadStatusCallback), retryCount, delegate { responseCallback(response); }, response.StatusCode, response.Exception.Message);
            }

            BMUtil.Instance.CloseLoading();

        }, uploadStatusCallback);

        yield return null;
    }

    public void Delete(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
    {
        //LogManager.Log("RestBase.Delete(" + uri + ")");

        retryCount = 0;

#if UNITY_EDITOR
        if (BMUtil.Instance.isNetDisconnect == true)
        {
            retryCount = 4;

            HttpResponseMessage<string> response = new HttpResponseMessage<string>();
            response.Exception = new Exception("Net Error");

            BMUtil.Instance.ReTryNetConnect(DeleteReceive(uri, responseCallback), retryCount, delegate { responseCallback(response); });
        }
        else
        {
            BMUtil.Instance.StartNetCoroutine(DeleteReceive(uri, responseCallback));
        }
#else
        BMUtil.Instance.StartNetCoroutine(DeleteReceive(uri, responseCallback));
#endif

        /*
        HttpClient client = new HttpClient();
        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);

        client.Delete(uri, response =>
        {
            if (string.IsNullOrEmpty(response.Data))
            {
                responseCallback(response);
            }
            else
            {
                try
                {
                    ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);
                    if (exceptionVo.exception_status_code > 200)
                    {
                        response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        LogManager.LogWarning("RestBase.Delete Warning(" + exceptionVo.exception_status_code + ") : " + exceptionVo.message);
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogError("RestBase.Delete Error : " + e.Message);
                }
                responseCallback(response);
            }

        });
        */
    }

    public IEnumerator DeleteReceive(Uri uri, Action<HttpResponseMessage<string>> responseCallback)
    {
        retryCount++;

        HttpClient client = new HttpClient();
        if (currentTokenVo != null)
            client.Headers.Add(HttpRequestHeader.Authorization, currentTokenVo.access_token);

        client.Delete(uri, response =>
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data.IndexOf("exception_status_code") > -1)
                {
                    try
                    {
                        ExceptionVo exceptionVo = JsonUtility.FromJson<ExceptionVo>(response.Data);

                        if (exceptionVo.exception_status_code > 200)
                        {
                            response.Exception = new BrushMonException(exceptionVo.exception_status_code, exceptionVo.message);
                        }
                    }
                    catch (Exception e)
                    {
                        LogManager.LogError("RestBase.Post Error : " + e.Message);
                        BMUtil.Instance.ReTryNetConnect(DeleteReceive(uri, responseCallback), retryCount);
                    }
                }

                responseCallback(response);
            }
            else
            {
                //네트워크 연결 실패(오류) 메시지
                BMUtil.Instance.ReTryNetConnect(DeleteReceive(uri, responseCallback), retryCount, delegate { responseCallback(response); }, response.StatusCode, response.Exception.Message);
            }
            BMUtil.Instance.CloseLoading();
        });

        yield return null;
    }

    public void CheckConnectServer(Action<bool> isConnect, Action<Exception> exception)
    {
        HttpClient client = new HttpClient();

#if UNITY_EDITOR
        if(BMUtil.Instance.isNetDisconnect == true)
        {
            client.Timeout = 1;
        }
        else
        {
            client.Timeout = 5000;
        }
        
#else
        client.Timeout = 5000;
#endif

        Uri uri = new Uri(RestService.ServiceUrl + "/hi");

        client.GetString(uri, callback =>
        {
            if (callback.Exception == null)
            {
                isConnect(true);
            }
            else
            {
                LogManager.LogWarning(callback.StatusCode + "(" + ((int)callback.StatusCode) + ") : " + callback.Exception.Message);

                if (exception != null)
                    exception(callback.Exception);
                else
                    LogManager.Log("exception == null?????????");
            }
        });
    }

    public string DictToString (Dictionary<string, string> items, string format) {

        format = String.IsNullOrEmpty (format) ? "{0}='{1}' " : format;

        string itemString = "";
        foreach (var item in items) {
            itemString = itemString + String.Format (format, item.Key, item.Value);
        }

        return itemString;
    }
}