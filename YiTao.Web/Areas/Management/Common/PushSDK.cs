using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YiTao.Web.Areas.Management.Common
{
    public class PushSDK
    {
        public static string PushMessageToSingle(string s, String CLIENTID)
        {
            // 推送主类
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);

            TransmissionTemplate template = TransmissionTemplateDemo();
            template.TransmissionContent = s;

            // 单推消息模型
            SingleMessage message = new SingleMessage();
            message.IsOffline = false;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;

            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = CLIENTID;

            String pushResult = push.pushMessageToSingle(message, target);
            return pushResult;
        }


        public static string pushMessageToApp(string s)
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);

            AppMessage message = new AppMessage();
            /*消息模版：
                1.TransmissionTemplate:透传模板
                2.LinkTemplate:通知链接模板
                3.NotificationTemplate：通知透传模板
                4.NotyPopLoadTemplate：通知弹框下载模板
             */

            //TransmissionTemplate template =  TransmissionTemplateDemo();
            TransmissionTemplate template = TransmissionTemplateDemo();
            template.TransmissionContent = s;
            //NotificationTemplate template =  NotificationTemplateDemo();
            //template.Text = "new1234";
            //template.Title = "new2222";
            //LinkTemplate template = LinkTemplateDemo();
            //NotyPopLoadTemplate template = NotyPopLoadTemplateDemo();

            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;

            List<String> appIdList = new List<string>();
            appIdList.Add(APPID);

            List<String> phoneTypeList = new List<string>();    //通知接收者的手机操作系统类型
            phoneTypeList.Add("ANDROID");
            phoneTypeList.Add("IOS");

            List<String> provinceList = new List<string>();     //通知接收者所在省份
            //provinceList.Add("浙江");
            //provinceList.Add("上海");
            //provinceList.Add("北京");

            List<String> tagList = new List<string>();
            //tagList.Add("开心");

            message.AppIdList = appIdList;
            message.PhoneTypeList = phoneTypeList;
            message.ProvinceList = provinceList;
            message.TagList = tagList;


            String pushResult = push.pushMessageToApp(message);
            return pushResult;
            //System.Console.WriteLine("-----------------------------------------------");
            //System.Console.WriteLine("服务端返回结果：" + pushResult);
        }



        public static TransmissionTemplate TransmissionTemplateDemo()
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.TransmissionType = "1";            //应用启动类型，1：强制应用启动 2：等待应用启动
            template.TransmissionContent = "透传内容";  //透传内容
            //iOS推送需要的pushInfo字段
            //template.setPushInfo(actionLocKey, badge, message, sound, payload, locKey, locArgs, launchImage);
            template.setPushInfo("", 1, "亿淘通知", "defult", "", "", "", "");
            return template;
        }
        private static String APPID = "6Ir4CDPwGy6dkQFPGJ26v1";                     //您应用的AppId
        private static String APPKEY = "CsYnEEM1pE7UCaohiDvkL4";                    //您应用的AppKey
        private static String MASTERSECRET = "VJtxyilq6N9uwUGPbFNFr2";              //您应用的MasterSecret 
        private static String HOST = "http://sdk.open.api.igexin.com/apiex.htm"; 
    }
}