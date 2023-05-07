using FYPBackEnd.Data.Constants;
using FYPBackEnd.Data.ReturnedResponse;
using FYPBackEnd.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using StatusCodes = FYPBackEnd.Data.Constants.StatusCodes;

namespace FYPBackEnd.Services.Implementation
{
    public class GoogleDrive: IGoogleDrive
    {
        private const string DirectoryId = "1cmBb_P3uOK5C17yplAhtJJRXIzKA8o75";
       // private const string uploadFilenme = "testing";
        private const string serviceAccountEmail = "fypbackend@fypbackend2023.iam.gserviceaccount.com";
        //private const string filePath = @"C:\Users\BEBS\Pictures\passport(2).jpg";
        public async Task<ApiResponse> UploadFileWithMetaData(IFormFile requestFile)
        {
            try
            {

                var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);


                var credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[] { DriveService.Scope.Drive , DriveService.Scope.DriveFile, DriveService.Scope.DriveAppdata}
               }.FromCertificate(certificate));

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "FYPBackEnd",
                });

                var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = requestFile.FileName,
                    MimeType = "image/jpeg",
                    Parents = new[] { DirectoryId }
                };

                
                // Create a new file on drive.
                await using (var stream = requestFile.OpenReadStream())
                {
                    // Create a new file, with metadata and stream.
                    var request = service.Files.Create(fileMetaData, stream, "image/jpeg");
                    //request.Fields = "id";
                     var result = await request.UploadAsync(CancellationToken.None);

                    if(result.Status == UploadStatus.Failed)
                    {
                        return ReturnedResponse.ErrorResponse(null, null, StatusCodes.GeneralError);
                    }
                    var file = request.ResponseBody;
                    return ReturnedResponse.SuccessResponse(file.Id, file, StatusCodes.Successful);
                }

                
            }
            catch(Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message, ex.InnerException.ToString(), StatusCodes.GeneralError);
            }
        }
    }
}