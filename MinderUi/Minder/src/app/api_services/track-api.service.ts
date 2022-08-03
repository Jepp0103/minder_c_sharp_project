import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import axios, { AxiosRequestConfig, AxiosPromise } from 'axios';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TrackApiService {

  readonly APIUrl = "https://localhost:7151/api";
  constructor(private http:HttpClient) { }

  getTracks():Observable<any[]> {
    return this.http.get<any>(this.APIUrl + '/tracks');
  }

  getTrackById(id:number):Observable<any> {
    return this.http.get<any>(this.APIUrl + '/tracks/' + id);
  }

  searchTracks(searchString:string):Observable<any[]> {
    return this.http.get<any>(this.APIUrl + '/tracks/search/?searchString=' + searchString);
  }

  addTrack(val:any) {
    return this.http.post(this.APIUrl + '/tracks', val);
  }

  updateTrack(id:number, body:any) {
    console.log("body here:", body)
    return this.http.put(this.APIUrl + '/tracks/' + id, body);
  }

  deleteTrack(id:number) {
    return this.http.delete(this.APIUrl + '/tracks/' + id);
  }
}
