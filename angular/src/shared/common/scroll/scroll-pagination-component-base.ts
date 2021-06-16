import { HostListener, Injector } from '@angular/core';
import { AppComponentBase } from '../app-component-base';

export abstract class ScrollPagnationComponentBase extends AppComponentBase {
  skipCount = 0;
  maxResultCount = 6;
  IsLoading = true;
  StopLoading = false;
  scrollY = 10;
  constructor(injector: Injector) {
    super(injector);
  }

  abstract LoadData();
  @HostListener('window:scroll', [])
  onScroll(): void {
    if (this.bottomReached() && !this.IsLoading && !this.StopLoading) {
      this.scrollY = Math.ceil(window.scrollY);
      //console.log(`(${window.innerHeight} + ${Math.ceil(window.scrollY)}) >= ${document.body.offsetHeight}+${this.scrollY}`);
      this.skipCount += this.maxResultCount;
      this.IsLoading = true;
      this.LoadData();
    }
  }
  bottomReached(): boolean {
    return window.innerHeight + Math.ceil(window.scrollY) > document.body.offsetHeight + this.scrollY;
  }
}
