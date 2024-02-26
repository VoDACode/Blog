import { HttpClient, HttpClientModule, provideHttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { PostComponent } from './components/post/post.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { RouterModule } from '@angular/router';
import { routes } from './app.routes';
import { ImagePreviewComponent } from './components/image-preview/image-preview.component';
import { TextRenderComponent } from './components/text-render/text-render.component';
import { FormsModule } from '@angular/forms';
import { AuthGuard } from './auth.guard';
import { CreatePostComponent } from './components/create-post/create-post.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { TestPageComponent } from './components/test-page/test-page.component';
import { ProgressBarComponent } from './components/progress-bar/progress-bar.component';

import { provideMarkdown, MarkdownModule } from 'ngx-markdown';

@NgModule({
  declarations: [
    AppComponent,
    NavigationComponent,
    PostComponent, CreatePostComponent, ImagePreviewComponent, TextRenderComponent,
    LoginPageComponent,
    HomePageComponent,
    UserProfileComponent,
    ProgressBarComponent,
    TestPageComponent
  ],
  imports: [
    FormsModule,
    BrowserModule,
    HttpClientModule,
    MarkdownModule.forRoot({ loader: HttpClient }),
    RouterModule.forRoot(routes)
  ],
  providers: [AuthGuard, provideHttpClient(), provideMarkdown({ loader: HttpClient })],
  bootstrap: [AppComponent]
})
export class AppModule { }
