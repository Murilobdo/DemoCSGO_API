import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppFirebaseModule } from './app-firebase/app-firebase.module';

const routes: Routes = [];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    AppFirebaseModule
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
