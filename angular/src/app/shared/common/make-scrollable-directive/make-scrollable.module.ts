import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MakeScrollableDirective } from '@app/shared/common/make-scrollable-directive/make-scrollable.directive';

@NgModule({
  imports: [CommonModule],
  declarations: [MakeScrollableDirective],
  exports: [MakeScrollableDirective],
})
export class MakeScrollableModule {}
