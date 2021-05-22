import {
  AngularFireStorage,
  AngularFireUploadTask,
} from '@angular/fire/storage';
import { from, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

export interface FilesUploadMetadata {
  uploadProgress$: Observable<number>;
  downloadUrl$: Observable<string>;
}

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor(private readonly storage: AngularFireStorage, private http: HttpClient) { }

  uploadFileAndGetMetadata(mediaFolderPath: string, fileToUpload: File): FilesUploadMetadata {
    const { name } = fileToUpload;
    const filePath = `${mediaFolderPath}/${new Date().getTime()}_${name}`;
    const uploadTask: AngularFireUploadTask = this.storage.upload(filePath,fileToUpload);
      // .then(data => { this.SendURL(filePath) })
      // .catch(error => console.log(error));

    return <FilesUploadMetadata>{
      uploadProgress$: uploadTask.percentageChanges(),
      downloadUrl$: this.getDownloadUrl$(uploadTask, filePath),
    };
  }

  public SendURL(urlDemo: string): any{
    console.log(urlDemo)
    var url = `${environment.URL_API}LoadData?pathDEMO=${urlDemo}&token=${urlDemo.split("&token")[1]}`
    this.http.post(url, null)
        .subscribe(data => {
          console.log(data);
        })
    // this.http.post(`${environment.URL_API}LoadData`, {pathDEMO: urlDemo}, {responseType: 'text'})
    //     .subscribe(data => {
    //       console.log(data);
    //     })
  }

  private getDownloadUrl$(uploadTask: AngularFireUploadTask, path: string): Observable<string> {
    return from(uploadTask).pipe(
      switchMap((_) => this.storage.ref(path).getDownloadURL()),
    );
  }
}
