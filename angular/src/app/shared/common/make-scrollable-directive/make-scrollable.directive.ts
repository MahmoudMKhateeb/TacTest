import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appMakeScrollable]',
})
export class MakeScrollableDirective {
  @Input('cssClassSelector') cssClassSelector = 'scrollable-col';
  private element: HTMLElement;

  constructor(private elementRef: ElementRef) {
    this.element = this.elementRef.nativeElement;
  }

  @HostListener('wheel', ['$event']) onScroll(event: WheelEvent): void {
    const targetElement = event.target as HTMLElement;
    const scrollableColElement = targetElement.closest(`.${this.cssClassSelector}`);
    if (scrollableColElement) {
      event.stopPropagation();
    }
  }
}
