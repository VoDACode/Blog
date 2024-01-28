import { environment } from "src/environments/environment";
import { FileModelResponse } from "./file.model";

export class PostModelRequest {
    title: string = "";
    content: string = "";
    isPublished: boolean = false;
    hasComments: boolean = false;
    tags: string[] = [];
}

export class PostModelResponse implements PostModelRequest {
    id: number = 0;
    title: string = "";
    content: string = "";
    createdAt: Date = new Date();
    updatedAt: Date | null = null;
    hasComments: boolean = false;
    isPublished: boolean = false;
    author: string = "";
    tags: string[] = [];
    files: FileModelResponse[] = [];
}

export class PostFileModel {
    private _uuid: string = '';
    public get uuid() {
        return this._uuid;
    }
    private _objectUrl: string = '';
    file: FileModelResponse | File;
    get name() {
        if (this.file instanceof File) {
            return this.file.name;
        } else {
            return this.file.name;
        }
    }

    get size() {
        if (this.file instanceof File) {
            return this.file.size;
        } else {
            return this.file.size;
        }
    }

    get contentType() {
        if (this.file instanceof File) {
            return this.file.type;
        } else {
            return this.file.contentType;
        }
    }

    get url() {
        if (this.file instanceof File) {
            if (this._objectUrl == '') {
                this._objectUrl = URL.createObjectURL(this.file);
            }
            return this._objectUrl;
        } else {
            return `${environment.apiBaseUrl}/api/file/${this.file.id}`
        }
    }

    public progress: number = 0;

    public get isUploaded() {
        return this.file instanceof FileModelResponse;
    }

    public processing: boolean = false;

    constructor(file: FileModelResponse | File) {
        this.file = file;
        this._uuid = this.uuidv4();
    }

    private uuidv4() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });
    }
}