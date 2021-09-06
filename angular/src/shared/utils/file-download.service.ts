import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { FileDto } from '@shared/service-proxies/service-proxies';

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
  }

  /**
   * downloads a file by Binary Id
   */
  downloadFileByBinaryId(fileName: string, BinaryToken: string) {
    const url = AppConsts.remoteServiceBaseUrl + '/File/DownloadBinaryFile?id=' + BinaryToken + '&contentType=application/zip&fileName=' + fileName;
    location.href = url;
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
}
