import { Routes } from "@angular/router";
import { HomePageComponent } from "./components/home-page/home-page.component";
import { LoginPageComponent } from "./components/login-page/login-page.component";
import { PostComponent } from "./components/post/post.component";
import { AuthGuard } from "./auth.guard";
import { CreatePostComponent } from "./components/create-post/create-post.component";
import { UserProfileComponent } from "./components/user-profile/user-profile.component";

export const routes: Routes = [
    { path: '', component: HomePageComponent },
    { path: 'login', component: LoginPageComponent },
    {
        path: 'post', children: [
            { path: 'new', component: CreatePostComponent, canActivate: [AuthGuard] },
            { path: ':id', component: PostComponent }
        ]
    },
    { path: 'profile', component: UserProfileComponent, canActivate: [AuthGuard] },
    { path: '**', redirectTo: '' }
];