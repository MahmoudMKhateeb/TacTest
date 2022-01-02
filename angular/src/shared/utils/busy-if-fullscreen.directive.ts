import { ComponentFactoryResolver, Directive, Injector, Input, OnChanges, SimpleChanges, ViewContainerRef } from '@angular/core';
import { NgxSpinnerComponent, NgxSpinnerService } from 'ngx-spinner';

@Directive({
  selector: '[busyIfFullscreen]',
})
export class BusyIfFullscreenDirective implements OnChanges {
  constructor(private _viewContainer: ViewContainerRef, private _componentFactoryResolver: ComponentFactoryResolver, private _injector: Injector) {
    this.ngxSpinnerService = _injector.get(NgxSpinnerService);
    this.loadComponent();
  }

  private static index = 0;
  @Input() busyIfFullscreen: boolean;
  ngxSpinnerService: NgxSpinnerService;
  private spinnerName = '';

  isBusy = false;
  refreshState(): void {
    if (this.isBusy === undefined || this.spinnerName === '') {
      return;
    }

    setTimeout(() => {
      if (this.isBusy) {
        this.ngxSpinnerService.show(this.spinnerName);
      } else {
        this.ngxSpinnerService.hide(this.spinnerName);
      }
    }, 10);
  }

  loadComponent() {
    const componentFactory = this._componentFactoryResolver.resolveComponentFactory(NgxSpinnerComponent);
    const componentRef = this._viewContainer.createComponent(componentFactory);
    this.spinnerName = 'BusyIfFullscreenSpinner-' + BusyIfFullscreenDirective.index++ + '-' + Math.floor(Math.random() * 1000000); // generate random name
    let component = <NgxSpinnerComponent>componentRef.instance;
    component.name = this.spinnerName;
    component.fullScreen = true;
    component.type = 'ball-clip-rotate';
    component.size = 'medium';
    component.color = '#dc3545';
    // console.log('component', component);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.busyIfFullscreen) {
      this.isBusy = changes.busyIfFullscreen.currentValue;
      this.refreshState();
    }
  }
}
