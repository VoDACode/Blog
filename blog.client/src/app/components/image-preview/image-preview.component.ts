import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PostFileModel } from 'src/app/models/post.model';

@Component({
  selector: 'app-image-preview',
  templateUrl: './image-preview.component.html',
  styleUrls: ['./image-preview.component.css']
})
export class ImagePreviewComponent {
  @Input()
  set images(images: PostFileModel[]) {
    this.imageModels = images.map(image => new ImageModel(image, false));
    if (this.imageModels.length > 0) {
      this.imageModels[0].selected = true;
    }
  }

  @Input()
  public canDelete: boolean = false;


  @Output()
  public delete: EventEmitter<PostFileModel> = new EventEmitter<PostFileModel>();

  public imageModels: ImageModel[] = [];

  public get selectedImageUrl(): string {
    const selectedImage = this.imageModels.find(image => image.selected);
    return selectedImage ? selectedImage.file.url : '';
  }

  public selectImage(image: ImageModel): void {
    this.imageModels.forEach(image => image.selected = false);
    image.selected = true;
  }

  public deleteImage(image: ImageModel): void {
    this.delete.emit(image.file);
    var index = this.imageModels.indexOf(image);
    let deletedImage = this.imageModels[index];
    if (deletedImage.selected) {
      if (this.imageModels.length > 1) {
        if (index + 1 > this.imageModels.length - 1) {
          this.imageModels[index - 1].selected = true;
        } else {
          this.imageModels[index + 1].selected = true;
        }
      }
    }
    this.imageModels = this.imageModels.filter(img => img !== image);
  }
}

class ImageModel {
  file: PostFileModel;
  selected: boolean;

  constructor(file: PostFileModel, selected: boolean) {
    this.file = file;
    this.selected = selected;
  }
}
