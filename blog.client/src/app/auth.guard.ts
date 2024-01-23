import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { AuthApiService } from "./services/auth-api.service";

@Injectable()
export class AuthGuard implements CanActivate  {
    constructor(public auth: AuthApiService, public router: Router) { } 
    canActivate(): boolean {
        if (!this.auth.isAuthenticated) {
            this.router.navigate(['/login']);
            return false;
        }
        return true;
    }
}