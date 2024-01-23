import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TextRenderComponent } from './text-render.component';

describe('TextRenderComponent', () => {
  let component: TextRenderComponent;
  let fixture: ComponentFixture<TextRenderComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TextRenderComponent]
    });
    fixture = TestBed.createComponent(TextRenderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
