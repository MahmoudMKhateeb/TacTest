import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { FileDto } from '@shared/service-proxies/service-proxies';
import { GetAllUploadedFileDto } from '@shared/service-proxies/service-proxies';

@Injectable()
export class FileDownloadService {
  downloadTempFile(file: FileDto) {
    const url =
      AppConsts.remoteServiceBaseUrl +
      '/File/DownloadTempFile?fileType=' +
      file.fileType +
      '&fileToken=' +
      file.fileToken +
      '&fileName=' +
      file.fileName;
    location.href = url; //TODO: This causes reloading of same page in Firefox
    return url;
  }

  /**
   * downloads a file by Binary Id
   */
  downloadFileByBinaryId(fileName: string, BinaryToken: string) {
    const url = AppConsts.remoteServiceBaseUrl + '/File/DownloadBinaryFile?id=' + BinaryToken + '&contentType=application/zip&fileName=' + fileName;
    location.href = url;
  }
  downloadFileByBinary(documentId: string, fileName: string, contentType: string): string {
    const url =
      AppConsts.remoteServiceBaseUrl + '/File/DownloadBinaryFile?id=' + documentId + '&contentType=' + contentType + '&fileName=' + fileName;
    location.href = url;
    return url;
  }

  GetTempFileUrl(file: FileDto): string {
    return (
      AppConsts.remoteServiceBaseUrl +
      '/File/DownloadTempFile?fileType=' +
      file.fileType +
      '&fileToken=' +
      file.fileToken +
      '&fileName=' +
      file.fileName
    );
  }
  GetBunaryFileUrl(file: GetAllUploadedFileDto): string {
    return (
      AppConsts.remoteServiceBaseUrl +
      '/File/DownloadBinaryFile?id=' +
      file.documentId +
      '&contentType=' +
      file.fileType +
      '&fileName=' +
      file.fileName
    );
  }
}
