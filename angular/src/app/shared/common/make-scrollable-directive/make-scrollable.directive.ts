import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appMakeScrollable]',
})
export class MakeScrollableDirective {
  private element: HTMLElement;

  constructor(private elementRef: ElementRef) {
    this.element = this.elementRef.nativeElement;
  }

  @HostListener('scroll') onScroll(event: WheelEvent) {
    // do something on scroll
    const { scrollTop, scrollHeight, clientHeight } = this.element;
    const atTop = scrollTop === 0;
    const atBottom = scrollTop + clientHeight >= scrollHeight;
    const delta = event.deltaY;

    if (atTop && delta > 0) {
      event.preventDefault();
    } else if (atBottom && delta < 0) {
      event.preventDefault();
    }
  }

  // @HostListener('wheel', ['$event']) onWheel(event: WheelEvent) {
  //     event.stopPropagation();
  // }
  //
  // @HostListener('touchmove', ['$event']) onTouchMove(event: TouchEvent) {
  //     event.stopPropagation();
  // }
}
