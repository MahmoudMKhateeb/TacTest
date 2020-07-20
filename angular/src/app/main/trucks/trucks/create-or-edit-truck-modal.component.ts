import {Component, EventEmitter, Injector, Output, ViewChild} from '@angular/core';
import {ModalDirective} from 'ngx-bootstrap/modal';
import {finalize} from 'rxjs/operators';
import {
    CreateOrEditTruckDto,
    TrucksServiceProxy,
    TruckTruckStatusLookupTableDto,
    TruckTrucksTypeLookupTableDto,
    UpdateTruckPictureInput
} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import * as moment from 'moment';
import {TruckUserLookupTableModalComponent} from './truck-user-lookup-table-modal.component';
import {base64ToFile, ImageCroppedEvent} from '@node_modules/ngx-image-cropper';
import {FileItem, FileUploader, FileUploaderOptions} from '@node_modules/ng2-file-upload';
import {IAjaxResponse, TokenService} from '@node_modules/abp-ng2-module';
import {AppConsts} from '@shared/AppConsts';

@Component({
    selector: 'createOrEditTruckModal',
    templateUrl: './create-or-edit-truck-modal.component.html'
})
export class CreateOrEditTruckModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', {static: true}) modal: ModalDirective;
    @ViewChild('truckUserLookupTableModal', {static: true}) truckUserLookupTableModal: TruckUserLookupTableModalComponent;
    @ViewChild('truckUserLookupTableModal2', {static: true}) truckUserLookupTableModal2: TruckUserLookupTableModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    truck: CreateOrEditTruckDto = new CreateOrEditTruckDto();

    trucksTypeDisplayName = '';
    truckStatusDisplayName = '';
    userName = '';
    userName2 = '';

    allTrucksTypes: TruckTrucksTypeLookupTableDto[];
    allTruckStatuss: TruckTruckStatusLookupTableDto[];

    imageChangedEvent: any = '';
    public maxProfilPictureBytesUserFriendlyValue = 5;
    public uploader: FileUploader;
    public temporaryPictureUrl: string;
    private _uploaderOptions: FileUploaderOptions = {};

    profilePicture: string;



    constructor(
        injector: Injector,
        private _trucksServiceProxy: TrucksServiceProxy,
        private _tokenService: TokenService
    ) {
        super(injector);
    }

    show(truckId?: string): void {

        if (!truckId) {
            this.truck = new CreateOrEditTruckDto();
            this.truck.id = truckId;
            this.truck.licenseExpirationDate = moment().startOf('day');
            this.trucksTypeDisplayName = '';
            this.truckStatusDisplayName = '';
            this.userName = '';
            this.userName2 = '';

            this.active = true;
            this.modal.show();
        } else {
            this._trucksServiceProxy.getTruckForEdit(truckId).subscribe(result => {
                this.truck = result.truck;

                this.trucksTypeDisplayName = result.trucksTypeDisplayName;
                this.truckStatusDisplayName = result.truckStatusDisplayName;
                this.userName = result.userName;
                this.userName2 = result.userName2;

                this.active = true;
                this.modal.show();
            });
        }
        this._trucksServiceProxy.getAllTrucksTypeForTableDropdown().subscribe(result => {
            this.allTrucksTypes = result;
        });
        this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe(result => {
            this.allTruckStatuss = result;
        });

        this.initializeModal();
    }

    initializeModal(): void {
        this.active = true;
        this.temporaryPictureUrl = '';
        this.initFileUploader();
    }

    save(): void {
        this.saving = true;
        this.uploader.uploadAll();


    }

    openSelectUserModal() {
        this.truckUserLookupTableModal.id = this.truck.driver1UserId;
        this.truckUserLookupTableModal.displayName = this.userName;
        this.truckUserLookupTableModal.show();
    }
    openSelectUserModal2() {
        this.truckUserLookupTableModal2.id = this.truck.driver2UserId;
        this.truckUserLookupTableModal2.displayName = this.userName;
        this.truckUserLookupTableModal2.show();
    }


    setDriver1UserIdNull() {
        this.truck.driver1UserId = null;
        this.userName = '';
    }
    setDriver2UserIdNull() {
        this.truck.driver2UserId = null;
        this.userName2 = '';
    }


    getNewDriver1UserId() {
        this.truck.driver1UserId = this.truckUserLookupTableModal.id;
        this.userName = this.truckUserLookupTableModal.displayName;
    }

    getNewDriver2UserId() {
        this.truck.driver2UserId = this.truckUserLookupTableModal2.id;
        this.userName2 = this.truckUserLookupTableModal2.displayName;
    }


    close(): void {
        this.active = false;
        this.imageChangedEvent = '';
        this.uploader.clearQueue();
        this.modal.hide();
    }

    fileChangeEvent(event: any): void {
        if (event.target.files[0].size > 5242880) { //5MB
            this.message.warn(this.l('ProfilePicture_Warn_SizeLimit', this.maxProfilPictureBytesUserFriendlyValue));
            return;
        }

        this.imageChangedEvent = event;
    }

    imageCroppedFile(event: ImageCroppedEvent) {
        this.uploader.clearQueue();
        this.uploader.addToQueue([<File>base64ToFile(event.base64)]);
    }

    initFileUploader(): void {
        this.uploader = new FileUploader({url: AppConsts.remoteServiceBaseUrl + '/Profile/UploadProfilePicture'});
        this._uploaderOptions.autoUpload = false;
        this._uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        this._uploaderOptions.removeAfterUpload = true;
        this.uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        this.uploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
            form.append('FileType', fileItem.file.type);
            form.append('FileName', 'ProfilePicture');
            form.append('FileToken', this.guid());
        };

        this.uploader.onSuccessItem = (item, response, status) => {
            const resp = <IAjaxResponse>JSON.parse(response);
            if (resp.success) {
                //this.updateProfilePicture(resp.result.fileToken);
                this.truck.updateTruckPictureInput = new UpdateTruckPictureInput();
                this.truck.updateTruckPictureInput.fileToken = resp.result.fileToken;
                this.truck.updateTruckPictureInput.x = 0;
                this.truck.updateTruckPictureInput.y = 0;
                this.truck.updateTruckPictureInput.width = 0;
                this.truck.updateTruckPictureInput.height = 0;
                this._trucksServiceProxy.createOrEdit(this.truck)
                    .pipe(finalize(() => {
                        this.saving = false;
                    }))
                    .subscribe(() => {
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.close();
                        this.modalSave.emit(null);
                    });
            } else {
                this.message.error(resp.error.message);
            }
        };

        this.uploader.setOptions(this._uploaderOptions);
    }

    guid(): string {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }


    getTruckPicture(truckId: number): void {
        if (!truckId) {
            this.profilePicture = this.appRootUrl() + 'assets/common/images/default-profile-picture.png';
            return;
        }

        this._profileService.getProfilePictureByUser(truckId).subscribe(result => {
            if (result && result.profilePicture) {
                this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
            } else {
                this.profilePicture = this.appRootUrl() + 'assets/common/images/default-profile-picture.png';
            }
        });
    }

}
