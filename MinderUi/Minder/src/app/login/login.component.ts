import { Component, OnInit } from '@angular/core';
import { AuthenticationApiService } from '../api-services/authentication-api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  Email:string;
  Password:string;
  router:Router;

  constructor(private service: AuthenticationApiService, route:Router) {
    this.Email = "";
    this.Password = "";
    this.router = route;
   }

  ngOnInit(): void {
  }

  loginCustomer() {
    const content = {
      Email: this.Email,
      Password: this.Password
    }

    this.service.validateUser(content).subscribe(data => {
      if(data === true) {
        sessionStorage.setItem('SessionKeyEmail', this.Email);
        this.router.navigate(['']);
      } else {
        alert("User does not exist.");
      }
    });
  }



}
