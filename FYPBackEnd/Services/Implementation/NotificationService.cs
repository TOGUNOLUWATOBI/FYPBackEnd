using CorePush.Google;
using FYPBackEnd.Data.Models.Notification;
using FYPBackEnd.Services.Interfaces;
using FYPBackEnd.Settings;
using Microsoft.Extensions.Options;
using static FYPBackEnd.Data.Models.Notification.GoogleNotification;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Data.Constants;

namespace FYPBackEnd.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        public NotificationService(IOptions<FcmNotificationSetting> settings)
        {
            _fcmNotificationSetting = settings.Value;
        }

        public async Task<ApiResponse> SendNotification(NotificationModel notificationModel)
        {
            ;
            try
            {
                if (notificationModel.IsAndroiodDevice)
                {
                    /* FCM Sender (Android Device) */
                    FcmSettings settings = new FcmSettings()
                    {
                        SenderId = _fcmNotificationSetting.SenderId,
                        ServerKey = _fcmNotificationSetting.ServerKey
                    };
                    HttpClient httpClient = new HttpClient();

                    string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                    string deviceToken = notificationModel.DeviceId;

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    DataPayload dataPayload = new DataPayload();
                    dataPayload.Title = notificationModel.Title;
                    dataPayload.Body = notificationModel.Body;

                    GoogleNotification notification = new GoogleNotification();
                    notification.Data = dataPayload;
                    notification.Notification = dataPayload;

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                    if (fcmSendResponse.IsSuccess())
                    {
                        return ReturnedResponse.SuccessResponse("Notification sent successfully", null, StatusCodes.Successful);
                    }
                    else
                    {
                        return ReturnedResponse.ErrorResponse(fcmSendResponse.Results[0].Error, null, StatusCodes.GeneralError);
                    }
                }
                else
                {
                    /* Code here for APN Sender (iOS Device) */
                    //var apn = new ApnSender(apnSettings, httpClient);
                    //await apn.SendAsync(notification, deviceToken);
                }
                return ReturnedResponse.ErrorResponse("An error occured", null, StatusCodes.GeneralError); ;
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse("An error occured", null, StatusCodes.GeneralError);
            }
        }
    }
}

