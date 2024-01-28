import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.css']
})
export class ProgressBarComponent {
  private roundingFactor: number = 0;

  @Input()
  public progress: number = 50;
  @Input()
  public max: number = 100;
  @Input()
  public min: number = 0;
  @Input()
  public set rounding(value: number) {
    if (value < 0) {
      this.roundingFactor = 0;
      return;
    }
    this.roundingFactor = value;
  }

  public get progressPercentage(): number {
    let progress = (this.progress - this.min) / (this.max - this.min) * 100;
    return Math.round(progress * Math.pow(10, this.roundingFactor)) / Math.pow(10, this.roundingFactor);
  }

  public get progressWidth(): string {
    return `${this.progressPercentage}%`;
  }
}
