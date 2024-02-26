import { Routes } from "@angular/router";
import { HomePageComponent } from "./components/home-page/home-page.component";
import { LoginPageComponent } from "./components/login-page/login-page.component";
import { PostComponent } from "./components/post/post.component";
import { AuthGuard } from "./auth.guard";
import { CreatePostComponent } from "./components/create-post/create-post.component";
import { UserProfileComponent } from "./components/user-profile/user-profile.component";
import { TestPageComponent } from "./components/test-page/test-page.component";

export const routes: Routes = [
    { path: '', component: HomePageComponent },
    { path: 'search', component: HomePageComponent },
    { path: 'login', component: LoginPageComponent },
    {
        path: 'post', children: [
            { path: '', redirectTo: '/', pathMatch: 'full' },
            { path: 'new', component: CreatePostComponent, canActivate: [AuthGuard] },
            {
                path: ':id', children: [
                    { path: '', component: PostComponent },
                    {
                        path: 'edit', component: CreatePostComponent, data: { editMode: true }, canActivate: [AuthGuard]
                    }
                ]
            }
        ]
    },
    { path: 'profile', component: UserProfileComponent, canActivate: [AuthGuard] },
    { path: 'test', component: TestPageComponent, canActivate: [AuthGuard] },
    { path: '**', redirectTo: '' }
];