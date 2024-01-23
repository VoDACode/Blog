export class UserResponseModel{
    id: number = 0;
    username: string = '';
    isAdmin: boolean = false;
    canPublish: boolean = false;
    isBanned: boolean = false;
    email: string | null = null;
}

export class UserUpdateRequestModel{
    username: string = '';
    email: string = '';
    password: string = '';
}

export class UserLoginRequestModel{
    username: string = '';
    password: string = '';
}

export class UserRegisterRequestModel{
    username: string = '';
    email: string = '';
    password: string = '';
}