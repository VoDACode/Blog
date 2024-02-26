import { Component, Input, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-text-render',
  templateUrl: './text-render.component.html',
  encapsulation: ViewEncapsulation.None
})
export class TextRenderComponent {
  @Input()
  text: string = '';
}
