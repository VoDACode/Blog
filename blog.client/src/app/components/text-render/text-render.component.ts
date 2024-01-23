import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-text-render',
  templateUrl: './text-render.component.html',
  styleUrls: ['./text-render.component.css']
})
export class TextRenderComponent {
  @Input()
  text: string = '';

  private processedText: string = '';

  get content() {
    this.processText();
    return this.processedText;
  }

  private processText() {
    this.processedText = this.text;

    // fix html tags
    this.processedText = this.processedText.replace(/(\&)/g, '&amp;');
    this.processedText = this.processedText.replace(/(\")/g, '&quot;');
    this.processedText = this.processedText.replace(/(\')/g, '&#x27;');
    this.processedText = this.processedText.replace(/(\\`)/g, '&#x60;');

    // '- ' -> ul
    this.processedText = this.processedText.replace(/(\n|^)(\- ([^\n]+))+/gm, '<ul>$&</ul>');
    // '1. ' -> ol
    this.processedText = this.processedText.replace(/(\n|^)(\d\. ([^\n]+))+/gm, '<ol>$&</ol>');
    // '> ' -> blockquote
    this.processedText = this.processedText.replace(/(\n|^)\> ([^\n]+)+/gm, '<blockquote>$2</blockquote>');
    // '* ' -> li
    this.processedText = this.processedText.replace(/(\n|^)\* ([^\n]+)+/gm, '<li>$2</li>');

    this.processedText = this.processedText.replace(/(\*\*([^\*]+)\*\*)/g, '<b>$2</b>');
    this.processedText = this.processedText.replace(/(\*([^\*]+)\*)/g, '<i>$2</i>');
    this.processedText = this.processedText.replace(/(\~\~([^\~]+)\~\~)/g, '<s>$2</s>');
    this.processedText = this.processedText.replace(/(\_\_([^\_]+)\_\_)/g, '<u>$2</u>');
    this.processedText = this.processedText.replace(/(\`{3}([^\`]+)\`{3})/g, '<pre><code>$2</code></pre>');
    this.processedText = this.processedText.replace(/(\!\[([^\]]+)\]\(([^\)]+)\))/g, '<img src="$3" alt="$2">');
    this.processedText = this.processedText.replace(/(\[([^\]]+)\]\(([^\)]+)\))/g, '<a href="$3" target="_blank">$2</a>');

    for (let i = 1; i <= 6; i++) {
      let regex = new RegExp(`^#{${i}}\\s([^\\n]+)`, 'gm');
      this.processedText = this.processedText.replace(regex, `<h${i}>$1</h${i}><hr>`);
    }

    this.processedText = this.processedText.replace(/!?(^#{1}\s[^\n]+)(\n{1})/gm, '<br>');
  }


}
